using System;
using System.Collections.Generic;
using System.Linq;

namespace aoc.ParseLib;

public record StructureParserContext(
    string Target,
    List<StructureAttribute> Attributes,
    (string Target, StructureAttribute Attribute)[] ParentAttributes,
    Dictionary<StructureAttribute, string> AllAttributes,
    Dictionary<StructureAttribute, string> UsedAttributes
)
{
    public void DeclareAttribute(StructureAttribute attribute)
    {
        AllAttributes.Add(attribute, Target);
        Attributes.Add(attribute);
    }

    public void UseAttribute(StructureAttribute attribute) => UsedAttributes.Add(attribute, Target);

    public static StructureParserContext CreateRoot() => new(
        Target: "",
        Attributes: new List<StructureAttribute>(),
        ParentAttributes: Array.Empty<(string Target, StructureAttribute Attribute)>(),
        AllAttributes: new Dictionary<StructureAttribute, string>(),
        UsedAttributes: new Dictionary<StructureAttribute, string>()
    );

    public StructureAttribute? GetStructureAttribute()
    {
        return Attributes
            .Where(x => string.IsNullOrEmpty(x.Target))
            .Concat(ParentAttributes.Where(pa => CombineTarget(pa.Target, pa.Attribute.Target) == Target).Select(pa => pa.Attribute))
            .SingleOrDefault();
    }

    public StructureParserContext Nested(string name)
    {
        return this with
        {
            Target = CombineTarget(Target, name),
            Attributes = new List<StructureAttribute>(),
            ParentAttributes = ParentAttributes.Concat(Attributes.Select(a => (Target, a))).ToArray(),
        };
    }

    private static string CombineTarget(string parent, string? child)
    {
        return string.IsNullOrEmpty(child) ? parent : $"{parent}.{child}";
    }

    public void Validate()
    {
        foreach (var (attribute, target) in AllAttributes)
        {
            if (!UsedAttributes.ContainsKey(attribute))
                throw new InvalidOperationException($"Attribute is not used. Define target: {target}; Attribute: {attribute}");
        }
    }
}
