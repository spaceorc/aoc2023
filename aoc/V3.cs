using System;
using System.Collections.Generic;
using System.Linq;

namespace aoc;

public class V3 : IEquatable<V3>
{
    public static readonly V3 Zero = new V3(0, 0, 0);
    public readonly long X;
    public readonly long Y;
    public readonly long Z;

    public V3(long x, long y, long z)
    {
        X = x;
        Y = y;
        Z = z;
    }
    
    public static V3 Parse(string s)
    {
        return Parse(s, ",");
    }

    public static V3 Parse(string s, params string[] sep)
    {
        return Parse(s.Split(sep, StringSplitOptions.RemoveEmptyEntries));
    }

    public static V3 Parse(string[] s)
    {
        return new V3(long.Parse(s[0]), long.Parse(s[1]), long.Parse(s[2]));
    }
    
    public bool Equals(V3 other) => X == other.X && Y == other.Y && Z == other.Z;
    public override bool Equals(object obj) => obj is V3 other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(X, Y, Z);

    public static bool operator ==(V3 a, V3 b) => a.Equals(b);
    public static bool operator !=(V3 a, V3 b) => !(a == b);
    public static V3 operator +(V3 a, V3 b) => new V3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    public static V3 operator -(V3 a, V3 b) => new V3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

    public long MLen() => Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z);
    public long CLen() => Math.Max(Math.Max(Math.Abs(X), Math.Abs(Y)), Math.Abs(Z));
    public static long DProd(V3 a, V3 b) => a.X * b.X + a.Y * b.Y + a.Z * b.Z;

    public override string ToString() => $"{X} {Y} {Z}";

    public IEnumerable<V3> NearsAndSelf()
    {
        for (var z = -1; z <= 1; z++)
        for (var y = -1; y <= 1; y++)
        for (var x = -1; x <= 1; x++)
            yield return this + new V3(x, y, z);
    }

    public IEnumerable<V3> Nears() => NearsAndSelf().Where(x => x != this).ToArray();

    public V3 Rotate(int dir) => Rotations3.Rotate(this, dir);
    public V3 RotateBack(int dir) => Rotations3.RotateBack(this, dir);
}