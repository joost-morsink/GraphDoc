using System.Text;

namespace GraphDoc;

public abstract class LinkCommand(ILinkSerializer serializer) : ICommand
{
    protected abstract Task<IEnumerable<Link>> GetLinks();
    public async Task<string> Execute()
    {
        var links = await GetLinks();
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(serializer.LeadingTrivia);
        foreach(var link in links)
            sb.AppendLine(serializer.Serialize(link));
        sb.AppendLine(serializer.TrailingTrivia);
        return sb.ToString();
    }
}