using System.Collections.Generic;
using System.Linq;

namespace aoc;

public static class V3Extensions
{
    public static Range3 BoundingBox(this IEnumerable<V3> vectors)
    {
        var list = vectors.ToList();
        var minX = list.Min(x => x.X);
        var maxX = list.Max(x => x.X);
        var minY = list.Min(x => x.Y);
        var maxY = list.Max(x => x.Y);
        var minZ = list.Min(x => x.Z);
        var maxZ = list.Max(x => x.Z);
        return new Range3(minX, minY, minZ, maxX, maxY, maxZ);
    }
}