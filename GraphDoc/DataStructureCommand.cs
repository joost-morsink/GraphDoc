using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.Loader;
using System.Xml.Linq;

namespace GraphDoc;
public class DataStructureCommand(IStructureSerializer Serializer, string Dll, string ClassName, string? Namespace) : StructureCommand(Serializer), ICommandDef
{
    private readonly string Directory = Path.GetDirectoryName(Dll)!;
    public static string Name => "datastructure";
    public static readonly char[] WHITESPACE = [' ', '\t', '\n', '\r' ];
    public static ICommand Parse(IReadOnlyList<string> positional, IReadOnlyDictionary<string, string> named)
    {
        var parameters = positional.Skip(1)
            .Zip(["dll", "classname"], (v, k) => new KeyValuePair<string, string>(k, v))
            .Concat(named)
            .ToCaseInsensitiveDictionary();
        
        var dll = parameters.GetRequired(nameof(Dll),"d");
        if(!Path.IsPathRooted(dll))
            dll = Path.Combine(System.IO.Directory.GetCurrentDirectory(), dll);
        var className = parameters.GetRequired(nameof(ClassName),"c");
        var @namespace = parameters.GetOptional(nameof(Namespace), "n");
        var serializer = parameters.GetOptional(nameof(Serializer), "s");
        
        
        return new DataStructureCommand(IStructureSerializer.Create(serializer), dll, className, @namespace);
    }
    
    protected override async Task<Structure> GetStructure()
    {
        await Task.Yield();
        AssemblyLoadContext.Default.Resolving += ResolveAssembly;
        Assembly asm;
        try
        {
            asm = Assembly.LoadFrom(Dll);
        }
        catch (Exception ex)
        {
            throw new GraphDocException("Could not load assembly", ex);
        }

        var xmlFile = Path.ChangeExtension(Dll, "xml");
        var xml = File.Exists(xmlFile)
            ? XElement.Load(xmlFile)
            : null;
        
        var type = asm.ExportedTypes
            .FirstOrDefault(t => string.Equals(t.Name, ClassName, StringComparison.OrdinalIgnoreCase)
                && (Namespace is null || string.Equals(t.Namespace, Namespace, StringComparison.OrdinalIgnoreCase)));
        if (type is null)
            throw new GraphDocException("Could not load type.");
        
        return GetStructure(type, xml);
    }

    private Structure GetStructure(Type type, XElement? documentation) 
    {
        return Inner(type);
        
        Structure Inner(Type innerType)
        {
            var innerDoc = GetDoc($"T:{innerType.FullName}");
            var desc = new Descriptor(innerType.Name, innerDoc);
            var props = innerType.GetProperties()
                .Select(prop =>
                {
                    var innerPropDoc = GetDoc($"P:{innerType.FullName}.{prop.Name}");
                    var underlying = UnderlyingType(prop.PropertyType);
                    var propDesc = new Descriptor($"{ShortName(prop.PropertyType)} {prop.Name}", innerPropDoc);
                    if (IsTerminal(underlying))
                        return (propDesc,
                            new Structure.Leaf(new($"{ShortName(prop.PropertyType)} {prop.Name}", innerPropDoc)));
                    return (propDesc, Inner(underlying));
                });
            return new Structure.Dict(desc, props.ToImmutableDictionary(x => x.Item1, x => x.Item2));

        }
        string ShortName(Type type)
            => type.GetGenericArguments().Length == 0
                ? type.Name
                : $"{type.Name.Split('`')[0]}<{string.Join(", ", type.GetGenericArguments().Select(ShortName))}>";
        
        bool IsTerminal(Type type)
            => UnNullable(type) switch
            {
                Type t when t.IsPrimitive || t == typeof(string) => true,
                Type t when t.IsEnum => true,
                Type t when t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>) => IsTerminal(t.GetGenericArguments()[0]),
                Type t when t.Namespace == "System" => true,
                _ => false
            };
        Type UnNullable(Type type)
            => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)
                ? type.GetGenericArguments()[0]
                : type;

        Type UnderlyingType(Type type)
            => type == typeof(string) 
                ? type
                : UnNullable((from itf in type.GetInterfaces()
                    let ga = itf.GetGenericArguments()
                    where ga.Length == 1 && itf.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                    select ga[0]).FirstOrDefault() ?? type);
        
        string? GetDoc(string name)
            => documentation
                ?.Elements("members")
                .Elements("member")
                .FirstOrDefault(e => e.Attribute("name")?.Value == name)
                ?.Element("summary")
                ?.Value
                .Trim(WHITESPACE);
    }

    private Assembly? ResolveAssembly(AssemblyLoadContext loadContext, AssemblyName name)
    {
        var path = Path.Combine(Directory, $"{name.Name}.dll");
        if (File.Exists(path))
        {
            return loadContext.LoadFromAssemblyPath(path);
        }

        return null;
    }
}