using System.Collections.Generic;

namespace aoc;

public class Range
{
    public Range(int minX, int minY, int maxX, int maxY)
        : this(new V(minX, minY), new V(maxX, maxY))
    {
    }

    public Range(V topLeft, V bottomRight)
    {
        TopLeft = topLeft;
        BottomRight = bottomRight;
    }

    public V TopLeft { get; }
    public V BottomRight { get; }
    public long MinX => TopLeft.X;
    public long MinY => TopLeft.Y;
    public long MaxX => BottomRight.X;
    public long MaxY => BottomRight.Y;

    public Range Grow(long delta) => new Range(TopLeft - new V(delta, delta), BottomRight + new V(delta, delta));

    public IEnumerable<V> All()
    {
        for (var y = MinY; y <= MaxY; y++)
        for (var x = MinX; x <= MaxX; x++)
            yield return new V(x, y);
    }
}