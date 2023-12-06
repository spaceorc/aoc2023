using System;

namespace aoc.ParseLib;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Parameter | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
public class AtomAttribute : StructureAttribute
{
    public AtomAttribute(string separators = StructureParser.DefaultSeparators)
    {
        Separators = separators;
    }

    public string Separators { get; }

    public override string ToString() => $"Atom[{Separators}], {base.ToString()}";

    public override Structure CreateStructure(Type type, StructureParserContext context)
    {
        return new AtomStructure(type, Separators);
    }
}
