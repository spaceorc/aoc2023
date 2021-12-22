using System;
using System.Collections.Generic;
using System.Linq;

namespace aoc;

public class Cube : IEquatable<Cube>
{
    public long MinX { get; }
    public long MaxX { get; }
    public long MinY { get; }
    public long MaxY { get; }
    public long MinZ { get; }
    public long MaxZ { get; }

    public Cube(long minX, long maxX, long minY, long maxY, long minZ, long maxZ)
    {
        MinX = minX;
        MaxX = maxX;
        MinY = minY;
        MaxY = maxY;
        MinZ = minZ;
        MaxZ = maxZ;
    }

    public bool Equals(Cube? other)
    {
        if (ReferenceEquals(null, other))
            return false;
        if (ReferenceEquals(this, other))
            return true;
        return MinX == other.MinX && MaxX == other.MaxX && MinY == other.MinY && MaxY == other.MaxY &&
               MinZ == other.MinZ && MaxZ == other.MaxZ;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (obj.GetType() != this.GetType())
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

    public bool IsEmpty() => MinX > MaxX || MinY > MaxY || MinZ > MaxZ;
    public long Size() => (MaxX - MinX + 1) * (MaxY - MinY + 1) * (MaxZ - MinZ + 1);

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
}