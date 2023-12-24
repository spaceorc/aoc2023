using System;

namespace aoc.Lib;

public class VRat : IEquatable<VRat>
{
    public static readonly VRat Zero = new(0, 0);
    public static readonly VRat One = new(1, 1);

    public readonly Rational X;
    public readonly Rational Y;

    public VRat(Rational x, Rational y)
    {
        X = x;
        Y = y;
    }

    public bool Equals(VRat? other) => !ReferenceEquals(other, null) && X == other.X && Y == other.Y;
    public override bool Equals(object? obj) => obj is VRat other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(X, Y);

    public static bool operator ==(VRat a, VRat b) => a.Equals(b);
    public static bool operator !=(VRat a, VRat b) => !(a == b);
    public static VRat operator +(VRat a, VRat b) => new(a.X + b.X, a.Y + b.Y);
    public static VRat operator -(VRat a, VRat b) => new(a.X - b.X, a.Y - b.Y);
    public static VRat operator -(VRat b) => new(-b.X, -b.Y);
    public static VRat operator *(VRat a, long k) => new(a.X * k, a.Y * k);
    public static VRat operator *(long k, VRat a) => a * k;
    public static VRat operator /(VRat a, long k) => new(a.X / k, a.Y / k);
    public static VRat operator *(VRat a, Rational k) => new(a.X * k, a.Y * k);
    public static VRat operator *(Rational k, VRat a) => a * k;
    public static VRat operator /(VRat a, Rational k) => new(a.X / k, a.Y / k);

    public Rational MLen() => X.Abs() + Y.Abs();
    public Rational CLen() => Rational.Max(X.Abs(), Y.Abs());
    public static Rational DProd(VRat a, VRat b) => a.X * b.X + a.Y * b.Y;

    public override string ToString() => $"{X} {Y}";
}
