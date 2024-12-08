namespace GraphDoc;

public record Descriptor(string Name, string? Description = null)
{
    public static implicit operator Descriptor(string name) => new (name);
}