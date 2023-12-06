using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace aoc.ParseLib;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Parameter | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
public class RegexArrayAttribute : StructureAttribute
{
    public RegexArrayAttribute(string regex)
    {
        Regex = regex;
    }

    public string Regex { get; }

    public override string ToString() => $"RegexArray[{Regex}], {base.ToString()}";

    public override TypeStructure CreateStructure(Type type, StructureParserContext context)
    {
        var regex = CreateRegex();
        var groupNames = regex.GetGroupNames();
        if (!type.IsArray)
            throw new InvalidOperationException($"Regex array attribute can be applied only to array. Target: {context.Target}");
        var groupName = groupNames.Single(n => n != "0");
        return new RegexArrayStructure(type, regex, StructureParser.Parse(type.GetElementType()!, null, context.Nested(groupName)));
    }

    private Regex CreateRegex()
    {
        return new Regex(Regex, RegexOptions.Compiled | RegexOptions.Singleline);
    }
}
