using System;
using System.Collections.Generic;
using System.Linq;

namespace aoc;

public static class Day12Recursive
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
        return GetPlacements(source, source.Length, groups, groups.Length, new());
    }

    /// <returns>How many variants to place groups[..count] in source[..len]</returns>
    private static long GetPlacements(string source, int len, int[] groups, int count, Dictionary<(int, int), long> cache)
    {
        var cacheKey = (len, count);
        if (cache.TryGetValue(cacheKey, out var result))
            return result;

        if (len <= 0)
            return count == 0 ? 1 : 0;

        switch (source[len - 1])
        {
            case '.':
                result = GetPlacements(source, len - 1, groups, count, cache);
                break;

            case '#':
                result = count != 0 && CanPlaceAtTheEnd(source, len, groups[count - 1])
                    ? GetPlacements(source, len - (groups[count - 1] + 1), groups, count - 1, cache)
                    : 0;
                break;

            case '?':
                // Calculate variants if '?' is '.'
                result = GetPlacements(source, len - 1, groups, count, cache);

                // Add variants if '?' is '#' (and group can be placed)
                if (count > 0 && CanPlaceAtTheEnd(source, len, groups[count - 1]))
                    result += GetPlacements(source, len - (groups[count - 1] + 1), groups, count - 1, cache);

                break;

            default:
                throw new Exception($"Bad char: {source[len - 1]}");
        }

        cache[cacheKey] = result;
        return result;
    }

    /// <returns>True if group of size groupSize can be placed at the end of source[..len]</returns>
    private static bool CanPlaceAtTheEnd(string source, int len, int groupSize)
    {
        if (groupSize > len)
            return false;

        return Enumerable.Range(len - groupSize, groupSize).All(i => source[i] is '#' or '?')
               && (len == groupSize || source[len - groupSize - 1] is '.' or '?');
    }
}