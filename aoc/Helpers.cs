using System;
using System.Collections.Generic;
using System.Linq;

namespace aoc;

public static class Helpers
{
    public static IEnumerable<int[]> Variants(int n, int v)
    {
        var arr = new int[n];
        var found = true;
        while (found)
        {
            yield return arr;
            found = false;
            for (int i = 0; i < n; i++)
            {
                arr[i]++;
                if (arr[i] == v)
                {
                    arr[i] = 0;
                    continue;
                }

                found = true;
                break;
            }
        }
    }

    public static IEnumerable<List<T>> Combinations<T>(this T[] items, int r)
    {
        int n = items.Length;

        if (r > n)
            yield break;

        int[] indices = Enumerable.Range(0, r).ToArray();

        yield return indices.Select(x => items[x]).ToList();

        while (true)
        {
            int i = indices.Length - 1;
            while (i >= 0 && indices[i] == i + n - r)
                i -= 1;

            if (i < 0)
                yield break;

            indices[i] += 1;

            for (int j = i + 1; j < r; j += 1)
                indices[j] = indices[j - 1] + 1;

            yield return indices.Select(x => items[x]).ToList();
        }
    }

    public static bool NextPermutation(int[] a)
    {
        int j = a.Length - 2;
        while (j != -1 && a[j] >= a[j + 1])
            j--;
        if (j == -1)
            return false;
        int k = a.Length - 1;
        while (a[j] >= a[k])
            k--;
        (a[j], a[k]) = (a[k], a[j]);
        int l = j + 1, r = a.Length - 1;
        while (l < r)
        {
            int i = l++;
            int j1 = r--;
            (a[i], a[j1]) = (a[j1], a[i]);
        }

        return true;
    }

    public static IEnumerable<T[]> Permutations<T>(this T[] items)
    {
        int[] indices = Enumerable.Range(0, items.Length).ToArray();
        do
        {
            yield return indices.Select(x => items[x]).ToArray();
        } while (NextPermutation(indices));
    }

    public static IEnumerable<T[]> Batch<T>(this IEnumerable<T> items, int batchSize)
    {
        var batch = new List<T>();
        foreach (var item in items)
        {
            batch.Add(item);
            if (batch.Count == batchSize)
            {
                yield return batch.ToArray();
                batch.Clear();
            }
        }

        if (batch.Count > 0)
            yield return batch.ToArray();
    }

    public static IEnumerable<T> TakeEvery<T>(this IEnumerable<T> items, int n, int startFrom = 0)
    {
        return items.Skip(startFrom).Batch(n).Select(x => x.First());
    }

    public static IEnumerable<(T, int)> WithIndex<T>(this IEnumerable<T> items)
    {
        return items.Select((v, i) => (v, i));
    }

    public static string[] Columns(this IEnumerable<string> lines)
    {
        var result = new List<string>();
        var linesArr = lines as string[] ?? lines.ToArray();
        for (int i = 0; i < linesArr[0].Length; i++)
            result.Add(new string(linesArr.Select(x => x[i]).ToArray()));

        return result.ToArray();
    }

    public static string[] RotateCW(this IEnumerable<string> lines)
    {
        return lines.Reverse().Columns();
    }

    public static string[] RotateCCW(this IEnumerable<string> lines, int count = 1)
    {
        return lines.RotateCW(3 * count);
    }

    public static string[] RotateCW(this IEnumerable<string> lines, int count)
    {
        var result = lines;
        for (int i = 0; i < count % 4; i++)
        {
            result = result.RotateCW();
        }

        return result.ToArray();
    }

    public static List<string[]> Regions(this IEnumerable<string> lines)
    {
        var result = new List<string[]>();
        var cur = new List<string>();
        foreach (var line in lines)
        {
            if (line != "")
                cur.Add(line);
            else if (cur.Count > 0)
            {
                result.Add(cur.ToArray());
                cur.Clear();
            }
        }

        if (cur.Count > 0)
            result.Add(cur.ToArray());

        return result;
    }

    public static Map<T> ToMap<T>(this IEnumerable<string> lines)
    {
        return lines.ToMap(c =>
        {
            if (typeof(T) == typeof(char))
                return (T)(object)c;
            if (typeof(T) == typeof(int))
                return (T)(object)(c - '0');
            if (typeof(T) == typeof(long))
                return (T)(object)(long)(c - '0');
            throw new InvalidOperationException($"Unsupported type {typeof(T)}");
        });
    }

    public static Map<T> ToMap<T>(this IEnumerable<string> lines, Func<char, T> selector)
    {
        var linesArr = lines as IList<string> ?? lines.ToArray();
        var result = new Map<T>(linesArr[0].Length, linesArr.Count);
        for (int y = 0; y < result.sizeY; y++)
        for (int x = 0; x < result.sizeX; x++)
        {
            result[new V(x, y)] = selector(linesArr[y][x]);
        }

        return result;
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

    public static long Lcm(params long[] values)
    {
        return values.Aggregate(1L, Lcm);
    }

    public static long Lcm(long a, long b)
    {
        return a / Gcd(a, b) * b;
    }

    public static int Lcm(params int[] values)
    {
        return values.Aggregate(1, Lcm);
    }

    public static int Lcm(int a, int b)
    {
        return a / Gcd(a, b) * b;
    }

    public static long Gcd(long a, long b)
    {
        while (a != 0 && b != 0)
        {
            if (a > b)
                a %= b;
            else
                b %= a;
        }

        return a + b;
    }

    public static int Gcd(int a, int b)
    {
        while (a != 0 && b != 0)
        {
            if (a > b)
                a %= b;
            else
                b %= a;
        }

        return a + b;
    }

    public static IEnumerable<V> MakeLine(V a, V b)
    {
        var d = b - a;
        if (d == new V(0, 0))
        {
            yield return a;
            yield break;
        }

        var gcd = Gcd(Math.Abs(d.X), Math.Abs(d.Y));
        d /= gcd;

        for (var v = a; v != b; v += d)
            yield return v;
        yield return b;
    }

    public static T Out<T>(this T value, string prefix)
    {
        var valueString = Convert.ToString(value);
        Console.WriteLine($"{prefix}{valueString}");
        return value;
    }

    public static IEnumerable<BfsPathItem<TState>> Bfs<TState>(
        IEnumerable<TState> startFrom,
        Func<TState, IEnumerable<TState>> getNextStates,
        int maxDistance = int.MaxValue)
        where TState : notnull
    {
        var queue = new Queue<TState>();
        var used = new Dictionary<TState, BfsPathItem<TState>>();
        foreach (var state in startFrom)
        {
            queue.Enqueue(state);
            used.Add(state, new BfsPathItem<TState>(state, 0, null));
        }

        while (queue.Count > 0)
        {
            var cur = queue.Dequeue();
            var curBfsState = used[cur];
            yield return curBfsState;

            if (curBfsState.Distance >= maxDistance)
                continue;

            foreach (var next in getNextStates(cur))
            {
                if (used.ContainsKey(next))
                    continue;
                used.Add(next, new BfsPathItem<TState>(next, curBfsState.Distance + 1, curBfsState));
                queue.Enqueue(next);
            }
        }
    }
}