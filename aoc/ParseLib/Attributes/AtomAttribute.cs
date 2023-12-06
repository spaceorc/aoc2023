using System;
using aoc.ParseLib.Structures;

namespace aoc.ParseLib.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Parameter | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
public class AtomAttribute : StructureAttribute
{
    public AtomAttribute(string separators = TypeStructureParser.DefaultSeparators)
    {
        Separators = separators;
    }

    public string Separators { get; }

    public override string ToString() => $"Atom[{Separators}], {base.ToString()}";

    public override TypeStructure CreateStructure(Type type, TypeStructureParserContext context)
    {
        return new AtomStructure(type, Separators);
    }
}
