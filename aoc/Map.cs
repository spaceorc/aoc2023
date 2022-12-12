using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace aoc;

public class Map<T>
{
    public readonly int sizeX;
    public readonly int sizeY;
    public readonly int totalCount;
    public readonly T[] data;

    public Map(int size)
        : this(size, size)
    {
    }

    public Map(int sizeX, int sizeY)
    {
        this.sizeX = sizeX;
        this.sizeY = sizeY;
        totalCount = sizeX * sizeY;
        data = new T[totalCount];
    }

    public T this[V v]
    {
        get => data[v.Y * sizeX + v.X];
        set => data[v.Y * sizeX + v.X] = value;
    }

    public T this[int index]
    {
        get => data[index];
        set => data[index] = value;
    }

    public void Clear()
    {
        Array.Fill(data, default);
    }

    public void Fill(T value)
    {
        Array.Fill(data, value);
    }

    public bool Inside(V v) => v.X >= 0 && v.Y >= 0 && v.X < sizeX && v.Y < sizeY;

    public IEnumerable<V> All()
    {
        for (int y = 0; y < sizeY; y++)
        for (int x = 0; x < sizeX; x++)
        {
            yield return new V(x, y);
        }
    }

    public IEnumerable<V> AllButBorder()
    {
        for (int y = 1; y < sizeY - 1; y++)
        for (int x = 1; x < sizeX - 1; x++)
        {
            yield return new V(x, y);
        }
    }

    public V BottomRight => new (sizeX - 1, sizeY - 1);

    public IEnumerable<V> Nears(V v)
    {
        return V.nears.Select(dv => v + dv).Where(Inside);
    }
        
    public IEnumerable<V> Nears8(V v)
    {
        return V.nears8.Select(dv => v + dv).Where(Inside);
    }
    
    public IEnumerable<V> Column(long x)
    {
        for (int y = 0; y < sizeY; y++)
            yield return new V(x, y);
    }
    
    public IEnumerable<V[]> Columns()
    {
        for (int x = 0; x < sizeX; x++)
            yield return Column(x).ToArray();
    }
    
    public IEnumerable<V> Row(long y)
    {
        for (int x = 0; x < sizeX; x++)
            yield return new V(x, y);
    }
    
    public IEnumerable<V[]> Rows()
    {
        for (int y = 0; y < sizeY; y++)
            yield return Row(y).ToArray();
    }

    public IEnumerable<T> ValuesAt(IEnumerable<V> vs)
    {
        return vs.Select(v => this[v]);
    }

    public Map<T> Clone()
    {
        var clone = new Map<T>(sizeX, sizeY);
        Array.Copy(data, clone.data, totalCount);
        return clone;
    }
        
    public void CopyTo(Map<T> other)
    {
        Array.Copy(data, other.data, totalCount);
    }

    public IEnumerable<V>? FindPath(
        Func<V, bool> startAt,
        Func<V, bool> endAt,
        Func<V, IEnumerable<V>> nexts,
        Func<T, T, bool> acceptNext)
    {
        var queue = new Queue<V>();
        var used = new Dictionary<V, V?>();
        foreach (var v in All().Where(startAt))
        {
            queue.Enqueue(v);
            used[v] = null;
        }

        while (queue.Count > 0)
        {
            var cur = queue.Dequeue();
            if (endAt(cur))
            {
                var result = new List<V>();
                for (V? c = cur; c != null; c = used[c.Value])
                {
                    result.Add(c.Value);
                }
                result.Reverse();
                return result;
            }

            foreach (var next in nexts(cur))
            {
                if (acceptNext(this[cur], this[next]))
                    continue;

                if (used.ContainsKey(next))
                    continue;

                used[next] = cur;
                queue.Enqueue(next);
            }
        }

        return null;
    }

    public IEnumerable<V>? FindPath4(
        Func<V, bool> startAt,
        Func<V, bool> endAt,
        Func<T, T, bool> acceptNext)
    {
        return FindPath(startAt, endAt, Nears, acceptNext);
    }

    public IEnumerable<V>? FindPath8(
        Func<V, bool> startAt,
        Func<V, bool> endAt,
        Func<T, T, bool> acceptNext)
    {
        return FindPath(startAt, endAt, Nears8, acceptNext);
    }
}