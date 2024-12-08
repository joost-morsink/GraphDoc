namespace GraphDoc;

public interface ICommandDef
{
    static abstract string Name { get; }
    static abstract ICommand Parse(IReadOnlyList<string> positional, IReadOnlyDictionary<string, string> named);
}