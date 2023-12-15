using System.Linq;

namespace aoc.Lib;

public static class MathHelpers
{
    public static long Lcm(params long[] values)
    {
        return values.Aggregate(1L, Lcm);
    }

    public static long Lcm(long a, long b)
    {
        return a / Gcd(a, b) * b;
    }

    public static int Lcm(params int[] values)
    {
        return values.Aggregate(1, Lcm);
    }

    public static int Lcm(int a, int b)
    {
        return a / Gcd(a, b) * b;
    }

    public static long Gcd(long a, long b)
    {
        while (a != 0 && b != 0)
        {
            if (a > b)
                a %= b;
            else
                b %= a;
        }

        return a + b;
    }

    public static int Gcd(int a, int b)
    {
        while (a != 0 && b != 0)
        {
            if (a > b)
                a %= b;
            else
                b %= a;
        }

        return a + b;
    }

    public static long Mod(this long v, long divisor) => (v % divisor + divisor) % divisor;
    public static int Mod(this int v, int divisor) => (v % divisor + divisor) % divisor;
}
