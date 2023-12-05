using System;

namespace aoc.ParseLib;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Parameter, AllowMultiple = true, Inherited = false)]
public class RegexAttribute : StructureAttribute
{
    public RegexAttribute(string regex)
    {
        Regex = regex;
    }

    public string Regex { get; }

    public override string ToString() => $"Regex[{Regex}], {base.ToString()}";
}
