using System;
using System.Collections.Generic;

namespace aoc;

public class Heap
{
    private readonly List<ulong> values = new();

    public bool IsEmpty => Count == 0;

    public int Count => values.Count;

    public ulong Min
    {
        get
        {
            if (IsEmpty)
                throw new InvalidOperationException($"{nameof(Heap)} is empty");
            return values[0];
        }
    }

    public void Add(ulong value)
    {
        values.Add(value);
        var c = values.Count - 1;
        while (c > 0)
        {
            var p = (c - 1) / 2;
            if (values[p] <= values[c])
                break;
            (values[p], values[c]) = (values[c], values[p]);
            c = p;
        }
    }

    public ulong DeleteMin()
    {
        if (IsEmpty)
            throw new InvalidOperationException($"{nameof(Heap)} is empty");
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
                if (values[p] > values[c1])
                {
                    (values[p], values[c1]) = (values[c1], values[p]);
                }

                break;
            }

            if (values[p] <= values[c1] && values[p] <= values[c2])
                break;
            if (values[p] <= values[c1])
            {
                (values[p], values[c2]) = (values[c2], values[p]);
                p = c2;
            }
            else if (values[p] <= values[c2])
            {
                (values[p], values[c1]) = (values[c1], values[p]);
                p = c1;
            }
            else if (values[c1] <= values[c2])
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