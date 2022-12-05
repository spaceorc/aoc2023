using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static System.Math;

namespace aoc;

public class Program
{
    static void Main()
    {
        Main_5_1();
        Main_5_2();
    }
    
    static void Main_5_2()
    {
        var lines = File
            .ReadAllText("day5.txt")
            .Split("\n\n");

        var stackLines = lines[0]
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .SkipLast(1)
            .Select(x => x.Batch(4).Select(c => c[1]).ToArray())
            .ToArray();

        var stackCount = stackLines[0].Length;
        
        var stacks = Enumerable
            .Range(0, stackCount)
            .Select(_ => new Stack<char>())
            .ToArray();

        for (var i = 0; i < stackCount; i++)
        {
            for (var k = stackLines.Length - 1; k >= 0; k--)
            {
                var towerLine = stackLines[k];
                if (towerLine[i] != ' ')
                    stacks[i].Push(towerLine[i]);
            }
        }
        
        var moves = lines[1]
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Split())
            .Select(x => (num: long.Parse(x[1]), from: long.Parse(x[3]), to: long.Parse(x[5])))
            .ToArray();

        foreach (var (num, from, to) in moves)
        {
            var tmp = new Stack<char>();
            for (var i = 0; i < num; i++)
                tmp.Push(stacks[from - 1].Pop());
            for (var i = 0; i < num; i++)
                stacks[to - 1].Push(tmp.Pop());
        }

        Console.WriteLine(new string(stacks.Select(x => x.First()).ToArray()));
    }

    static void Main_5_1()
    {
        var lines = File
            .ReadAllText("day5.txt")
            .Split("\n\n");

        var stackLines = lines[0]
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .SkipLast(1)
            .Select(x => x.Batch(4).Select(c => c[1]).ToArray())
            .ToArray();

        var stackCount = stackLines[0].Length;
        
        var stacks = Enumerable
            .Range(0, stackCount)
            .Select(_ => new Stack<char>())
            .ToArray();

        for (var i = 0; i < stackCount; i++)
        {
            for (var k = stackLines.Length - 1; k >= 0; k--)
            {
                var towerLine = stackLines[k];
                if (towerLine[i] != ' ')
                    stacks[i].Push(towerLine[i]);
            }
        }
        
        var moves = lines[1]
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Split())
            .Select(x => (num: long.Parse(x[1]), from: long.Parse(x[3]), to: long.Parse(x[5])))
            .ToArray();

        foreach (var (num, from, to) in moves)
            for (var i = 0; i < num; i++)
                stacks[to - 1].Push(stacks[from - 1].Pop());

        Console.WriteLine(new string(stacks.Select(x => x.First()).ToArray()));
    }

    static void Main_4_2()
    {
        Console.WriteLine(File
            .ReadAllLines("day4.txt")
            .Select(x => x.Split('-', ',').Select(long.Parse).ToArray())
            .Select(x => new[] { (begin: x[0], end: x[1]), (begin: x[2], end: x[3]) })
            .Count(x => x[0].begin <= x[1].end && x[0].end >= x[1].begin));
    }

    static void Main_4_1()
    {
        Console.WriteLine(File
            .ReadAllLines("day4.txt")
            .Select(x => x.Split('-', ',').Select(long.Parse).ToArray())
            .Select(x => new[] { (begin: x[0], end: x[1]), (begin: x[2], end: x[3]) })
            .Count(x => x[0].begin >= x[1].begin && x[0].end <= x[1].end
                        || x[1].begin >= x[0].begin && x[1].end <= x[0].end));
    }

    static void Main_3_2()
    {
        Console.Out.WriteLine(File
            .ReadAllLines("day3.txt")
            .Batch(3)
            .Select(b => b.Aggregate(
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray(),
                (x, y) => x.Intersect(y).ToArray()
            ).Single())
            .Select(c => c switch
            {
                >= 'a' and <= 'z' => c - 'a' + 1,
                >= 'A' and <= 'Z' => c - 'A' + 27,
                _ => throw new Exception()
            }).Sum());
    }

    static void Main_3_1()
    {
        Console.Out.WriteLine(File
            .ReadAllLines("day3.txt")
            .Select(x => new[] { x[..(x.Length / 2)], x[(x.Length / 2)..] })
            .Select(b => b.Aggregate(
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray(),
                (x, y) => x.Intersect(y).ToArray()
            ).Single())
            .Select(c => c switch
            {
                >= 'a' and <= 'z' => c - 'a' + 1,
                >= 'A' and <= 'Z' => c - 'A' + 27,
                _ => throw new Exception()
            }).Sum());
    }

    static void Main_2_2()
    {
        Console.Out.WriteLine(File
            .ReadAllLines("day2.txt")
            .Select(x => (v1: x[0] - 'A', outcome: x[2] - 'X'))
            .Select(x => (x.v1, v2: (x.v1 + x.outcome + 2) % 3))
            .Select(x => x.v2 + 1 + (x.v2 - x.v1 + 4) % 3 * 3)
            .Sum());
    }

    static void Main_2_1()
    {
        Console.Out.WriteLine(File
            .ReadAllLines("day2.txt")
            .Select(x => (v1: x[0] - 'A', v2: x[2] - 'X'))
            .Select(x => x.v2 + 1 + (x.v2 - x.v1 + 4) % 3 * 3)
            .Sum());
    }

    static void Main_1_2()
    {
        Console.Out.WriteLine(File
            .ReadAllText("day1.txt")
            .Split("\n\n", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(x => x.Split("\n").Select(long.Parse).ToArray().Sum())
            .OrderDescending()
            .Take(3)
            .Sum());
    }

    static void Main_1_1()
    {
        Console.Out.WriteLine(File
            .ReadAllText("day1.txt")
            .Split("\n\n", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(x => x.Split("\n").Select(long.Parse).ToArray().Sum())
            .Max());
    }
}