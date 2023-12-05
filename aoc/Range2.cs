using System.Collections.Generic;

namespace aoc;

public class Range2
{
    public Range2(long minX, long minY, long maxX, long maxY)
        : this(new V(minX, minY), new V(maxX, maxY))
    {
    }

    public Range2(V topLeft, V bottomRight)
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

    public Range2 Grow(long delta) => new Range2(TopLeft - new V(delta, delta), BottomRight + new V(delta, delta));

    public long Area => (MaxX - MinX + 1) * (MaxY - MinY + 1);
    
    public IEnumerable<V> All()
    {
        for (var y = MinY; y <= MaxY; y++)
        for (var x = MinX; x <= MaxX; x++)
            yield return new V(x, y);
    }

    public override string ToString()
    {
        return $"MinX: {MinX}, MinY: {MinY}, MaxX: {MaxX}, MaxY: {MaxY}";
    }
}