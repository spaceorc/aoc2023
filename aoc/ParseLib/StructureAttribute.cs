using System;

namespace aoc.ParseLib;

public abstract class StructureAttribute : Attribute
{
    public string? Target { get; set; }

    public override string ToString() => $"Target: {Target}";

    public abstract Structure CreateStructure(Type type, StructureParserContext context);
}
