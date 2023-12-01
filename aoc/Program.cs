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
        long Solve(bool isPart2)
        {
            var replacements1 = Enumerable
                .Range(1, 9)
                .Select(digit => (str: digit.ToString(), digit));
            
            var replacements2 = new[] { "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" }
                .Select((str, i) => (str, digit: i + 1));
            
            var replacements = isPart2 ? replacements1.Concat(replacements2) : replacements1;
            return input
                .Select(line => (
                        first: replacements.Select(x => (x.digit, p: line.IndexOf(x.str))).Where(x => x.p != -1).MinBy(r => r.p).digit,
                        last: replacements.Select(x => (x.digit, p: line.LastIndexOf(x.str))).Where(x => x.p != -1).MaxBy(r => r.p).digit
                    )
                )
                .Select(x => x.first * 10 + x.last)
                .Sum();
        }

        Solve(isPart2: false).Out("Part 1: ");
        Solve(isPart2: true).Out("Part 2: ");
    }
}