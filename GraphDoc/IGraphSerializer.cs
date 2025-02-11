using System.Collections.Immutable;

namespace GraphDoc;

public interface IGraphSerializer
{
    string LeadingTrivia { get; }
    string TrailingTrivia { get; }
    string Serialize(Link link);
    
    private static readonly ImmutableDictionary<string, Func<IGraphSerializer>> Serializers
        = ImmutableDictionary.Create<string, Func<IGraphSerializer>>(StringComparer.OrdinalIgnoreCase)
            .Add("simpleMermaid", () => SimpleMermaid)
            .Add("mermaid", () => Mermaid);

    public static IGraphSerializer Create(string? str)
        => str is not null && Serializers.TryGetValue(str, out var serializerCreator)
            ? serializerCreator()
            : SimpleMermaid;

    public static IGraphSerializer SimpleMermaid => new SimpleMermaidImpl();
    public static IGraphSerializer Mermaid => new MermaidImpl();

    private class MermaidImpl : IGraphSerializer
    {
        public string LeadingTrivia => "graph LR";
        public string TrailingTrivia => "";
        public string Serialize(Link link) => $"    {Component(link.From)} -->{Link(link.Label)} {Component(link.To)}";

        private string Component(Descriptor descriptor)
            => descriptor.Description is null
                ? descriptor.Name
                : $"{ComponentName(descriptor)}[\"{Escape(descriptor.Name)}: {Escape(descriptor.Description)}\"]";
    }

    private class SimpleMermaidImpl : IGraphSerializer
    {
        public string LeadingTrivia => "graph LR";
        public string TrailingTrivia => "";

        public string Serialize(Link link)
        {
            var fr = ComponentName(link.From);
            var to = ComponentName(link.To);
            var frl = fr == link.From.Name ? "" : $"[\"{Escape(link.From.Name)}\"]";
            var tol = to == link.To.Name ? "" : $"[\"{Escape(link.To.Name)}\"]";

            return $"    {fr}{frl} -->{Link(link.Label, true)} {to}{tol}";
        }
    }

    private static string ComponentName(Descriptor descriptor)
        => descriptor.Name.Replace(' ', '_')
            .Replace('<', '_')
            .Replace('>', '_')
            .Replace(',', '_')
            .Replace('`', '_');

    private static string Link(Descriptor descriptor, bool @short = false)
    {
        var result = Inner();
        return string.IsNullOrWhiteSpace(result)
            ? ""
            : $"|\"{Escape(result)}\"|";

        string Inner()
            => @short || descriptor.Description is null
                ? descriptor.Name
                : $"{descriptor.Name}: {descriptor.Description}";
    }

    private static string Escape(string escape)
        => escape.Replace("<", "&lt;");
}