using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace aoc.ParseLib;

public record AtomStructure(Type Type, string Separators = StructureParser.DefaultSeparators) : Structure(Type)
{
    public override object CreateObject(string source)
    {
        if (Type == typeof(long))
            return long.Parse(source);
        if (Type == typeof(int))
            return int.Parse(source);
        if (Type == typeof(string))
            return source;
        if (Type == typeof(char))
        {
            if (source.Length != 1)
                throw new InvalidOperationException($"Invalid char {source}");
            return source[0];
        }

        var typeParseMethod = Type.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, new[] { typeof(string) });
        if (typeParseMethod != null)
            return typeParseMethod.Invoke(null, new object?[] { source })!;

        var queue = new Queue<string>(
            source
                .Split(Separators.ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrEmpty(x))
        );
        var result = ReadFrom(Type, queue);
        if (queue.Count > 0)
            throw new InvalidOperationException($"Not all input parsed. Remaining parts: [{string.Join(", ", queue)}]");

        return result;
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

        var typeParseMethod = type.GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, new[] { typeof(string) });
        if (typeParseMethod != null)
            return typeParseMethod.Invoke(null, new object?[] { source.Dequeue() })!;

        if (type.IsArray)
        {
            var list = new ArrayList();
            while (source.Count != 0)
                list.Add(ReadFrom(type.GetElementType()!, source));
            var result = Array.CreateInstance(type.GetElementType()!, list.Count);
            for (var i = 0; i < list.Count; i++)
                result.SetValue(list[i], i);
            return result;
        }

        var constructor = type.GetConstructors().Single(x => x.GetParameters().Length != 0);
        var parameters = constructor
            .GetParameters()
            .Select(parameter => ReadFrom(parameter.ParameterType, source))
            .ToArray();
        return constructor.Invoke(parameters);
    }
}
