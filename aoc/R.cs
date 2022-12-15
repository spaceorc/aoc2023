using System.Collections.Generic;
using System.Linq;
using static System.Math;

namespace aoc;

public record R(long Start, long End)
{
    public bool Overlaps(R other) => Start <= other.End && End >= other.Start;
    public bool Touches(R other) => Start == other.End + 1 || End == other.Start - 1;
    public bool Contains(R other) => Start <= other.Start && End >= other.End;

    public R IntersectWith(R other) => new(Max(Start, other.Start), Min(End, other.End));
    public R UnionWith(R other) => new(Min(Start, other.Start), Max(End, other.End));

    public long Length => End - Start + 1;
}

public static class RExtensions
{
    public static List<R> Pack(this IEnumerable<R> rs)
    {
        var ranges = rs.OrderBy(x => x.Start).ThenByDescending(x => x.End).ToList();

        var current = 0;

        while (current < ranges.Count)
        {
            var mergedCount = 0;
            for (var i = current + 1; i < ranges.Count; i++)
            {
                if (ranges[i].Overlaps(ranges[current]) || ranges[i].Touches(ranges[current]))
                {
                    ranges[current] = ranges[current].UnionWith(ranges[i]);
                    mergedCount++;
                }
                else
                    ranges[i - mergedCount] = ranges[i];
            }

            current++;
            for (var i = 0; i < mergedCount; i++)
                ranges.RemoveAt(ranges.Count - 1);
        }

        return ranges;
    }
}