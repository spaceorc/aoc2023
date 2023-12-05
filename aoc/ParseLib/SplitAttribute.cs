using System;

namespace aoc.ParseLib;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Parameter, AllowMultiple = true, Inherited = false)]
public class SplitAttribute : StructureAttribute
{
    public SplitAttribute(string separators = "- ;,:|")
    {
        Separators = separators;
    }

    public string Separators { get; }

    public override string ToString() => $"Split[{Separators}], {base.ToString()}";
}