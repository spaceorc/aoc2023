using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace aoc;

public static class Runner
{
    public static void RunFile(string fileName, Delegate solve)
    {
        Run(File.ReadAllLines(fileName), solve);
    }

    public static void RunString(string source, Delegate solve)
    {
        Run(source.Split('\n'), solve);
    }

    public static void Run(string[] source, Delegate solve)
    {
        var parameters = solve.Method.GetParameters();
        if (parameters.Length == 1 && parameters[0].ParameterType == typeof(string[]))
        {
            solve.DynamicInvoke(new object?[] { source });
            return;
        }

        var regions = source.Regions();
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
                var separators = parameter.GetCustomAttribute<SplitAttribute>()?.Separators ?? "- ;,";
                var parseAllGeneric =
                    typeof(Parser).GetMethod(nameof(Parser.ParseAll), BindingFlags.Public | BindingFlags.Static)!;

                var regionItemArrayType = parameter.ParameterType.GetElementType()!;
                if (!regionItemArrayType.IsArray)
                    throw new Exception("Invalid param array element type - must be array");

                var parseAll = parseAllGeneric.MakeGenericMethod(regionItemArrayType.GetElementType()!);

                var arg = Array.CreateInstance(regionItemArrayType, restRegions.Length);
                for (var r = 0; r < restRegions.Length; r++)
                    arg.SetValue(parseAll.Invoke(null, new object?[] { restRegions[r], separators }), r);

                args.Add(arg);
                break;
            }

            var region = regions[i];
            if (parameter.ParameterType == typeof(string[]))
                args.Add(region);
            else if (parameter.ParameterType.IsArray)
            {
                var separators = parameter.GetCustomAttribute<SplitAttribute>()?.Separators ?? "- ;,";
                var parseAllGeneric =
                    typeof(Parser).GetMethod(nameof(Parser.ParseAll), BindingFlags.Public | BindingFlags.Static)!;
                var parseAll = parseAllGeneric.MakeGenericMethod(parameter.ParameterType.GetElementType()!);
                args.Add(parseAll.Invoke(null, new object?[] { region, separators }));
            }
            else if (parameter.ParameterType.IsGenericType &&
                     parameter.ParameterType.GetGenericTypeDefinition() == typeof(Map<>))
            {
                var toMapGeneric =
                    typeof(Helpers).GetMethod(nameof(Helpers.ToMap), BindingFlags.Public | BindingFlags.Static,
                        new[] { typeof(IEnumerable<string>) })!;
                var toMap = toMapGeneric.MakeGenericMethod(parameter.ParameterType.GetGenericArguments()[0]);
                args.Add(toMap.Invoke(null, new object?[] { region }));
            }
        }

        solve.DynamicInvoke(args.ToArray());
    }
}