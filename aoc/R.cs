using System.Collections.Generic;
using System.Linq;
using static System.Math;

namespace aoc;

public record R(long Start, long Len)
{
    public long End => Start + Len;
    public bool IsEmpty => Len <= 0;
    public R IntersectWith(R other) => FromStartEnd(Max(Start, other.Start), Min(End, other.End));
    public R Shift(long delta) => this with {Start = Start + delta};
    public static R FromStartEnd(long start, long end) => new(start, end - start);

    public IEnumerable<R> ExceptWith(IEnumerable<R> others)
    {
        var s = Start;
        foreach (var u in others.OrderBy(x => x.Start))
        {
            var part = FromStartEnd(s, u.Start);
            if (!part.IsEmpty)
                yield return part;
            s = u.End;
        }

        var lastPart = FromStartEnd(s, End);
        if (!lastPart.IsEmpty)
            yield return lastPart;
    }
}

// public static class RExtensions
// {
//     public static List<R> Pack(this IEnumerable<R> rs)
//     {
//         var ranges = rs.OrderBy(x => x.Start).ThenByDescending(x => x.End).ToList();
//
//         var current = 0;
//
//         while (current < ranges.Count)
//         {
//             var mergedCount = 0;
//             for (var i = current + 1; i < ranges.Count; i++)
//             {
//                 if (ranges[i].Intersects(ranges[current]) || ranges[i].Touches(ranges[current]))
//                 {
//                     ranges[current] = ranges[current].UnionWith(ranges[i]);
//                     mergedCount++;
//                 }
//                 else
//                     ranges[i - mergedCount] = ranges[i];
//             }
//
//             current++;
//             for (var i = 0; i < mergedCount; i++)
//                 ranges.RemoveAt(ranges.Count - 1);
//         }
//
//         return ranges;
//     }
// }
