using System;
using System.Collections.Generic;
using System.Linq;

namespace aoc.Lib;

public static class RationalExtensions
{
    public static Rational Sum(this IEnumerable<Rational> source)
    {
        return source.Aggregate(Rational.Zero, (current, value) => current + value);
    }
        
    public static Rational Max(this IEnumerable<Rational> source)
    {
        return source.Aggregate(Rational.Max);
    }

    public static Rational Sum<T>(this IEnumerable<T> source, Func<T, Rational> selector)
    {
        return source.Select(selector).Sum();
    }
}
