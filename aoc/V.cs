using System;

namespace aoc;

public struct V : IEquatable<V>
{
    public static readonly V Zero = new(0, 0); 
    public readonly long X;
    public readonly long Y;

    public V(long x, long y)
    {
        X = x;
        Y = y;
    }

    public static V Parse(string s)
    {
        return Parse(s, ",");
    }

    public static V Parse(string s, params string[] sep)
    {
        return Parse(s.Split(sep, StringSplitOptions.RemoveEmptyEntries));
    }

    public static V Parse(string[] s)
    {
        return new V(long.Parse(s[0]), long.Parse(s[1]));
    }

    public bool Equals(V other) => X == other.X && Y == other.Y;
    public override bool Equals(object? obj) => obj is V other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(X, Y);
    public static bool operator ==(V a, V b) => a.Equals(b);
    public static bool operator !=(V a, V b) => !(a == b);
    public static V operator +(V a, V b) => new(a.X + b.X, a.Y + b.Y);
    public static V operator *(V a, long k) => new(a.X * k, a.Y * k);
    public static V operator /(V a, long k) => new(a.X / k, a.Y / k);
    public static V operator -(V a, V b) => new(a.X - b.X, a.Y - b.Y);
    public static V operator -(V a) => new(-a.X, -a.Y);

    public long MLen() => Math.Abs(X) + Math.Abs(Y);
    public static long XProd(V a, V b) => a.X * b.Y - a.Y * b.X;
    public static long DProd(V a, V b) => a.X * b.X + a.Y * b.Y;


    public V Dir => new V(Math.Sign(X), Math.Sign(Y));
    
    public override string ToString() => $"{X} {Y}";

    public static  V[] nears = { new(1, 0), new(-1, 0), new(0, 1), new(0, -1) };
    public static  V[] nears8 =
    {
        new(1, 0), new(-1, 0), new(0, 1), new(0, -1),
        new(1, 1), new(-1, -1), new(-1, 1), new(1, -1),
    };

    public bool InRange(Range r) => X >= r.MinX && X <= r.MaxX && Y >= r.MinY && Y <= r.MaxY;
}