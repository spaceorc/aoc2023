using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace aoc.ParseLib;

public record RegexSingleElementStructure(Type Type, Regex Regex, Structure Element) : Structure(Type)
{
    public override object CreateObject(string source)
    {
        var match = Regex.Match(source);
        if (!match.Success)
            throw new InvalidOperationException($"Source doesn't match. Regex: {Regex}. Source: {source}");

        return Element.CreateObject(match.Groups[1].Value);
    }
}
