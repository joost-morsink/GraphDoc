using System.Collections.Immutable;

namespace GraphDoc;

public interface IStructureSerializer
{
    string Serialize(Structure structure);

    public static IStructureSerializer SimpleMermaid { get; } = new ByGraphImpl(IGraphSerializer.SimpleMermaid);
    public static IStructureSerializer Mermaid { get; } = new ByGraphImpl(IGraphSerializer.Mermaid);
    private static readonly ImmutableDictionary<string, Func<IStructureSerializer>> Serializers
        = ImmutableDictionary.Create<string, Func<IStructureSerializer>>(StringComparer.OrdinalIgnoreCase)
            .Add("simpleMermaid", () => SimpleMermaid)
            .Add("mermaid", () => Mermaid);

    public static IStructureSerializer Create(string? str)
        => str is not null && Serializers.TryGetValue(str, out var serializerCreator)
            ? serializerCreator()
            : SimpleMermaid;

    private class ByGraphImpl(IGraphSerializer graphSerializer) : IStructureSerializer
    {
        public string Serialize(Structure structure)
            => $@"{graphSerializer.LeadingTrivia}
{string.Join(Environment.NewLine, structure.Select(graphSerializer.Serialize))}
{graphSerializer.TrailingTrivia}";
}