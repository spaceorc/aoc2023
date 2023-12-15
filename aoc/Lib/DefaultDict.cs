using System.Collections;
using System.Collections.Generic;

namespace aoc.Lib;

public static class DefaultDict
{
    public static DefaultDict<TKey, TValue> ToDefault<TKey, TValue>(this Dictionary<TKey, TValue> impl)
        where TKey : notnull
    {
        return new DefaultDict<TKey, TValue>(impl);
    }
}

public class DefaultDict<TKey, TValue> :
    IDictionary<TKey, TValue>,
    IReadOnlyDictionary<TKey, TValue>
    where TKey : notnull
{
    private readonly Dictionary<TKey, TValue> impl;

    public DefaultDict(Dictionary<TKey, TValue> impl)
    {
        this.impl = impl;
    }

    public DefaultDict() : this(new Dictionary<TKey, TValue>())
    {
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return impl.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)impl).GetEnumerator();
    }

    void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
    {
        ((ICollection<KeyValuePair<TKey, TValue>>)impl).Add(item);
    }

    public void Clear()
    {
        impl.Clear();
    }

    bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
    {
        return ((ICollection<KeyValuePair<TKey, TValue>>)impl).Contains(item);
    }

    void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        ((ICollection<KeyValuePair<TKey, TValue>>)impl).CopyTo(array, arrayIndex);
    }

    bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
    {
        return ((ICollection<KeyValuePair<TKey, TValue>>)impl).Remove(item);
    }

    public int Count => impl.Count;

    bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => ((ICollection<KeyValuePair<TKey, TValue>>)impl).IsReadOnly;

    public void Add(TKey key, TValue value)
    {
        impl.Add(key, value);
    }

    public bool ContainsKey(TKey key)
    {
        return impl.ContainsKey(key);
    }

    public bool Remove(TKey key)
    {
        return impl.Remove(key);
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        return impl.TryGetValue(key, out value!);
    }

    public TValue this[TKey key]
    {
        get => impl.TryGetValue(key, out var v) ? v : default!;
        set => impl[key] = value;
    }

    public ICollection<TKey> Keys => impl.Keys;

    public ICollection<TValue> Values => impl.Values;

    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => impl.Keys;

    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => impl.Values;
}
