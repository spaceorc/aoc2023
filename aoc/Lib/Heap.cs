using System;
using System.Collections.Generic;

namespace aoc.Lib;

public class Heap<T>
{
    private readonly IComparer<T> comparer;
    private readonly List<T> values = new();

    public Heap(IComparer<T>? comparer = null)
    {
        this.comparer = comparer ?? Comparer<T>.Default;
    }

    public bool IsEmpty => Count == 0;

    public int Count => values.Count;

    public T Min
    {
        get
        {
            if (IsEmpty)
                throw new InvalidOperationException("Heap is empty");
            return values[0];
        }
    }

    public void Add(T value)
    {
        values.Add(value);
        var c = values.Count - 1;
        while (c > 0)
        {
            var p = (c - 1) / 2;
            if (comparer.Compare(values[p], values[c]) <= 0)
                break;
            (values[p], values[c]) = (values[c], values[p]);
            c = p;
        }
    }

    public T DeleteMin()
    {
        if (IsEmpty)
            throw new InvalidOperationException("Heap is empty");
        var res = values[0];
        values[0] = values[^1];
        values.RemoveAt(values.Count - 1);

        var p = 0;
        while (true)
        {
            var c1 = p * 2 + 1;
            var c2 = p * 2 + 2;
            if (c1 >= values.Count)
                break;
            if (c2 >= values.Count)
            {
                if (comparer.Compare(values[p], values[c1]) > 0)
                    (values[p], values[c1]) = (values[c1], values[p]);

                break;
            }

            if (comparer.Compare(values[p], values[c1]) <= 0 && comparer.Compare(values[p], values[c2]) <= 0)
                break;
            if (comparer.Compare(values[p], values[c1]) <= 0)
            {
                (values[p], values[c2]) = (values[c2], values[p]);
                p = c2;
            }
            else if (comparer.Compare(values[p], values[c2]) <= 0)
            {
                (values[p], values[c1]) = (values[c1], values[p]);
                p = c1;
            }
            else if (comparer.Compare(values[c1], values[c2]) <= 0)
            {
                (values[p], values[c1]) = (values[c1], values[p]);
                p = c1;
            }
            else
            {
                (values[p], values[c2]) = (values[c2], values[p]);
                p = c2;
            }
        }

        return res;
    }
}
