using System;
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

    public V BottomRight => new(sizeX - 1, sizeY - 1);

    public Range2 Range() => new Range2(V.Zero, BottomRight);

    public IEnumerable<V> Area4(V v)
    {
        return V.area4.Select(dv => v + dv).Where(Inside);
    }

    public IEnumerable<V> Area5(V v)
    {
        return V.area5.Select(dv => v + dv).Where(Inside);
    }

    public IEnumerable<V> Area8(V v)
    {
        return V.area8.Select(dv => v + dv).Where(Inside);
    }

    public IEnumerable<V> Column(long x)
    {
        for (int y = 0; y < sizeY; y++)
            yield return new V(x, y);
    }

    public IEnumerable<T> ColumnValues(long x)
    {
        return Column(x).Select(v => this[v]);
    }

    public string ColumnString(long x, string separator = "")
    {
        return string.Join(separator, ColumnValues(x));
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
    
    public IEnumerable<T> RowValues(long y)
    {
        return Row(y).Select(v => this[v]);
    }

    public string RowString(long y, string separator = "")
    {
        return string.Join(separator, RowValues(y));
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

    public IDisposable ChangeAt(V v, T newValue)
    {
        var original = this[v];
        this[v] = newValue;
        return new DisposableAction(() => this[v] = original);
    }

    public IDisposable ChangeAt(V v, Func<T, T> getNewValue)
    {
        return ChangeAt(v, getNewValue(this[v]));
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

    public IEnumerable<BfsPathItem<V>> Bfs(
        IEnumerable<V> startFrom,
        Func<V, IEnumerable<V>> nexts,
        Func<T, T, bool> acceptNext)
    {
        return Helpers.Bfs(startFrom, nexts);
    }

    public IEnumerable<BfsPathItem<V>> Bfs(
        V startFrom,
        Func<V, IEnumerable<V>> nexts,
        Func<T, T, bool> acceptNext)
    {
        return Bfs(new[] { startFrom }, nexts, acceptNext);
    }

    public IEnumerable<BfsPathItem<V>> Bfs4(
        IEnumerable<V> startFrom,
        Func<T, T, bool> acceptNext)
    {
        return Bfs(startFrom, Area4, acceptNext);
    }

    public IEnumerable<BfsPathItem<V>> Bfs4(
        V startFrom,
        Func<T, T, bool> acceptNext)
    {
        return Bfs(new[] { startFrom }, Area4, acceptNext);
    }

    public IEnumerable<BfsPathItem<V>> Bfs8(
        IEnumerable<V> startFrom,
        Func<T, T, bool> acceptNext)
    {
        return Bfs(startFrom, Area8, acceptNext);
    }

    public IEnumerable<BfsPathItem<V>> Bfs8(
        V startFrom,
        Func<T, T, bool> acceptNext)
    {
        return Bfs(new[] { startFrom }, Area8, acceptNext);
    }
    
    public static Map<T> Parse(string s)
    {
        return s.Split('\n').ToMap<T>();
    }
}