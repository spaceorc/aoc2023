using System.Collections.Generic;
using System.Linq;

namespace aoc;

public static class V2Extensions
{
    public static Range BoundingBox(this IEnumerable<V> vectors)
    {
        var list = vectors.ToList();
        var minX = list.Min(x => x.X);
        var maxX = list.Max(x => x.X);
        var minY = list.Min(x => x.Y);
        var maxY = list.Max(x => x.Y);
        return new Range(minX, minY, maxX, maxY);
    }
}