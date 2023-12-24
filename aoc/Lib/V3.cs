using System;
using System.Collections.Generic;
using System.Linq;

namespace aoc.Lib;

public class V3 : IEquatable<V3>
{
    public static readonly V3 Zero = new(0, 0, 0);
    public static readonly V3 One = new(1, 1, 1);

    public static readonly V3[] neighbors =
    {
        new(1, 0, 0),
        new(0, 1, 0),
        new(0, 0, 1),
        new(-1, 0, 0),
        new(0, -1, 0),
        new(0, 0, -1),
    };

    public readonly long X;
    public readonly long Y;
    public readonly long Z;

    public V3(long x, long y, long z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public bool Equals(V3? other) => !ReferenceEquals(other, null) && X == other.X && Y == other.Y && Z == other.Z;
    public override bool Equals(object? obj) => obj is V3 other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(X, Y, Z);

    public static bool operator ==(V3 a, V3 b) => a.Equals(b);
    public static bool operator !=(V3 a, V3 b) => !(a == b);
    public static V3 operator +(V3 a, V3 b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    public static V3 operator -(V3 a, V3 b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    public static V3 operator *(V3 a, long k) => new(a.X * k, a.Y * k, a.Z * k);
    public static V3 operator *(long k, V3 a) => a * k;
    public static V3 operator /(V3 a, long k) => new(a.X / k, a.Y / k, a.Z / k);

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

    public IEnumerable<V3> Neighbors() => neighbors.Select(n => this + n);

    public V3 Rotate(int dir) => Rotations3.Rotate(this, dir);
    public V3 RotateBack(int dir) => Rotations3.RotateBack(this, dir);
    public static V3 FromZ(long z) => new V3(0, 0, z);

    public V XY() => new V(X, Y);
    
    public V3Rat ToRational() => new(X, Y, Z);
    
    public bool InCube(Cube c) => X >= c.MinX && X <= c.MaxX && Y >= c.MinY && Y <= c.MaxY && Z >= c.MinZ && Z <= c.MaxZ;
}
