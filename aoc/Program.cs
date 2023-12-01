using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using static System.Math;

namespace aoc;

public class Program
{
    static void Main()
    {
        Runner.RunFile("day1.txt", Solve_1);
    }

    static void Solve_1(string[] input)
    {
        input
            .Select(l => string.Join("", l.Where(char.IsDigit)))
            .Select(l => $"{l[0]}{l[^1]}")
            .Select(long.Parse)
            .ToArray()
            .Sum()
            .Out("Part 1: ");

        var digits = new[] { "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };
        var digits2 = new[] { "1", "2", "3", "4", "5", "6", "7", "8", "9" };
        input
            .Select(l =>
            {
                var first = digits.Select((d, i) => (d, i))
                    .Concat(digits2.Select((d, i) => (d, i)))
                    .Select(x => (num: x.i + 1, s: x.d, ind: l.IndexOf(x.d)))
                    .Select(x => x with { ind = x.ind == -1 ? int.MaxValue : x.ind })
                    .MinBy(x => x.ind)
                    .num;
                var last = digits.Select((d, i) => (d, i))
                    .Concat(digits2.Select((d, i) => (d, i)))
                    .Select(x => (num: x.i + 1, s: x.d, ind: l.LastIndexOf(x.d)))
                    .MaxBy(x => x.ind)
                    .num;
                return first * 10 + last;
            })
            .Sum()
            .Out("Part 2: ");
    }
}