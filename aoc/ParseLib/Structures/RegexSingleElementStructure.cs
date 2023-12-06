using System;
using System.Text.RegularExpressions;

namespace aoc.ParseLib.Structures;

public record RegexSingleElementStructure(Type Type, Regex Regex, TypeStructure Element) : TypeStructure(Type)
{
    public override object CreateObject(string source)
    {
        var match = Regex.Match(source);
        if (!match.Success)
            throw new InvalidOperationException($"Source doesn't match. Regex: {Regex}. Source: {source}");

        return Element.CreateObject(match.Groups[1].Value);
    }
}
