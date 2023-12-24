using System;
using System.Collections.Generic;
using System.Linq;

namespace aoc.Lib;

public class V3Rat : IEquatable<V3Rat>
{
    public static readonly V3Rat Zero = new(0, 0, 0);
    public static readonly V3Rat One = new(1, 1, 1);

    public readonly Rational X;
    public readonly Rational Y;
    public readonly Rational Z;

    public V3Rat(Rational x, Rational y, Rational z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public bool Equals(V3Rat? other) => !ReferenceEquals(other, null) && X == other.X && Y == other.Y && Z == other.Z;
    public override bool Equals(object? obj) => obj is V3Rat other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(X, Y, Z);

    public static bool operator ==(V3Rat a, V3Rat b) => a.Equals(b);
    public static bool operator !=(V3Rat a, V3Rat b) => !(a == b);
    public static V3Rat operator +(V3Rat a, V3Rat b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    public static V3Rat operator -(V3Rat a, V3Rat b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    public static V3Rat operator *(V3Rat a, long k) => new(a.X * k, a.Y * k, a.Z * k);
    public static V3Rat operator *(long k, V3Rat a) => a * k;
    public static V3Rat operator /(V3Rat a, long k) => new(a.X / k, a.Y / k, a.Z / k);
    public static V3Rat operator *(V3Rat a, Rational k) => new(a.X * k, a.Y * k, a.Z * k);
    public static V3Rat operator *(Rational k, V3Rat a) => a * k;
    public static V3Rat operator /(V3Rat a, Rational k) => new(a.X / k, a.Y / k, a.Z / k);

    public Rational MLen() => X.Abs() + Y.Abs() + Z.Abs();
    public Rational CLen() => Rational.Max(X.Abs(), Y.Abs(), Z.Abs());
    public static Rational DProd(V3Rat a, V3Rat b) => a.X * b.X + a.Y * b.Y + a.Z * b.Z;

    public override string ToString() => $"{X} {Y} {Z}";
}
