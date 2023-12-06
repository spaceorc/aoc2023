using System;
using System.Diagnostics;
using System.Linq;

namespace aoc.ParseLib;

public record SplitClassStructure(Type Type, string Separators, params TypeStructure[] Parameters) : TypeStructure(Type)
{
    public override object CreateObject(string source)
    {
        var items = source
            .Split(Separators.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim())
            .Where(x => !string.IsNullOrEmpty(x))
            .ToArray();

        var parameters = new object?[Parameters.Length];
        if (parameters.Length != items.Length)
            throw new InvalidOperationException($"Invalid input values count. Expected count: {parameters.Length}. Actual count: {items.Length}. Items: [{string.Join(", ", items)}]");

        var constructor = Type.GetConstructors().Single(x => x.GetParameters().Length != 0);
        Debug.Assert(constructor.GetParameters().Length == Parameters.Length);
        for (var i = 0; i < constructor.GetParameters().Length; i++)
            parameters[i] = Parameters[i].CreateObject(items[i]);

        return constructor.Invoke(parameters.ToArray());
    }
}