using System;
using System.Collections.Generic;
using System.Linq;

namespace aoc
{
    public class V3 : IEquatable<V3>
    {
        public readonly long X;
        public readonly long Y;
        public readonly long Z;

        public V3(long x, long y, long z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public bool Equals(V3 other) => X == other.X && Y == other.Y && Z == other.Z;
        public override bool Equals(object obj) => obj is V3 other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(X, Y, Z);

        public static bool operator ==(V3 a, V3 b) => a.Equals(b);
        public static bool operator !=(V3 a, V3 b) => !(a == b);
        public static V3 operator +(V3 a, V3 b) => new V3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static V3 operator -(V3 a, V3 b) => new V3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

        public long MLen() => Math.Abs(X) + Math.Abs(Y) + Math.Abs(Z);
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
    }
}