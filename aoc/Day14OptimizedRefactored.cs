using System;
using System.Collections.Generic;

namespace aoc;

public static unsafe class Day14OptimizedRefactored
{
    public struct LinesIterator
    {
        public int sizeX, sizeY, x, y, posDx, posDy, lineDx, lineDy;

        public LinesIterator(int sizeX, int sizeY, int x, int y, int posDx, int posDy, int lineDx, int lineDy)
        {
            this.sizeX = sizeX;
            this.sizeY = sizeY;
            this.x = x;
            this.y = y;
            this.posDx = posDx;
            this.posDy = posDy;
            this.lineDx = lineDx;
            this.lineDy = lineDy;
        }

        public bool MoveNextPos()
        {
            x += posDx;
            y += posDy;
            return x >= 0 && y >= 0 && x < sizeX && y < sizeY;
        }

        public bool MoveNextLine()
        {
            x = (x + sizeX) % sizeX;
            y = (y + sizeY) % sizeY;
            x += lineDx;
            y += lineDy;
            return x >= 0 && y >= 0 && x < sizeX && y < sizeY;
        }

        public static LinesIterator Columns(char[,] map) => new(map.GetLength(0), map.GetLength(1), x: 0, y: 0, posDx: 0, posDy: 1, lineDx: 1, lineDy: 0);
        public static LinesIterator ReversedColumns(char[,] map) => new(map.GetLength(0), map.GetLength(1), x: 0, y: map.GetLength(1) - 1, posDx: 0, posDy: -1, lineDx: 1, lineDy: 0);
        public static LinesIterator Rows(char[,] map) => new(map.GetLength(0), map.GetLength(1), x: 0, y: 0, posDx: 1, posDy: 0, lineDx: 0, lineDy: 1);
        public static LinesIterator ReversedRows(char[,] map) => new(map.GetLength(0), map.GetLength(1), x: map.GetLength(0) - 1, y: 0, posDx: -1, posDy: 0, lineDx: 0, lineDy: 1);
    }

    public static void Fall(char[,] map, LinesIterator iterator)
    {
        do
        {
            var start = iterator;
            do
            {
                if (map[iterator.x, iterator.y] == '#')
                {
                    start = iterator;
                    start.MoveNextPos();
                }
                else if (map[iterator.x, iterator.y] == 'O')
                {
                    (map[iterator.x, iterator.y], map[start.x, start.y]) = (map[start.x, start.y], map[iterator.x, iterator.y]);
                    start.MoveNextPos();
                }
            } while (iterator.MoveNextPos());
        } while (iterator.MoveNextLine());
    }

    public static void FallN(char[,] map) => Fall(map, LinesIterator.Columns(map));
    public static void FallS(char[,] map) => Fall(map, LinesIterator.ReversedColumns(map));
    public static void FallW(char[,] map) => Fall(map, LinesIterator.Rows(map));
    public static void FallE(char[,] map) => Fall(map, LinesIterator.ReversedRows(map));

    public static long CalcLoad(char[,] map)
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

    public static int GetHash(char[,] map)
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
        Console.WriteLine($"Part 1 Optimized Refactored = {CalcLoad(map)}");
    }

    public static void SolvePart2(string[] input)
    {
        const long count = 1000000000L;

        var map = ToMap(input);

        var iterationByHash = new Dictionary<int, int>();
        var resultByIteration = new Dictionary<long, long>();
        for (int i = 0; i < count; i++)
        {
            var hash = GetHash(map);
            if (iterationByHash.TryGetValue(hash, out var prev))
            {
                var cycleSize = i - prev;
                var iterationsLeft = count - i;
                iterationsLeft %= cycleSize;
                var resultIteration = prev + iterationsLeft;
                Console.WriteLine($"Part 2 Optimized Refactored = {resultByIteration[resultIteration]}");
                return;
            }

            iterationByHash.Add(hash, i);
            resultByIteration.Add(i, CalcLoad(map));

            FallN(map);
            FallW(map);
            FallS(map);
            FallE(map);
        }

        Console.WriteLine($"Part 2 Optimized Refactored = {CalcLoad(map)}");
    }
}
