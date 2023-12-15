using System;
using System.Collections.Generic;
using System.Linq;

namespace aoc.Lib;

public class Cube : IEquatable<Cube>
{
    public Cube(long minX, long maxX, long minY, long maxY, long minZ, long maxZ)
        : this(new V3(minX, minY, minZ), new V3(maxX, maxY, maxZ))
    {
    }

    public Cube(V3 min, V3 max)
    {
        Min = min;
        Max = max;
    }

    public V3 Min { get; }
    public V3 Max { get; }

    public long MinX => Min.X;
    public long MaxX => Max.X;
    public long MinY => Min.Y;
    public long MaxY => Max.Y;
    public long MinZ => Min.Z;
    public long MaxZ => Max.Z;

    public bool Equals(Cube? other)
    {
        if (ReferenceEquals(null, other))
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return MinX == other.MinX &&
               MaxX == other.MaxX &&
               MinY == other.MinY &&
               MaxY == other.MaxY &&
               MinZ == other.MinZ &&
               MaxZ == other.MaxZ;
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
        return HashCode.Combine(MinX, MaxX, MinY, MaxY, MinZ, MaxZ);
    }

    public static bool operator ==(Cube? left, Cube? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Cube? left, Cube? right)
    {
        return !Equals(left, right);
    }

    public Cube Grow(long delta) => new(Min - new V3(delta, delta, delta), Max + new V3(delta, delta, delta));

    public bool IsEmpty() => MinX > MaxX || MinY > MaxY || MinZ > MaxZ;
    public long Volume() => (MaxX - MinX + 1) * (MaxY - MinY + 1) * (MaxZ - MinZ + 1);

    public override string ToString()
    {
        return $"{MinX}..{MaxX}, {MinY}..{MaxY}, {MinZ}..{MaxZ}";
    }

    public Cube[] Subtract(Cube b)
    {
        var (intersection, inA, _) = Intersect(this, b);

        if (intersection == this)
            return Array.Empty<Cube>();

        if (intersection == null)
            return new[] { this };

        return inA;
    }

    public bool IntersectsWith(Cube b)
    {
        if (MinX > b.MaxX || b.MinX > MaxX)
            return false;
        if (MinY > b.MaxY || b.MinY > MaxY)
            return false;
        if (MinZ > b.MaxZ || b.MinZ > MaxZ)
            return false;
        return true;
    }

    public bool Overlaps(Cube b)
    {
        if (MinX > b.MinX || MaxX < b.MaxX)
            return false;
        if (MinY > b.MinY || MaxY < b.MaxY)
            return false;
        if (MinZ > b.MinZ || MaxZ < b.MaxZ)
            return false;
        return true;
    }

    public static (Cube? intersection, Cube[] inA, Cube[] inB) Intersect(Cube a, Cube b)
    {
        if (!a.IntersectsWith(b))
            return (null, new[] { a }, new[] { b });

        var intersection = new List<Cube>();
        var inA = new List<Cube>();
        var inB = new List<Cube>();
        var xs0 = new[] { a.MinX, a.MaxX + 1, b.MinX, b.MaxX + 1 }.Distinct().ToArray();
        var ys0 = new[] { a.MinY, a.MaxY + 1, b.MinY, b.MaxY + 1 }.Distinct().ToArray();
        var zs0 = new[] { a.MinZ, a.MaxZ + 1, b.MinZ, b.MaxZ + 1 }.Distinct().ToArray();
        var xs1 = new[] { a.MinX - 1, a.MaxX, b.MinX - 1, b.MaxX }.Distinct().ToArray();
        var ys1 = new[] { a.MinY - 1, a.MaxY, b.MinY - 1, b.MaxY }.Distinct().ToArray();
        var zs1 = new[] { a.MinZ - 1, a.MaxZ, b.MinZ - 1, b.MaxZ }.Distinct().ToArray();
        Array.Sort(xs0);
        Array.Sort(ys0);
        Array.Sort(zs0);
        Array.Sort(xs1);
        Array.Sort(ys1);
        Array.Sort(zs1);
        for (var ix0 = 0; ix0 < xs0.Length; ix0++)
        for (var ix1 = 0; ix1 < xs1.Length; ix1++)
        {
            if (xs0[ix0] > xs1[ix1])
                continue;

            for (var iy0 = 0; iy0 < ys0.Length; iy0++)
            for (var iy1 = 0; iy1 < ys1.Length; iy1++)
            {
                if (ys0[iy0] > ys1[iy1])
                    continue;

                for (var iz0 = 0; iz0 < zs0.Length; iz0++)
                for (var iz1 = 0; iz1 < zs1.Length; iz1++)
                {
                    if (zs0[iz0] > zs1[iz1])
                        continue;

                    var cube = new Cube(xs0[ix0], xs1[ix1], ys0[iy0], ys1[iy1], zs0[iz0], zs1[iz1]);
                    var v = new V3(cube.MinX, cube.MinY, cube.MinZ);
                    if (v.InCube(a))
                    {
                        if (v.InCube(b))
                            intersection.Add(cube);
                        else
                            inA.Add(cube);
                    }
                    else if (v.InCube(b))
                        inB.Add(cube);

                    break;
                }

                break;
            }

            break;
        }

        return (intersection.Single(), inA.ToArray(), inB.ToArray());
    }

    public HashSet<V3> Border()
    {
        var border = new HashSet<V3>();
        for (var x = MinX; x <= MaxX; x++)
        for (var y = MinY; y <= MaxY; y++)
        {
            border.Add(new V3(x, y, MinZ));
            border.Add(new V3(x, y, MaxZ));
        }

        for (var x = MinX; x <= MaxX; x++)
        for (var z = MinZ; z <= MaxZ; z++)
        {
            border.Add(new V3(x, MinY, z));
            border.Add(new V3(x, MaxY, z));
        }

        for (var y = MinY; y <= MaxY; y++)
        for (var z = MinZ; z <= MaxZ; z++)
        {
            border.Add(new V3(MinX, y, z));
            border.Add(new V3(MaxX, y, z));
        }

        return border;
    }

    public IEnumerable<V3> All()
    {
        for (var x = MinX; x <= MaxX; x++)
        for (var y = MinY; y <= MaxY; y++)
        for (var z = MinZ; z <= MaxZ; z++)
            yield return new V3(x, y, z);
    }

    public bool Contains(V3 other) => other.InCube(this);
}
