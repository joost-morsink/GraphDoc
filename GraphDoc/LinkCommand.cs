using System.Text;

namespace GraphDoc;

public abstract class LinkCommand(IGraphSerializer serializer) : ICommand
{
    protected abstract Task<IEnumerable<Link>> GetLinks();
    public async Task<string> Execute()
    {
        var links = await GetLinks();
        var sb = new StringBuilder();
        sb.AppendLine(serializer.LeadingTrivia);
        foreach(var link in links)
            sb.AppendLine(serializer.Serialize(link));
        sb.AppendLine(serializer.TrailingTrivia);
        return sb.ToString();
    }
}

public abstract class StructureCommand(IStructureSerializer serializer) : ICommand
{
    protected abstract Task<Structure> GetStructure();

    public async Task<string> Execute()
    {
        var structure = await GetStructure();
        return serializer.Serialize(structure);
    }
}
