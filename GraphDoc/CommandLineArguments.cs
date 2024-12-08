using System.Collections.Immutable;

namespace GraphDoc;

public static class CommandLineArguments
{

    private static readonly ImmutableDictionary<string, Type> CommandTypes = ImmutableDictionary.Create<string, Type>(StringComparer.OrdinalIgnoreCase)
        .Add<HelpCommand>()
        .Add<DataStructureCommand>();

    public static Func<IReadOnlyList<string>, IReadOnlyDictionary<string, string>, ICommand> CommandParser(string name)
        => CommandTypes.GetValueOrDefault(name, typeof(HelpCommand))
            .GetMethod(nameof(ICommandDef.Parse))!
            .CreateDelegate<Func<IReadOnlyList<string>, IReadOnlyDictionary<string, string>, ICommand>>();
    
    
    public static ICommand Parse(string[] args)
    {
        var (positional, named) = DetermineParameters(args);
        if (positional.Count == 0 && named.Count == 0)
            positional = ["help"];

        var p = CommandParser(positional[0]);
        return p(positional, named);
    }

    private static (IReadOnlyList<string> positional, IReadOnlyDictionary<string, string> named)
        DetermineParameters(string[] args)
    {
        var positional = new List<string>();
        var named = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        string? key = null;
        foreach (var arg in args)
        {
            if (arg.StartsWith('-'))
            {
                if (key is not null)
                    named[key] = "";
                key = arg.TrimStart('-', '/');
            }
            else if (key is null)

                positional.Add(arg);
            else
            {
                named[key] = arg;
                key = null;
            }
        }

        if (key is not null)
            named[key] = "";

        return (positional, named);
    }
}