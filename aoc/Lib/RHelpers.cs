using System.Collections.Generic;
using System.Linq;

namespace aoc;

public static class RHelpers
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
                if (ranges[i].Intersects(ranges[current]) || ranges[i].Touches(ranges[current]))
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
