using System;
using System.Collections.Generic;
using System.Linq;

namespace aoc;

public static unsafe class Day14Optimized
{
    public static void FallN(char[,] map)
    {
        var sizeX = map.GetLength(0);
        var sizeY = map.GetLength(1);

        for (int x = 0; x < sizeX; x++)
        {
            var start = 0;
            for (int y = 0; y < sizeY; y++)
            {
                if (map[x, y] == '#')
                    start = y + 1;
                else if (map[x, y] == 'O')
                {
                    (map[x, y], map[x, start]) = (map[x, start], map[x, y]);
                    start++;
                }
            }
        }
    }

    public static void FallW(char[,] map)
    {
        var sizeX = map.GetLength(0);
        var sizeY = map.GetLength(1);

        for (int y = 0; y < sizeY; y++)
        {
            var start = 0;
            for (int x = 0; x < sizeX; x++)
            {
                if (map[x, y] == '#')
                    start = x + 1;
                else if (map[x, y] == 'O')
                {
                    (map[x, y], map[start, y]) = (map[start, y], map[x, y]);
                    start++;
                }
            }
        }
    }

    public static void FallE(char[,] map)
    {
        var sizeX = map.GetLength(0);
        var sizeY = map.GetLength(1);
        
        for (int y = 0; y < sizeY; y++)
        {
            var start = sizeX - 1;
            for (int x = sizeX - 1; x >= 0; x--)
            {
                if (map[x, y] == '#')
                    start = x - 1;
                else if (map[x, y] == 'O')
                {
                    (map[x, y], map[start, y]) = (map[start, y], map[x, y]);
                    start--;
                }
            }
        }
    }

    public static void FallS(char[,] map)
    {
        var sizeX = map.GetLength(0);
        var sizeY = map.GetLength(1);
        
        for (int x = 0; x < sizeX; x++)
        {
            var start = sizeY - 1;
            for (int y = sizeY - 1; y >= 0; y--)
            {
                if (map[x, y] == '#')
                    start = y - 1;
                else if (map[x, y] == 'O')
                {
                    (map[x, y], map[x, start]) = (map[x, start], map[x, y]);
                    start--;
                }
            }
        }
    }

    public static long CalcLoad(char[,] map)
    {
        return CalcLoad_Fixed(map);
    }

    public static long CalcLoad_Fixed(char[,] map)
    {
        var result = 0;

        var sizeX = map.GetLength(0);
        var sizeY = map.GetLength(1);

        for (int x = 0; x < sizeX; x++)
        for (int y = 0; y < sizeY; y++)
        {
            if (map[x, y] == 'O')
                result += sizeY - y;
        }

        return result;
    }

    public static long CalcLoad_Unfixed(char[,] map)
    {
        var input = ToLines(map);
        var result = 0L;
        for (int i = 0; i < input.Length; i++)
            result += input[i].Count(c => c == 'O') * (input.Length - i);
        return result;
    }

    public static int GetHash(char[,] map)
    {
        return GetHash_Fixed2(map);
    }

    public static int GetHash_Fixed2(char[,] map)
    {
        var mapLength = map.Length;
        var count = mapLength / 4;
        var rest = mapLength % 4;
        var result = 0;
        fixed (char* mapCharPtr = &map[0, 0])
        {
            var mapPtr = (int*)mapCharPtr;
            for (var i = 0; i < count; ++i, ++mapPtr)
                result = HashCode.Combine(result, *mapPtr);
            var mapRestPtr = (char*)mapPtr;
            for (int i = 0; i < rest; ++i, ++mapRestPtr)
                result = HashCode.Combine(result, (*mapRestPtr).GetHashCode());
        }

        return result;
    }

    public static int GetHash_Fixed(char[,] map)
    {
        var result = 0;

        var sizeX = map.GetLength(0);
        var sizeY = map.GetLength(1);

        for (int x = 0; x < sizeX; x++)
        for (int y = 0; y < sizeY; y++)
            result = HashCode.Combine(result, map[x, y].GetHashCode());

        return result;
    }

    public static int GetHash_Unfixed(char[,] map)
    {
        var input = ToLines(map);
        return input.Aggregate(0, (current, line) => HashCode.Combine(current, line.GetHashCode()));
    }

    public static void Dump(char[,] map)
    {
        var input = ToLines(map);
        foreach (var line in input)
            Console.WriteLine(line);
    }

    public static string[] ToLines(char[,] map)
    {
        var result = new List<string>();
        for (int y = 0; y < map.GetLength(1); y++)
        {
            var line = new string(Enumerable.Range(0, map.GetLength(0)).Select(x => map[x, y]).ToArray());
            result.Add(line);
        }

        return result.ToArray();
    }

    public static char[,] ToMap(string[] input)
    {
        var result = new char[input[0].Length, input.Length];
        for (int x = 0; x < input[0].Length; x++)
        for (int y = 0; y < input.Length; y++)
            result[x, y] = input[y][x];

        return result;
    }

    public static void SolvePart1(string[] input)
    {
        var map = ToMap(input);
        FallN(map);
        // Dump(map);
        Console.WriteLine($"Part 1 Fixed = {CalcLoad(map)}");
    }

    public static void SolvePart2(string[] input)
    {
        const long count = 1000000000L;

        var map = ToMap(input);

        var iterationByHash = new Dictionary<int, int>();
        var resultByIteration = new Dictionary<long, long>();
        for (int i = 0; i < count; i++)
        {
            // if (i % 1000 == 0)
            //     Console.WriteLine($"i = {i}");
            // Console.WriteLine(GetHash(map));

            var hash = GetHash(map);
            if (iterationByHash.TryGetValue(hash, out var prev))
            {
                var cycleSize = i - prev;
                var iterationsLeft = count - i;
                iterationsLeft %= cycleSize;
                var resultIteration = prev + iterationsLeft;
                Console.WriteLine($"Part 2 Fixed = {resultByIteration[resultIteration]}");
                return;
            }

            iterationByHash.Add(hash, i);
            resultByIteration.Add(i, CalcLoad(map));

            FallN(map);
            FallW(map);
            FallS(map);
            FallE(map);
        }

        Console.WriteLine($"Part 2 Fixed = {CalcLoad(map)}");
    }
}