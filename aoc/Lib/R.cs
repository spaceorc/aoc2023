using System.Collections.Generic;
using System.Linq;
using static System.Math;

namespace aoc;

public record R(long Start, long Len)
{
    public long End => Start + Len;
    public bool IsEmpty => Len <= 0;
    public bool Touches(R other) => End == other.Start || Start == other.End;
    public bool Intersects(R other) => !IntersectWith(other).IsEmpty;
    public R IntersectWith(R other) => FromStartEnd(Max(Start, other.Start), Min(End, other.End));
    public R MakeGreaterThan(long value) => FromStartEnd(Max(Start, value + 1), End);
    public R MakeLessThan(long value) => FromStartEnd(Start, Min(End, value));
    public R MakeGreaterThanOrEqualTo(long value) => FromStartEnd(Max(Start, value), End);
    public R MakeLessThanOrEqualTo(long value) => FromStartEnd(Start, Min(End, value + 1));
    public R UnionWith(R other) => FromStartEnd(Min(Start, other.Start), Max(End, other.End));
    public R Shift(long delta) => this with { Start = Start + delta };
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
