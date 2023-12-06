using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace aoc.ParseLib;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Parameter | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
public class TemplateAttribute : StructureAttribute
{
    public TemplateAttribute(string template)
    {
        Template = template;
    }

    public string Template { get; }
    public bool IsRegex { get; set; }

    public override string ToString() => $"Template[{Template}], IsRegex={IsRegex}, {base.ToString()}";

    public override TypeStructure CreateStructure(Type type, StructureParserContext context)
    {
        var regex = CreateRegex();
        var groupNames = regex.GetGroupNames();

        if (IsPrimitive(type))
            return new RegexSingleElementStructure(type, regex, new AtomStructure(type));

        if (type.IsArray)
        {
            var groupName = groupNames.Single(n => n != "0");
            return new RegexSingleElementStructure(type, regex, StructureParser.Parse(type, null, context.Nested(groupName)));
        }

        var constructor = type.GetConstructors().Single(x => x.GetParameters().Length != 0);
        var paramStructures = new List<(string Group, TypeStructure Structure)>();
        for (var paramIndex = 0; paramIndex < constructor.GetParameters().Length; paramIndex++)
        {
            var parameter = constructor.GetParameters()[paramIndex];
            var name = parameter.Name!;
            var groupName = name == $"item{paramIndex + 1}"
                ? groupNames.Where(n => n != "0").ElementAt(paramIndex)
                : name;
            paramStructures.Add((groupName, StructureParser.Parse(parameter.ParameterType, parameter, context.Nested(groupName))));
        }

        return new RegexClassStructure(type, regex, paramStructures.ToArray());
    }

    private Regex CreateRegex()
    {
        if (IsRegex)
            return new Regex(Template, RegexOptions.Compiled | RegexOptions.Singleline);

        var templateTransformationRegex = new Regex(@"([\\$^()[\]:+*?|])| |({[^}]+})", RegexOptions.Compiled | RegexOptions.Singleline);
        var templateRegexString = "^" +
                                  templateTransformationRegex.Replace(
                                      Template,
                                      m =>
                                      {
                                          if (m.Value == " ")
                                              return "\\s+";

                                          if (m.Value.Length == 1)
                                              return "\\" + m.Value;

                                          if (m.Value == "{?}")
                                              return "(?:.*)";

                                          return $"(?<{m.Value.Substring(1, m.Value.Length - 2)}>.*)";
                                      }
                                  ) +
                                  "$";
        return new Regex(templateRegexString, RegexOptions.Compiled | RegexOptions.Singleline);
    }

    private static bool IsPrimitive(Type type)
    {
        return type == typeof(long) ||
               type == typeof(int) ||
               type == typeof(string) ||
               type == typeof(char) ||
               type.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, new[] { typeof(string) }) is not null;
    }
}
