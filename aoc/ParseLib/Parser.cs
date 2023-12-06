using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace aoc.ParseLib;

public static class Parser
{
    public static object?[] ParseMethodParameterValues(MethodInfo method, string[] lines)
    {
        var parameters = method.GetParameters();
        if (parameters.Length == 1 
            && parameters[0].ParameterType == typeof(string[]) 
            && !parameters[0].GetCustomAttributes().OfType<StructureAttribute>().Any()
            && !method.GetCustomAttributes().OfType<StructureAttribute>().Any())
            return new object?[] { lines };

        if (method.GetCustomAttributes().OfType<StructureAttribute>().Any())
        {
            var tupleType = CreateTupleType(method.GetParameters().Select(p => p.ParameterType).ToArray());
            var value = ParseParameterValue(method, tupleType, lines);
            return method.GetParameters().Select((_, i) => tupleType.GetProperty($"Item{i + 1}")!.GetValue(value)).ToArray();
        }

        var regions = lines.Regions();
        if (parameters.All(p => p.GetCustomAttribute<ParamArrayAttribute>() == null) &&
            parameters.Length != regions.Count)
            throw new Exception($"Input regions count {regions.Count}, but parameters count {parameters.Length}");

        var args = new List<object?>();

        for (var i = 0; i < parameters.Length; i++)
        {
            var parameter = parameters[i];
            if (parameter.GetCustomAttribute<ParamArrayAttribute>() != null)
            {
                var restRegions = regions.Skip(i).ToArray();

                var regionItemType = parameter.ParameterType.GetElementType()!;
                if (regionItemType == typeof(string[]))
                {
                    args.Add(restRegions);
                    break;
                }

                var arg = Array.CreateInstance(regionItemType, restRegions.Length);
                for (var r = 0; r < restRegions.Length; r++)
                    arg.SetValue(ParseParameterValue(parameter, regionItemType, restRegions[r]), r);

                args.Add(arg);
                break;
            }

            var region = regions[i];
            args.Add(
                parameter.ParameterType == typeof(string[])
                    ? region
                    : ParseParameterValue(parameter, parameter.ParameterType, region)
            );
        }

        return args.ToArray();
    }

    private static Type CreateTupleType(Type[] types)
    {
        var genericType = typeof(Tuple<>)
            .Assembly
            .GetTypes()
            .Single(t => t.Name == $"Tuple`{types.Length}");
        return genericType.MakeGenericType(types);
    }

    private static object ParseParameterValue(ICustomAttributeProvider customAttributeProvider, Type parameterType, string[] lines)
    {
        if (parameterType.IsArray && !customAttributeProvider.GetCustomAttributes(false).OfType<NonArrayAttribute>().Any())
        {
            var itemStructure = StructureParser.Parse(parameterType.GetElementType()!, customAttributeProvider);
            var parseAllGeneric = typeof(Parser).GetMethod(nameof(ParseAll), BindingFlags.NonPublic | BindingFlags.Static);
            var parseAll = parseAllGeneric!.MakeGenericMethod(parameterType.GetElementType()!);
            return parseAll.Invoke(null, new object?[] { itemStructure, lines })!;
        }

        var structure = StructureParser.Parse(parameterType, customAttributeProvider);
        var parseGeneric = typeof(Parser).GetMethod(nameof(Parse), BindingFlags.NonPublic | BindingFlags.Static);
        var parse = parseGeneric!.MakeGenericMethod(parameterType);
        return parse.Invoke(null, new object?[] { structure, string.Join('\n', lines) })!;
    }

    private static T[] ParseAll<T>(Structure structure, IEnumerable<string> lines)
    {
        return lines.Select(x => Parse<T>(structure, x)).ToArray();
    }

    private static T Parse<T>(Structure structure, string line)
    {
        return (T)structure.CreateObject(line);
    }
}
