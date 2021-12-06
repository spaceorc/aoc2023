using System;
using System.Collections.Generic;
using System.Linq;

namespace aoc
{
    public static class Helpers
    {
        public static V GetOrDefault<K, V>(this Dictionary<K, V> dict, K key, V defaultValue = default)
        {
            if (dict.TryGetValue(key, out var result))
                return result;
            return defaultValue;
        }

        public static IEnumerable<int[]> Iterate(int n, int inputSize)
        {
            var connections = new int[n];
            for (int i = 0; i < n; i++)
                connections[i] = -inputSize;

            var found = true;
            while (found)
            {
                yield return connections;
                found = false;
                for (int i = 0; i < n; i++)
                {
                    connections[i]++;
                    if (connections[i] == n)
                    {
                        connections[i] = -inputSize;
                        continue;
                    }

                    found = true;
                    break;
                }
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
}