using System.Collections;
using System.Collections.Immutable;

namespace GraphDoc;

public abstract class Structure : IEnumerable<Link>
{
    private Structure(Descriptor description)
    {
        Description = description;
    }

    public Descriptor Description { get; }

    public sealed class Dict(Descriptor description, IEnumerable<KeyValuePair<Descriptor, Structure>>members) : Structure(description)
    {
        public IImmutableDictionary<Descriptor, Structure> Members { get; } = members.ToImmutableSortedDictionary();
    }

    public sealed class Leaf(Descriptor description) : Structure(description);


    public IEnumerator<Link> GetEnumerator()
    {
        if (this is Dict dict)
        {
            foreach (var (key, value) in dict.Members)
            {
                yield return new (Description, value.Description, value is Leaf ? "": key);
                foreach (var link in value)
                    yield return link;
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator() 
        => GetEnumerator();
}