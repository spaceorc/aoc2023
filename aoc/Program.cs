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

    static void Main_6_2()
    {
        const int count = 14;
        
        var input = File.ReadAllLines("day6.txt")[0];
        for (var i = 0; i < input.Length; i++)
        {
            if (input.Substring(i, count).ToHashSet().Count == count)
            {
                Console.WriteLine(i + count);
                return;
            }
        }
    }

    static void Main_6_1()
    {
        const int count = 4;
        
        var input = File.ReadAllLines("day6.txt")[0];
        for (var i = 0; i < input.Length; i++)
        {
            if (input.Substring(i, count).ToHashSet().Count == count)
            {
                Console.WriteLine(i + count);
                return;
            }
        }
    }

    static void Main_5_2()
    {
        var lines = File
            .ReadAllText("day5.txt")
            .Split("\n\n");

        var stacks = lines[0]
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .SkipLast(1)
            .RotateCW()
            .EveryNth(4, startFrom: 1)
            .Select(x => new Stack<char>(x.Where(c => c != ' ')))
            .ToArray();

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

        var stacks = lines[0]
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .SkipLast(1)
            .RotateCW()
            .EveryNth(4, startFrom: 1)
            .Select(x => new Stack<char>(x.Where(c => c != ' ')))
            .ToArray();

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
            .Select(x => new[] { new R(x[0], x[1]), new R(x[2], x[3]) })
            .Count(x => x[0].Overlaps(x[1])));
    }

    static void Main_4_1()
    {
        Console.WriteLine(File
            .ReadAllLines("day4.txt")
            .Select(x => x.Split('-', ',').Select(long.Parse).ToArray())
            .Select(x => new[] { new R(x[0], x[1]), new R(x[2], x[3]) })
            .Count(x => x[0].Contains(x[1]) || x[1].Contains(x[0])));
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