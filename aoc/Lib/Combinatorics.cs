using System.Collections.Generic;
using System.Linq;

namespace aoc.Lib;

public static class Combinatorics
{
    /// <summary>
    ///     Variants(3, 2) => [0,0,0], [0,0,1], [0,1,0], [0,1,1], [1,0,0], [1,0,1], [1,1,0], [1,1,1]
    /// </summary>
    public static IEnumerable<int[]> Variants(int n, int v)
    {
        var arr = new int[n];
        var found = true;
        while (found)
        {
            yield return arr;
            found = false;
            for (var i = 0; i < n; i++)
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

    /// <summary>
    ///     [1,2,3].Combinations(2) => [1,2], [1,3], [2,3]
    /// </summary>
    public static IEnumerable<List<T>> Combinations<T>(this T[] items, int r)
    {
        var n = items.Length;

        if (r > n)
            yield break;

        var indices = Enumerable.Range(0, r).ToArray();

        yield return indices.Select(x => items[x]).ToList();

        while (true)
        {
            var i = indices.Length - 1;
            while (i >= 0 && indices[i] == i + n - r)
                i -= 1;

            if (i < 0)
                yield break;

            indices[i] += 1;

            for (var j = i + 1; j < r; j += 1)
                indices[j] = indices[j - 1] + 1;

            yield return indices.Select(x => items[x]).ToList();
        }
    }

    /// <summary>
    ///     Generates next permutation from current permutation of indexes array
    ///     [0,1,2] - starting permutation
    ///     Returns false if no more permutations
    /// </summary>
    public static bool NextPermutation(int[] a)
    {
        var j = a.Length - 2;
        while (j != -1 && a[j] >= a[j + 1])
            j--;
        if (j == -1)
            return false;
        var k = a.Length - 1;
        while (a[j] >= a[k])
            k--;
        (a[j], a[k]) = (a[k], a[j]);
        int l = j + 1, r = a.Length - 1;
        while (l < r)
        {
            var i = l++;
            var j1 = r--;
            (a[i], a[j1]) = (a[j1], a[i]);
        }

        return true;
    }

    /// <summary>
    ///     [1,2,3].Permutations() => [1,2,3], [1,3,2], [2,1,3], [2,3,1], [3,1,2], [3,2,1]
    /// </summary>
    public static IEnumerable<T[]> Permutations<T>(this T[] items)
    {
        var indices = Enumerable.Range(0, items.Length).ToArray();
        do
            yield return indices.Select(x => items[x]).ToArray();
        while (NextPermutation(indices));
    }
}
