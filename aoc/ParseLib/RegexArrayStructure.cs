using System;
using System.Text.RegularExpressions;

namespace aoc.ParseLib;

public record RegexArrayStructure(Type Type, Regex Regex, Structure Item) : Structure(Type)
{
    public override object CreateObject(string source)
    {
        var match = Regex.Match(source);
        if (!match.Success)
            throw new InvalidOperationException($"Source doesn't match. Regex: {Regex}. Source: {source}");

        var result = Array.CreateInstance(Type.GetElementType()!, match.Groups[1].Captures.Count);
        for (var i = 0; i < result.Length; i++)
            result.SetValue(Item.CreateObject(match.Groups[1].Captures[i].Value), i);
        return result;
    }
}
