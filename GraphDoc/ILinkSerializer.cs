using System.Collections.Immutable;

namespace GraphDoc;

public interface ILinkSerializer
{
    string LeadingTrivia { get; }
    string TrailingTrivia { get; }
    string Serialize(Link link);
    private static readonly ImmutableDictionary<string, Func<ILinkSerializer>> Serializers 
            = ImmutableDictionary.Create<string, Func<ILinkSerializer>>(StringComparer.OrdinalIgnoreCase)
                .Add("simpleMermaid", () => SimpleMermaid)
                .Add("mermaid", () => Mermaid);

    public static ILinkSerializer Create(string? str)
        => str is not null && Serializers.TryGetValue(str, out var serializerCreator)
            ? serializerCreator()
            : SimpleMermaid;
    public static ILinkSerializer SimpleMermaid => new SimpleMermaidImpl();
    public static ILinkSerializer Mermaid => new MermaidImpl();
    
    private class MermaidImpl : ILinkSerializer
    {
        public string LeadingTrivia => "graph TD";
        public string TrailingTrivia => "";
        public string Serialize(Link link) => $"    {Component(link.From)} -->|{Link(link.Label)}| {Component(link.To)}";
        private string Component(Descriptor descriptor)
            => descriptor.Description is null
                ? descriptor.Name
                : $"{descriptor.Name}[\"{descriptor.Name}: {descriptor.Description}\"]";

        private string Link(Descriptor descriptor)
            => descriptor.Description is null 
                ? descriptor.Name
                : $"\"{descriptor.Name}: {descriptor.Description}\"";  
    }

    private class SimpleMermaidImpl : ILinkSerializer
    {
        public string LeadingTrivia => "graph TD";
        public string TrailingTrivia => "";
        public string Serialize(Link link) => $"    {link.From.Name} -->|{link.Label.Name}| {link.To.Name}";

    }
}