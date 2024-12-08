using System.Collections.Immutable;

namespace GraphDoc;

internal static class Ext
{
    public static ImmutableDictionary<string,Type> Add<T>(this ImmutableDictionary<string,Type> dict)
        where T : ICommandDef
        => dict.Add(T.Name, typeof(T));
    public static ImmutableDictionary<string,V> ToCaseInsensitiveDictionary<V>(this IEnumerable<KeyValuePair<string,V>> pairs)
        => pairs.ToImmutableDictionary(p => p.Key, p => p.Value, StringComparer.OrdinalIgnoreCase);
    public static V GetRequired<V>(this IReadOnlyDictionary<string, V> dict, string key, params string[] keys)
    {
        foreach (var k in keys.Prepend(key))
            if (dict.TryGetValue(k, out var value))
                return value;
        throw new ArgumentException($"Missing {key} argument.");
    }
    public static V? GetOptional<V>(this IReadOnlyDictionary<string, V> dict, string key, params string[] keys)
    {
        foreach (var k in keys.Prepend(key))
            if (dict.TryGetValue(k, out var value))
                return value;
        return default;
    }
}