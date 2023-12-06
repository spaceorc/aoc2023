using System;
using System.Linq;
using System.Reflection;

namespace aoc.ParseLib;

public record MethodStructure(MethodInfo Method, TypeStructure Parameters)
{
    public static MethodStructure CreateStructure(MethodInfo methodInfo)
    {
        var parameterInfos = methodInfo.GetParameters();
        var parameterTypes = parameterInfos.Select(p => p.ParameterType).ToArray();
        var genericType = typeof(Tuple<>)
            .Assembly
            .GetTypes()
            .Single(t => t.Name == $"Tuple`{parameterTypes.Length}");
        var tupleType = genericType.MakeGenericType(parameterTypes);
        var context = StructureParserContext.CreateRoot();
        for (var i = 0; i < parameterInfos.Length; i++)
        {
            foreach (var attribute in StructureParser.GetStructureAttributes(parameterInfos[i]))
                context.DeclareParentAttribute($"{i + 1}", attribute);
        }

        var parametersStructure = StructureParser.Parse(tupleType, methodInfo, context);
        context.Validate();
        return new MethodStructure(methodInfo, parametersStructure);
    }
    
    public object?[] CreateParameters(string source)
    {
        var result = Parameters.CreateObject(source);
        var parameters = new object?[Method.GetParameters().Length];
        for (var i = 0; i < parameters.Length; i++)
            parameters[i] = result.GetType().GetProperty($"Item{i + 1}")!.GetValue(result);

        return parameters;
    }
}
