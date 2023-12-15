using System;
using System.Collections.Generic;
using System.Linq;

namespace aoc.Lib;

public static class VHelpers
{
    public static Square BoundingBox(this IEnumerable<V> vectors)
    {
        var list = vectors.ToList();
        var minX = list.Min(x => x.X);
        var maxX = list.Max(x => x.X);
        var minY = list.Min(x => x.Y);
        var maxY = list.Max(x => x.Y);
        return new Square(minX, minY, maxX, maxY);
    }

    public static IEnumerable<V> MakeLine(V a, V b)
    {
        var d = b - a;
        if (d == new V(0, 0))
        {
            yield return a;
            yield break;
        }

        var gcd = MathHelpers.Gcd(Math.Abs(d.X), Math.Abs(d.Y));
        d /= gcd;

        for (var v = a; v != b; v += d)
            yield return v;
        yield return b;
    }
}
