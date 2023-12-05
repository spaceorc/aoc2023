using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace aoc.ParseLib;

public static class StructureParser
{
    public const string DefaultSeparators = "- ;,:|\n";

    public static Structure Parse(Type type, ParameterInfo? parameterInfo)
    {
        var context = StructureParserContext.CreateRoot();
        var result = Parse(type, parameterInfo, context);
        context.Validate();
        return result;
    }

    public static Structure Parse(
        Type type,
        ParameterInfo? parameterInfo,
        StructureParserContext context
    )
    {
        var attributes = GetStructureAttributes(type, parameterInfo);
        foreach (var attribute in attributes)
            context.DeclareAttribute(attribute);

        var structureAttribute = context.GetStructureAttribute();
        if (structureAttribute == null)
            return new AtomStructure(type);

        context.UseAttribute(structureAttribute);
        return structureAttribute.CreateStructure(type, context);
    }

    private static List<StructureAttribute> GetStructureAttributes(params ICustomAttributeProvider?[] customAttributeProviders)
    {
        return customAttributeProviders
            .Where(customAttributeProvider => customAttributeProvider != null)
            .SelectMany(customAttributeProvider => customAttributeProvider!.GetCustomAttributes(false))
            .OfType<StructureAttribute>()
            .ToList();
    }
}
