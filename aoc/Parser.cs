using System;
using System.Collections.Generic;
using System.Linq;

namespace aoc;

public static class Parser
{
    public static T[] ParseAll<T>(this IEnumerable<string> lines, string separators = "- ;,")
    {
        return lines.Select(x => Parse<T>(x, separators)).ToArray();
    }

    public static T Parse<T>(this string line, string separators = "- ;,")
    {
        var source = new Queue<string>(line.Split(separators.ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
        return (T)ReadFrom(typeof(T), source);
    }

    private static object ReadFrom(Type type, Queue<string> source)
    {
        if (type == typeof(long))
            return long.Parse(source.Dequeue());
        if (type == typeof(int))
            return int.Parse(source.Dequeue());
        if (type == typeof(string))
            return source.Dequeue();
        if (type == typeof(char))
        {
            var value = source.Dequeue();
            if (value.Length != 1)
                throw new InvalidOperationException($"Invalid char {value}");
            return value[0];
        }

        var constructor = type.GetConstructors().Single(x => x.GetParameters().Any());
        var parameters = new List<object>();
        foreach (var parameter in constructor.GetParameters())
            parameters.Add(ReadFrom(parameter.ParameterType, source));
        return constructor.Invoke(parameters.ToArray());
    }
}