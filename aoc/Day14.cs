using System;
using System.Collections.Generic;
using System.Linq;

namespace aoc;

public static class Day14
{
    public static string[] Transpose(string[] input)
    {
        return Enumerable
            .Range(0, input[0].Length)
            .Select(col => new string(input.Select(line => line[col]).ToArray()))
            .ToArray();
    }

    public static string[] RotateCCW(string[] input)
    {
        /*
         *  +-----X-+    +-------+
         *  |       | => X       |
         *  |       |    |       |
         *  +-------+    +-------+
         */
        return Transpose(
            input
                .Select(line => new string(line.Reverse().ToArray()))
                .ToArray()
        );
    }

    public static string[] RotateCCW(string[] input, long count)
    {
        for (int i = 0; i < count; i++)
            input = RotateCCW(input);

        return input;
    }

    public static string[] RotateCW(string[] input)
    {
        return RotateCCW(input, 3);
    }

    public static string[] RotateCW(string[] input, long count)
    {
        for (int i = 0; i < count; i++)
            input = RotateCW(input);

        return input;
    }

    public static string FallPartLeft(string line)
    {
        return new string('O', line.Count(c => c == 'O')) + new string('.', line.Count(c => c == '.'));
    }

    public static string FallLineLeft(string line)
    {
        return string.Join(
            '#',
            line.Split('#').Select(FallPartLeft)
        );
    }

    public static string[] FallLeft(string[] input)
    {
        return input.Select(FallLineLeft).ToArray();
    }

    public static string[] FallW(string[] input)
    {
        return FallLeft(input);
    }

    public static string[] FallN(string[] input)
    {
        return RotateCW(FallLeft(RotateCCW(input)));
    }

    public static string[] FallS(string[] input)
    {
        return RotateCCW(FallLeft(RotateCW(input)));
    }

    public static string[] FallE(string[] input)
    {
        return RotateCCW(RotateCCW(FallLeft(RotateCW(RotateCW(input)))));
    }

    public static long CalcLoad(string[] input)
    {
        var result = 0L;
        for (int i = 0; i < input.Length; i++)
            result += input[i].Count(c => c == 'O') * (input.Length - i);
        return result;
    }

    public static int GetHash(string[] input)
    {
        return input.Aggregate(0, (current, line) => HashCode.Combine(current, line.GetHashCode()));
    }

    public static void Dump(string[] input)
    {
        foreach (var line in input)
            Console.WriteLine(line);
    }

    public static void SolvePart1(string[] input)
    {
        input = FallN(input);
        // Dump(input);
        Console.WriteLine($"Part 1 = {CalcLoad(input)}");
    }

    public static void SolvePart2(string[] input)
    {
        const long count = 1000000000L;

        var iterationByHash = new Dictionary<int, int>();
        var resultByIteration = new Dictionary<long, long>();
        for (int i = 0; i < count; i++)
        {
            // if (i % 1000 == 0)
            //     Console.WriteLine($"i = {i}");
            // Console.WriteLine(GetHash(input));

            var hash = GetHash(input);
            if (iterationByHash.TryGetValue(hash, out var prev))
            {
                var cycleSize = i - prev;
                var iterationsLeft = count - i;
                iterationsLeft %= cycleSize;
                var resultIteration = prev + iterationsLeft;
                Console.WriteLine($"Part 2 = {resultByIteration[resultIteration]}");
                return;
            }

            iterationByHash.Add(hash, i);
            resultByIteration.Add(i, CalcLoad(input));

            input = FallN(input);
            input = FallW(input);
            input = FallS(input);
            input = FallE(input);
        }

        Console.WriteLine($"Part 2 = {CalcLoad(input)}");
    }
}