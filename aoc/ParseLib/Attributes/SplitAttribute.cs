using System;
using System.Collections.Generic;
using System.Linq;
using aoc.ParseLib.Structures;

namespace aoc.ParseLib.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Parameter | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
public class SplitAttribute : StructureAttribute
{
    public SplitAttribute(string separators = TypeStructureParser.DefaultSeparators)
    {
        Separators = separators;
    }

    public string Separators { get; }

    public override string ToString() => $"Split[{Separators}], {base.ToString()}";

    public override TypeStructure CreateStructure(Type type, TypeStructureParserContext context)
    {
        if (type.IsArray)
            return new SplitArrayStructure(type, Separators, TypeStructureParser.Parse(type.GetElementType()!, null, context.Nested("item")));

        var parameters = new List<TypeStructure>();
        var constructor = type.GetConstructors().Single(x => x.GetParameters().Length != 0);
        for (var paramIndex = 0; paramIndex < constructor.GetParameters().Length; paramIndex++)
        {
            var parameter = constructor.GetParameters()[paramIndex];
            var name = parameter.Name!;
            var childName = name == $"item{paramIndex + 1}"
                ? (paramIndex + 1).ToString()
                : name;
            parameters.Add(TypeStructureParser.Parse(parameter.ParameterType, parameter, context.Nested(childName)));
        }

        return new SplitClassStructure(type, Separators, parameters.ToArray());
    }
}
