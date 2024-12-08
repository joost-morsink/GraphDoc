namespace GraphDoc;

public class HelpCommand : ICommandDef, ICommand
{
    public static string Name => "help";
    public static ICommand Parse(IReadOnlyList<string> positional, IReadOnlyDictionary<string, string> named)
        => new HelpCommand();

    public Task<string> Execute()
        => Task.FromResult(@"
GraphDoc v1 - Convert dlls to mermaid or plant uml graphs
Usage: deps2mermaid {{command}}
Commands: 
    help        Show this help.");
}