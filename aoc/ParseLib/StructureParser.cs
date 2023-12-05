using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace aoc.ParseLib;

public static class StructureParser
{
    public static Structure Parse(Type type, ICustomAttributeProvider? customAttributeProvider)
    {
        var allAttributes = new Dictionary<StructureAttribute, string>();
        var usedAttributes = new Dictionary<StructureAttribute, string>();
        var result = Parse(
            type,
            customAttributeProvider,
            "",
            Array.Empty<(string target, StructureAttribute attribute)>(),
            allAttributes,
            usedAttributes
        );
        foreach (var (attribute, target) in allAttributes)
        {
            if (!usedAttributes.ContainsKey(attribute))
                throw new InvalidOperationException($"Attribute is not used. Define target: {target}; Attribute: {attribute}");
        }
        return result;
    }

    private static Structure Parse(
        Type type,
        ICustomAttributeProvider? customAttributeProvider,
        string target,
        (string target, StructureAttribute attribute)[] parentAttributes,
        Dictionary<StructureAttribute, string> allAttributes,
        Dictionary<StructureAttribute, string> usedAttributes
    )
    {
        var attributes = GetStructureAttributes(type, customAttributeProvider);
        foreach (var attribute in attributes)
            allAttributes.Add(attribute, target);
        
        var childAttributes = parentAttributes.Concat(attributes.Select(a => (target, a))).ToArray();

        var applicableAttributes = attributes
            .Where(x => string.IsNullOrEmpty(x.Target))
            .Concat(parentAttributes.Where(pa => CombineTarget(pa.target, pa.attribute.Target) == target).Select(pa => pa.attribute))
            .ToList();

        var (regex, regexAttribute) = TryGetRegex(applicableAttributes, target, usedAttributes);
        if (regex != null)
        {
            var groupNames = regex.GetGroupNames();
            
            if (regexAttribute is RegexArrayAttribute)
            {
                if (!type.IsArray)
                    throw new InvalidOperationException($"Regex array attribute can be applied only to array. Target: {target}");
                var groupName = groupNames.Single(n => n != "0");
                return new RegexArrayStructure(type, regex, Parse(type.GetElementType()!, null, CombineTarget(target, groupName), childAttributes, allAttributes, usedAttributes));
            }

            if (IsPrimitive(type))
                return new RegexSingleElementStructure(type, regex, new PrimitiveStructure(type));

            if (type.IsArray)
            {
                var groupName = groupNames.Single(n => n != "0");
                return new RegexSingleElementStructure(
                    type,
                    regex,
                    Parse(
                        type,
                        null,
                        CombineTarget(target, groupName),
                        childAttributes,
                        allAttributes,
                        usedAttributes
                    )
                );
            }

            var constructor = type.GetConstructors().Single(x => x.GetParameters().Length != 0);
            var paramStructures = new List<(string Group, Structure Structure)>();
            for (var paramIndex = 0; paramIndex < constructor.GetParameters().Length; paramIndex++)
            {
                var parameter = constructor.GetParameters()[paramIndex];
                var name = parameter.Name!;
                var groupName = name == $"item{paramIndex + 1}"
                    ? groupNames.Where(n => n != "0").ElementAt(paramIndex)
                    : name;
                paramStructures.Add(
                    (
                        groupName,
                        Parse(
                            parameter.ParameterType,
                            parameter,
                            CombineTarget(target, groupName),
                            childAttributes,
                            allAttributes,
                            usedAttributes
                        )
                    )
                );
            }

            return new RegexClassStructure(type, regex, paramStructures.ToArray());
        }

        if (IsPrimitive(type))
            return new PrimitiveStructure(type);

        var splitAttribute = applicableAttributes.OfType<SplitAttribute>().SingleOrDefault();
        if (splitAttribute != null)
        {
            usedAttributes.Add(splitAttribute, target);
            if (type.IsArray)
                return new SplitArrayStructure(
                    type,
                    splitAttribute.Separators,
                    Parse(
                        type.GetElementType()!,
                        null,
                        CombineTarget(target, "item"),
                        childAttributes,
                        allAttributes,
                        usedAttributes
                    )
                );

            var parameters = new List<Structure>();
            var constructor = type.GetConstructors().Single(x => x.GetParameters().Length != 0);
            for (var paramIndex = 0; paramIndex < constructor.GetParameters().Length; paramIndex++)
            {
                var parameter = constructor.GetParameters()[paramIndex];
                var name = parameter.Name!;
                var childName = name == $"item{paramIndex + 1}"
                    ? (paramIndex + 1).ToString()
                    : name;
                parameters.Add(
                    Parse(
                        parameter.ParameterType,
                        parameter,
                        CombineTarget(target, childName),
                        childAttributes,
                        allAttributes,
                        usedAttributes
                    )
                );
            }

            return new SplitClassStructure(type, splitAttribute.Separators, parameters.ToArray());
        }

        var recursiveSplitAttribute = applicableAttributes.OfType<RecursiveSplitAttribute>().SingleOrDefault();
        if (recursiveSplitAttribute != null)
            usedAttributes.Add(recursiveSplitAttribute, target);
        return new SplitTreeStructure(type, (recursiveSplitAttribute ?? new RecursiveSplitAttribute()).Separators);
    }

    private static bool IsPrimitive(Type type)
    {
        return type == typeof(long) ||
               type == typeof(int) ||
               type == typeof(string) ||
               type == typeof(string) ||
               type == typeof(char) ||
               type.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, new[] { typeof(string) }) is not null;
    }

    private static (Regex? regex, StructureAttribute? attribute) TryGetRegex(IReadOnlyList<StructureAttribute> applicableAttributes, string target, Dictionary<StructureAttribute, string> usedAttributes)
    {
        var templateAttribute = applicableAttributes.OfType<TemplateAttribute>().SingleOrDefault();
        var regexAttribute = applicableAttributes.OfType<RegexAttribute>().SingleOrDefault();
        var regexArrayAttribute = applicableAttributes.OfType<RegexArrayAttribute>().SingleOrDefault();
        if (templateAttribute != null && regexAttribute != null)
            throw new InvalidOperationException("Both template and regex are set");
        if (templateAttribute != null && regexArrayAttribute != null)
            throw new InvalidOperationException("Both template and regex array are set");
        if (regexAttribute != null && regexArrayAttribute != null)
            throw new InvalidOperationException("Both regex and regex array are set");

        if (templateAttribute != null)
        {
            usedAttributes.Add(templateAttribute, target);
            var templateTransformationRegex = new Regex(@"([\\$^()[\]:+*?|])| |({[^}]+})", RegexOptions.Compiled | RegexOptions.Singleline);
            var templateRegexString = "^" +
                                      templateTransformationRegex.Replace(
                                          templateAttribute.Template,
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
            return (new Regex(templateRegexString, RegexOptions.Compiled | RegexOptions.Singleline), templateAttribute);
        }

        if (regexAttribute != null)
        {
            usedAttributes.Add(regexAttribute, target);
            return (new Regex(regexAttribute.Regex, RegexOptions.Compiled | RegexOptions.Singleline), regexAttribute);
        }
        
        if (regexArrayAttribute != null)
        {
            usedAttributes.Add(regexArrayAttribute, target);
            return (new Regex(regexArrayAttribute.Regex, RegexOptions.Compiled | RegexOptions.Singleline), regexArrayAttribute);
        }

        return (null, null);
    }

    private static string CombineTarget(string parent, string? child)
    {
        return string.IsNullOrEmpty(child) ? parent : $"{parent}.{child}";
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
