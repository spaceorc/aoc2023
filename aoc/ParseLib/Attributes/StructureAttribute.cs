using System;
using aoc.ParseLib.Structures;

namespace aoc.ParseLib.Attributes;

public abstract class StructureAttribute : Attribute
{
    public string? Target { get; set; }

    public override string ToString() => $"Target: {Target}";

    public abstract TypeStructure CreateStructure(Type type, TypeStructureParserContext context);
}
