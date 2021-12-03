using System;

namespace aoc
{
    public struct V : IEquatable<V>
    {
        public readonly long X;
        public readonly long Y;

        public V(long x, long y)
        {
            X = x;
            Y = y;
        }

        public bool Equals(V other) => X == other.X && Y == other.Y;
        public override bool Equals(object obj) => obj is V other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(X, Y);
        public static bool operator ==(V a, V b) => a.Equals(b);
        public static bool operator !=(V a, V b) => !(a == b);
        public static V operator +(V a, V b) => new V(a.X + b.X, a.Y + b.Y);
        public static V operator *(V a, long k) => new V(a.X * k, a.Y * k);
        public static V operator -(V a, V b) => new V(a.X - b.X, a.Y - b.Y);
        public static V operator -(V a) => new V(-a.X, -a.Y);

        public long MLen() => Math.Abs(X) + Math.Abs(Y);
        public static long XProd(V a, V b) => a.X * b.Y - a.Y * b.X;
        public static long DProd(V a, V b) => a.X * b.X + a.Y * b.Y;

        public override string ToString() => $"{X} {Y}";
    }
}