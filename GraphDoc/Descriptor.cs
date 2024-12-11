namespace GraphDoc;

public record Descriptor(string Name, string? Description = null) : IEquatable<Descriptor>, IComparable<Descriptor>
{
    public static implicit operator Descriptor(string name) => new (name);

    public virtual bool Equals(Descriptor? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase);
    }

    public override int GetHashCode()
        => StringComparer.OrdinalIgnoreCase.GetHashCode(Name);
    

    public int CompareTo(Descriptor? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (other is null) return 1;
        return string.Compare(Name, other.Name, StringComparison.OrdinalIgnoreCase);
    }
}