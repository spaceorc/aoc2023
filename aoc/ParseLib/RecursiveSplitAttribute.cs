using System;

namespace aoc.ParseLib;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Parameter, AllowMultiple = true, Inherited = false)]
public class RecursiveSplitAttribute : StructureAttribute
{
    public RecursiveSplitAttribute(string separators = "- ;,:|")
    {
        Separators = separators;
    }

    public string Separators { get; }

    public override string ToString() => $"RecursiveSplit[{Separators}], {base.ToString()}";
}
