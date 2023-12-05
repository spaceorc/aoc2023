using System;
using System.IO;
using System.Reflection.Emit;

namespace aoc.ParseLib;

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
        Invoke(solve, Parser.ParseMethodParameterValues(solve.Method, source));
    }

    private static void Invoke(Delegate solve, object?[] args)
    {
        var parameters = solve.Method.GetParameters();
        var dynamicMethod = new DynamicMethod(
            Guid.NewGuid().ToString(),
            typeof(void),
            new[] { typeof(object?[]) },
            typeof(Runner),
            skipVisibility: true
        );
        var il = dynamicMethod.GetILGenerator();
        for (var i = 0; i < parameters.Length; i++)
        {
            var parameter = parameters[i];
            il.Emit(OpCodes.Ldarg_0); // [args]
            il.Emit(OpCodes.Ldc_I4, i); // [args, i]
            il.Emit(OpCodes.Ldelem, typeof(object)); // [args[i]]
            if (!parameter.ParameterType.IsValueType)
                il.Emit(OpCodes.Castclass, parameter.ParameterType); // [(ParameterType)args[i]]
            else
                il.Emit(OpCodes.Unbox_Any, parameter.ParameterType); // [(ParameterType)args[i]]
        }
        // [*args]
        il.Emit(OpCodes.Call, solve.Method); // []
        il.Emit(OpCodes.Ret); // []
        
        dynamicMethod.CreateDelegate<Action<object?[]>>()(args);
    }
}