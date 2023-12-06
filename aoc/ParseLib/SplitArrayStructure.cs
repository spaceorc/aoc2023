using System;
using System.Linq;

namespace aoc.ParseLib;

public record SplitArrayStructure(Type Type, string Separators, TypeStructure Item) : TypeStructure(Type)
{
    public override object CreateObject(string source)
    {
        var items = source
            .Split(Separators.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim())
            .Where(x => !string.IsNullOrEmpty(x))
            .ToArray();

        var result = Array.CreateInstance(Type.GetElementType()!, items.Length);
        for (var i = 0; i < result.Length; i++)
            result.SetValue(Item.CreateObject(items[i]), i);
        return result;
    }
}
