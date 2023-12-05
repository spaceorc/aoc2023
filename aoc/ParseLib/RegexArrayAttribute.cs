using System;

namespace aoc.ParseLib;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Parameter, AllowMultiple = true, Inherited = false)]
public class RegexArrayAttribute : StructureAttribute
{
    public RegexArrayAttribute(string regex)
    {
        Regex = regex;
    }

    public string Regex { get; }

    public override string ToString() => $"RegexArray[{Regex}], {base.ToString()}";
}
