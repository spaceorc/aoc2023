using System;
using System.Collections;
using System.Linq;

namespace aoc.Lib;

public static class Helpers
{
    public static T Out<T>(this T value, string prefix)
    {
        if (value is string || value is not IEnumerable enumerable)
        {
            var valueString = Convert.ToString(value);
            Console.WriteLine($"{prefix}{valueString}");
        }
        else
        {
            var arr = enumerable.Cast<object>().ToArray();
            var valueString = string.Join(", ", arr.Select(Convert.ToString));
            Console.WriteLine($"{prefix}{valueString}");
        }

        return value;
    }
}
