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
        Main_2_1();
        Main_2_2();
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