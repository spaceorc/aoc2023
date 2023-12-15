using System;
using System.Collections.Generic;

namespace aoc.Lib;

public class Square : IEquatable<Square>
{
    public Square(long minX, long minY, long maxX, long maxY)
        : this(new V(minX, minY), new V(maxX, maxY))
    {
    }

    public Square(V topLeft, V bottomRight)
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

    public long Area => (MaxX - MinX + 1) * (MaxY - MinY + 1);

    public bool Equals(Square? other)
    {
        if (ReferenceEquals(null, other))
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return MinX == other.MinX && MaxX == other.MaxX && MinY == other.MinY && MaxY == other.MaxY;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (obj.GetType() != GetType())
            return false;
        return Equals((Cube)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(MinX, MaxX, MinY, MaxY);
    }

    public static bool operator ==(Square? left, Square? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Square? left, Square? right)
    {
        return !Equals(left, right);
    }

    public Square Grow(long delta) => new(TopLeft - new V(delta, delta), BottomRight + new V(delta, delta));
    public bool IsEmpty() => MinX > MaxX || MinY > MaxY;

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
