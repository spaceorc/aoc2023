using System;
using System.Collections.Generic;

namespace aoc.Lib;

public static class ArrayComparer
{
    public static ArrayComparer<T> Create<T>() => new();
}

public class ArrayComparer<T> : IComparer<T[]>
{
    private readonly IComparer<T> elementComparer;

    public ArrayComparer(IComparer<T>? elementComparer = null)
    {
        this.elementComparer = elementComparer ?? Comparer<T>.Default;
    }

    public int Compare(T[]? x, T[]? y)
    {
        if (x == null && y == null)
            return 0;
        if (x == null)
            return -1;
        if (y == null)
            return 1;
        for (var i = 0; i < Math.Min(x.Length, y.Length); i++)
        {
            var res = elementComparer.Compare(x[i], y[i]);
            if (res != 0)
                return res;
        }

        if (x.Length < y.Length)
            return -1;
        if (x.Length > y.Length)
            return 1;
        return 0;
    }
}
