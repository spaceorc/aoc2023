using System;
using System.Collections.Generic;
using System.Linq;
using aoc.ParseLib.Attributes;

namespace aoc.ParseLib.Structures;

public record TypeStructureParserContext(
    string Target,
    List<StructureAttribute> Attributes,
    List<(string Target, StructureAttribute Attribute)> ParentAttributes,
    Dictionary<StructureAttribute, string> AllAttributes,
    Dictionary<StructureAttribute, string> UsedAttributes
)
{
    public void DeclareAttribute(StructureAttribute attribute)
    {
        AllAttributes.Add(attribute, Target);
        Attributes.Add(attribute);
    }

    public void DeclareParentAttribute(string child, StructureAttribute attribute)
    {
        var target = CombineTarget(Target, child);
        AllAttributes.Add(attribute, target);
        ParentAttributes.Add((target, attribute));
    }

    public void UseAttribute(StructureAttribute attribute) => UsedAttributes.Add(attribute, Target);

    public static TypeStructureParserContext CreateRoot() => new(
        Target: "",
        Attributes: new List<StructureAttribute>(),
        ParentAttributes: new List<(string Target, StructureAttribute Attribute)>(), 
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

    public TypeStructureParserContext Nested(string name)
    {
        return this with
        {
            Target = CombineTarget(Target, name),
            Attributes = new List<StructureAttribute>(),
            ParentAttributes = ParentAttributes.Concat(Attributes.Select(a => (Target, a))).ToList(),
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
