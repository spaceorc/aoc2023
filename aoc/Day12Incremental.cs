using System;
using System.Collections.Generic;
using System.Linq;
using aoc.Lib;

namespace aoc;

public static class Day12Incremental
{
    public static void Run((string source, int[] groups)[] input)
    {
        Console.WriteLine($"Part 1: {Solve(input)}");
        Console.WriteLine($"Part 2: {Solve(ReplicateLines(input, 5))}");
    }

    /// <returns>Replicates input lines n times</returns>
    public static IEnumerable<(string source, int[] groups)> ReplicateLines(IEnumerable<(string source, int[] groups)> input, int n)
    {
        return input
            .Select(
                x => (
                    source: string.Join('?', Enumerable.Repeat(x.source, n)),
                    groups: Enumerable.Repeat(x.groups, n).SelectMany(v => v).ToArray()
                )
            );
    }

    /// <returns>Solves the problem the specific input</returns>
    public static long Solve(IEnumerable<(string source, int[] groups)> input)
    {
        return input.Sum(i => GetPlacements(i.source, i.groups));
    }

    /// <returns>How many variants to place groups in source</returns>
    public static long GetPlacements(string source, int[] groups)
    {
        var current = new DefaultDict<(int groupIndex, int used), long>
        {
            {(0, 0), 1L}
        };
        foreach (var c in source)
        {
            var next = new DefaultDict<(int groupIndex, int used), long>();
            if (c is '.' or '?')
            {
                foreach (var (key, value) in current.Where(cur => cur.Key.used == 0 || cur.Key.groupIndex >= groups.Length))
                    next[key] += value;

                foreach (var (key, value) in current.Where(cur => cur.Key.groupIndex < groups.Length && cur.Key.used == groups[cur.Key.groupIndex]))
                    next[(key.groupIndex + 1, 0)] += value;
            }

            if (c is '#' or '?')
            {
                foreach (var (key, value) in current.Where(cur => cur.Key.groupIndex < groups.Length && cur.Key.used < groups[cur.Key.groupIndex]))
                    next[(key.groupIndex, key.used + 1)] += value;
            }

            current = next;
        }

        return current[(groups.Length, 0)] + current[(groups.Length - 1, groups[^1])];
    }
}
