using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

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
            args.Add(parameter.ParameterType == typeof(string[])
                ? region
                : ParseParameterValue(parameter, parameter.ParameterType, region));
        }

        return args.ToArray();
    }

    public static object ParseParameterValue(ParameterInfo parameter, Type parameterType, string[] lines)
    {
        var template = parameter.GetCustomAttribute<TemplateAttribute>()?.Template;
        if (template != null)
            return ParseWithTemplate(parameterType, lines, template);

        if (parameterType.IsArray)
        {
            var separators = parameter.GetCustomAttribute<SplitAttribute>()?.Separators ?? "- ;,";
            var parseAllGeneric =
                typeof(Parser).GetMethod(nameof(ParseAll), BindingFlags.Public | BindingFlags.Static)!;
            var parseAll = parseAllGeneric.MakeGenericMethod(parameterType.GetElementType()!);
            return parseAll.Invoke(null, new object?[] { lines, separators })!;
        }

        if (parameterType.IsGenericType &&
            parameterType.GetGenericTypeDefinition() == typeof(Map<>))
        {
            var toMapGeneric =
                typeof(Helpers).GetMethod(nameof(Helpers.ToMap), BindingFlags.Public | BindingFlags.Static,
                    new[] { typeof(IEnumerable<string>) })!;
            var toMap = toMapGeneric.MakeGenericMethod(parameterType.GetGenericArguments()[0]);
            return toMap.Invoke(null, new object?[] { lines })!;
        }

        throw new Exception($"Unsupported parameter type: {parameter}");
    }

    public static object ParseWithTemplate(Type type, string[] lines, string template)
    {
        return type.IsArray
            ? lines.ParseAllWithTemplate(type, template)
            : lines.ParseRegionWithTemplate(type, template);
    }

    public static T[] ParseAllWithTemplate<T>(this IEnumerable<string> lines, string template)
    {
        return lines.Select(line => new[] { line }.ParseRegionWithTemplate<T>(template)).ToArray();
    }

    public static object ParseAllWithTemplate(this IEnumerable<string> lines, Type type, string template)
    {
        var linesArr = lines as IList<string> ?? lines.ToArray();
        var result = Array.CreateInstance(type, linesArr.Count);

        for (int i = 0; i < linesArr.Count; i++)
            result.SetValue(new[] { linesArr[i] }.ParseRegionWithTemplate(type, template), i);

        return result;
    }

    public static T ParseRegionWithTemplate<T>(this IEnumerable<string> lines, string template)
    {
        return (T)lines.ParseRegionWithTemplate(typeof(T), template);
    }

    public static object ParseRegionWithTemplate(this IEnumerable<string> lines, Type type, string template)
    {
        var region = string.Join("\n", lines);
        var regex = new Regex(@"([\\$^()[\]:+*?])|({[^}]+})", RegexOptions.Compiled | RegexOptions.Singleline);
        var templateRegexString = "^" + regex.Replace(template, m =>
        {
            if (m.Value.Length == 1)
                return "\\" + m.Value;

            return $"(?<{m.Value.Substring(1, m.Value.Length - 2)}>.*)";
        }) + "$";
        var templateRegex = new Regex(templateRegexString, RegexOptions.Compiled | RegexOptions.Singleline);
        var match = templateRegex.Match(region);
        if (!match.Success)
            throw new Exception("Template doesn't match");

        if (type == typeof(long))
            return long.Parse(match.Groups["Value"].Value);
        if (type == typeof(int))
            return int.Parse(match.Groups["Value"].Value);
        if (type == typeof(string))
            return match.Groups["Value"].Value;
        if (type == typeof(char))
        {
            var value = match.Groups["Value"].Value;
            if (value.Length != 1)
                throw new InvalidOperationException($"Invalid char {value}");
            return value[0];
        }

        var constructor = type.GetConstructors().Single(x => x.GetParameters().Any());
        var parameters = new List<object>();
        foreach (var parameter in constructor.GetParameters())
        {
            var value = match.Groups[parameter.Name!].Value;
            var separators = parameter.GetCustomAttribute<SplitAttribute>()?.Separators ?? "- ;,";
            parameters.Add(value.Parse(parameter.ParameterType, separators));
        }

        return constructor.Invoke(parameters.ToArray());
    }

    public static T[] ParseAll<T>(this IEnumerable<string> lines, string separators = "- ;,")
    {
        return lines.Select(x => Parse<T>(x, separators)).ToArray();
    }

    public static T Parse<T>(this string line, string separators = "- ;,")
    {
        return (T)line.Parse(typeof(T), separators);
    }

    public static object Parse(this string line, Type type, string separators = "- ;,")
    {
        if (type == typeof(string))
            return line;
        var source = new Queue<string>(line.Split(separators.ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
        return ReadFrom(type, source);
    }

    private static object ReadFrom(Type type, Queue<string> source)
    {
        if (type == typeof(long))
            return long.Parse(source.Dequeue());
        if (type == typeof(int))
            return int.Parse(source.Dequeue());
        if (type == typeof(string))
            return source.Dequeue();
        if (type == typeof(char))
        {
            var value = source.Dequeue();
            if (value.Length != 1)
                throw new InvalidOperationException($"Invalid char {value}");
            return value[0];
        }

        if (type.IsArray)
        {
            var result = Array.CreateInstance(type.GetElementType()!, source.Count);
            for (int i = 0; i < result.Length; i++)
                result.SetValue(ReadFrom(type.GetElementType()!, source), i);
            return result;
        }

        var constructor = type.GetConstructors().Single(x => x.GetParameters().Any());
        var parameters = new List<object>();
        foreach (var parameter in constructor.GetParameters())
            parameters.Add(ReadFrom(parameter.ParameterType, source));
        return constructor.Invoke(parameters.ToArray());
    }
}