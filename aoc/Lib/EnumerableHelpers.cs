using System;
using System.Collections.Generic;
using System.Linq;

namespace aoc.Lib;

public static class EnumerableHelpers
{
    public static IEnumerable<T> Generate<T>(this T start, Func<T, T> next)
    {
        while (true)
        {
            yield return start;
            start = next(start);
        }
        // ReSharper disable once IteratorNeverReturns
    }

    public static IEnumerable<T[]> SlidingWindow<T>(this IEnumerable<T> items, int windowSize)
    {
        var queue = new Queue<T>(windowSize);
        foreach (var item in items)
        {
            if (queue.Count == windowSize)
                queue.Dequeue();

            queue.Enqueue(item);

            if (queue.Count == windowSize)
                yield return queue.ToArray();
        }
    }

    public static IEnumerable<T> TakeEvery<T>(this IEnumerable<T> items, int n, int startFrom = 0)
    {
        return items.Skip(startFrom).Chunk(n).Select(x => x.First());
    }

    public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> items)
    {
        return items.Select((v, i) => (v, i));
    }

    public static IEnumerable<T> Without<T>(this IEnumerable<T> items, params T[] values)
    {
        return items.Where(x => !values.Contains(x));
    }

    public static long Product(this IEnumerable<long> items)
    {
        return items.Aggregate(1L, (a, b) => a * b);
    }

    public static int Product(this IEnumerable<int> items)
    {
        return items.Aggregate(1, (a, b) => a * b);
    }

    public static IEnumerable<T> TakeUntil<T>(this IEnumerable<T> items, Func<T, bool> predicate)
    {
        foreach (var item in items)
        {
            yield return item;
            if (predicate(item))
                break;
        }
    }
}
