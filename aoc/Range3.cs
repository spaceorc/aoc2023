using System.Collections.Generic;

namespace aoc;

public class Range3
{
    public long MinX { get; }
    public long MinY { get; }
    public long MinZ { get; }
    public long MaxX { get; }
    public long MaxY { get; }
    public long MaxZ { get; }

    public Range3(long minX, long minY, long minZ, long maxX, long maxY, long maxZ)
    {
        MinX = minX;
        MinY = minY;
        MinZ = minZ;
        MaxX = maxX;
        MaxY = maxY;
        MaxZ = maxZ;
    }

    public bool Contains(V3 other)
    {
        return other.X <= MaxX && other.X >= MinX
                               && other.Y <= MaxY && other.Y >= MinY
                               && other.Z <= MaxZ && other.Z >= MinZ;
    }

    public IEnumerable<V3> All()
    {
        for (var x = MinX; x <= MaxX; x++)
        for (var y = MinY; y <= MaxY; y++)
        for (var z = MinZ; z <= MaxZ; z++)
            yield return new V3(x, y, z);
    }

    public List<V3> Border()
    {
        var border = new List<V3>();
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
}