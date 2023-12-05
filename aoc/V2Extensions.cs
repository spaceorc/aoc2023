using System.Collections.Generic;
using System.Linq;

namespace aoc;

public static class V2Extensions
{
    public static Range2 BoundingBox(this IEnumerable<V> vectors)
    {
        var list = vectors.ToList();
        var minX = list.Min(x => x.X);
        var maxX = list.Max(x => x.X);
        var minY = list.Min(x => x.Y);
        var maxY = list.Max(x => x.Y);
        return new Range2(minX, minY, maxX, maxY);
    }
}