using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace aoc.ParseLib;

public record RegexClassStructure(Type Type, Regex Regex, params (string Group, Structure Structure)[] Parameters) : Structure(Type)
{
    public override object CreateObject(string source)
    {
        var match = Regex.Match(source);
        if (!match.Success)
            throw new InvalidOperationException($"Source doesn't match. Regex: {Regex}. Source: {source}");

        var parameters = new object?[Parameters.Length];
        var constructor = Type.GetConstructors().Single(x => x.GetParameters().Any());
        Debug.Assert(constructor.GetParameters().Length == Parameters.Length);
        for (var i = 0; i < Parameters.Length; i++)
        {
            var value = int.TryParse(Parameters[i].Group, out var index)
                ? match.Groups[index].Value
                : match.Groups[Parameters[i].Group].Value;
            parameters[i] = Parameters[i].Structure.CreateObject(value);
        }

        return constructor.Invoke(parameters.ToArray());
    }
}
