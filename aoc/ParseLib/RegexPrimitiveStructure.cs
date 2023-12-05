using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace aoc.ParseLib;

public record RegexPrimitiveStructure(Type Type, Regex Regex) : Structure(Type)
{
    public override object CreateObject(string source)
    {
        var match = Regex.Match(source);
        if (!match.Success)
            throw new InvalidOperationException($"Source doesn't match. Regex: {Regex}. Source: {source}");
        
        if (Type == typeof(long))
            return long.Parse(match.Groups[1].Value);
        if (Type == typeof(int))
            return int.Parse(match.Groups[1].Value);
        if (Type == typeof(string))
            return match.Groups[1].Value;
        if (Type == typeof(char))
        {
            var value = match.Groups[1].Value;
            if (value.Length != 1)
                throw new InvalidOperationException($"Invalid char {value}");
            return value[0];
        }
        
        var typeParseMethod = Type.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, new[] { typeof(string) });
        if (typeParseMethod != null)
            return typeParseMethod.Invoke(null, new object?[] { match.Groups[1].Value })!;
        
        throw new InvalidOperationException($"Invalid primitive type {Type}");
    }
}
