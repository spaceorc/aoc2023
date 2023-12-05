using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using aoc.ParseLib;

namespace aoc;

public static class Parser
{
    public static object?[] ParseMethodParameterValues(MethodInfo method, string[] lines)
    {
        var parameters = method.GetParameters();
        if (parameters.Length == 1 && parameters[0].ParameterType == typeof(string[]))
            return new object?[] { lines };

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

    public static object ParseParameterValue(ParameterInfo parameter, Type parameterType, string[] lines)
    {
        if (parameterType.IsArray)
        {
            var itemStructure = StructureParser.Parse(parameterType.GetElementType()!, parameter);
            var parseAllGeneric = typeof(Parser).GetMethod(nameof(ParseAll), BindingFlags.Public | BindingFlags.Static);
            var parseAll = parseAllGeneric!.MakeGenericMethod(parameterType.GetElementType()!);
            return parseAll.Invoke(null, new object?[] { itemStructure, lines })!;
        }
        
        var structure = StructureParser.Parse(parameterType, parameter);
        var parseGeneric = typeof(Parser).GetMethod(nameof(Parse), BindingFlags.Public | BindingFlags.Static);
        var parse = parseGeneric!.MakeGenericMethod(parameterType);
        return parse.Invoke(null, new object?[] { structure, string.Join('\n', lines) })!;
    }

    public static T[] ParseAll<T>(Structure structure, IEnumerable<string> lines)
    {
        return lines.Select(x => Parse<T>(structure, x)).ToArray();
    }

    public static T Parse<T>(Structure structure, string line)
    {
        return (T)structure.CreateObject(line);
    }
}
