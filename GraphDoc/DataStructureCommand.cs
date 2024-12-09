using System.Reflection;
using System.Runtime.Loader;
using System.Xml.Linq;

namespace GraphDoc;

public class DataStructureCommand(ILinkSerializer Serializer, string Dll, string ClassName, string? Namespace) : LinkCommand(Serializer), ICommandDef
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
        
        
        return new DataStructureCommand(ILinkSerializer.Create(serializer), dll, className, @namespace);
    }
    
    protected override async Task<IEnumerable<Link>> GetLinks()
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
        
        return GetLinks(type, xml);
    }

    public IEnumerable<Link> GetLinks(Type type, XElement? documentation) 
    {
        var done = new HashSet<Type>();
        var result = new List<Link>();
        Inner(type);
        return result;
        
        void Inner(Type innerType)
        {
            if (done.Contains(innerType) || innerType.Namespace == "System")
                return;
            done.Add(innerType);
            var innerDoc = GetDoc($"T:{innerType.FullName}");
            foreach (var prop in innerType.GetProperties())
            {
                var innerPropDoc = GetDoc($"P:{innerType.FullName}.{prop.Name}");
                var underlying = UnderlyingType(prop.PropertyType);
                if(IsTerminal(underlying))
                    result.Add(new (new (innerType.Name,innerDoc), new ($"{ShortName(prop.PropertyType)} {prop.Name}",innerPropDoc), ""));
                else
                    result.Add(new (new(innerType.Name, innerDoc), underlying.Name, new(prop.Name, innerPropDoc)));
                Inner(underlying);
            }
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