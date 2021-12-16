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

    public V BottomRight => new (sizeX - 1, sizeY - 1);

    public IEnumerable<V> Nears(V v)
    {
        return V.nears.Select(dv => v + dv).Where(Inside);
    }
        
    public IEnumerable<V> Nears8(V v)
    {
        return V.nears8.Select(dv => v + dv).Where(Inside);
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
}