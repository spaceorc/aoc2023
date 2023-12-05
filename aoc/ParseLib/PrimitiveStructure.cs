using System;
using System.Reflection;

namespace aoc.ParseLib;

public record PrimitiveStructure(Type Type) : Structure(Type)
{
    public override object CreateObject(string source)
    {
        if (Type == typeof(long))
            return long.Parse(source);
        if (Type == typeof(int))
            return int.Parse(source);
        if (Type == typeof(string))
            return source;
        if (Type == typeof(char))
        {
            if (source.Length != 1)
                throw new InvalidOperationException($"Invalid char {source}");
            return source[0];
        }

        var typeParseMethod = Type.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, new[] { typeof(string) });
        if (typeParseMethod != null)
            return typeParseMethod.Invoke(null, new object?[] { source })!;

        throw new InvalidOperationException($"Invalid primitive type {Type}");
    }
}
