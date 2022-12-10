using System;

namespace aoc;

[AttributeUsage(AttributeTargets.Parameter)]
public class SplitAttribute : Attribute
{
    public SplitAttribute(string separators)
    {
        Separators = separators;
    }

    public string Separators { get; }
}