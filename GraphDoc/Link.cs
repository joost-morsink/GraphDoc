namespace GraphDoc;

public record Link(Descriptor From, Descriptor To, Descriptor Label)
{
    public static Link Create(string from, string to, string label)
        => new (from, to, label);
}