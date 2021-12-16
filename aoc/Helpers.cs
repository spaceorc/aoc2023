using System;
using System.Collections.Generic;
using System.Linq;

namespace aoc;

public static class Helpers
{
    public static IEnumerable<int[]> Variants(int n)
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
                if (arr[i] == n)
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

        if (r > n) yield break;

        int[] indices = Enumerable.Range(0, r).ToArray();

        yield return indices.Select(x => items[x]).ToList();

        while (true)
        {
            int i = indices.Length-1;
            while(i>=0 && indices[i] == i + n - r)
                i-=1;

            if(i<0) yield break;

            indices[i] += 1;

            for(int j=i+1; j<r; j+=1)
                indices[j] = indices[j-1] + 1;

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
        
    public static long Lcm(params long[] values)
    {
        return values.Aggregate(1L, Lcm);
    }

    public static long Lcm(long a, long b)
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
}