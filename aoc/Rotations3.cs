using System.Collections.Generic;
using System.Linq;

namespace aoc;

public static class Rotations3
{
    private static List<long[,]> A = new()
    {
        new long[,]
        {
            { 1, 0, 0 },
            { 0, 1, 0 },
            { 0, 0, 1 },
        },
        new long[,]
        {
            { 0, 1, 0 },
            { 0, 0, 1 },
            { 1, 0, 0 },
        },
        new long[,]
        {
            { 0, 0, 1 },
            { 1, 0, 0 },
            { 0, 1, 0 },
        },
    };

    private static List<long[,]> B = new()
    {
        new long[,]
        {
            { 1, 0, 0 },
            { 0, 1, 0 },
            { 0, 0, 1 },
        },
        new long[,]
        {
            { -1, 0, 0 },
            { 0, -1, 0 },
            { 0, 0, 1 },
        },
        new long[,]
        {
            { -1, 0, 0 },
            { 0, 1, 0 },
            { 0, 0, -1 },
        },
        new long[,]
        {
            { 1, 0, 0 },
            { 0, -1, 0 },
            { 0, 0, -1 },
        },
    };

    private static List<long[,]> C = new()
    {
        new long[,]
        {
            { 1, 0, 0 },
            { 0, 1, 0 },
            { 0, 0, 1 },
        },
        new long[,]
        {
            { 0, 0, -1 },
            { 0, -1, 0 },
            { -1, 0, 0 },
        }
    };

    static long[,] Mul(long[,] a, long[,] b)
    {
        var res = new long[3, 3];
        for (var i = 0; i < 3; i++)
        {
            for (var j = 0; j < 3; j++)
            {
                var temp = 0L;
                for (var k = 0; k < 3; k++)
                    temp += a[i, k] * b[k, j];

                res[i, j] = temp;
            }
        }

        return res;
    }

    static V3 Mul(long[,] a, V3 v)
    {
        return new V3(
            a[0, 0] * v.X + a[0, 1] * v.Y + a[0, 2] * v.Z,
            a[1, 0] * v.X + a[1, 1] * v.Y + a[1, 2] * v.Z,
            a[2, 0] * v.X + a[2, 1] * v.Y + a[2, 2] * v.Z
        );
    }

    static List<long[,]> GenRotations()
    {
        var res = new List<long[,]>();
        for (int i = 0; i < A.Count; i++)
        for (int j = 0; j < B.Count; j++)
        for (int k = 0; k < C.Count; k++)
            res.Add(Mul(Mul(A[i], B[j]), C[k]));

        return res;
    }

    static long[,] Inverse(long[,] rot)
    {
        var res = new long[3, 3];
        for (int i = 0; i < 3; i++)
        for (int j = 0; j < 3; j++)
        {
            res[i, j] = rot[j, i];
        }

        return res;
    }

    static List<long[,]> GenRotationInversions()
    {
        return ROTATIONS.Select(Inverse).ToList();
    }

    static bool IsIdentity(long[,] m)
    {
        for (int i = 0; i < 3; i++)
        for (int j = 0; j < 3; j++)
        {
            if (i == j && m[i, j] != 1)
                return false;
            if (i != j && m[i, j] != 0)
                return false;
        }

        return true;
    }

    private static List<long[,]> ROTATIONS = GenRotations();
    private static List<long[,]> ROTATION_INVERSIONS = GenRotationInversions();

    public static V3 Rotate(V3 v, int direction)
    {
        return Mul(ROTATIONS[direction], v);
    }
    
    public static V3 RotateBack(V3 v, int direction)
    {
        return Mul(ROTATION_INVERSIONS[direction], v);
    }
}