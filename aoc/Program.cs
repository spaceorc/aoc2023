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
        Runner.RunFile("day2.txt", Solve_2);
    }


    public record GameDay2Set(long R, long G, long B)
    {
        public static GameDay2Set Parse(string source)
        {
            var r = source.Trim()
                .Split(", ")
                .Select(x => x.Split())
                .ToDictionary(x => x[1], x => long.Parse(x[0]));
            return new GameDay2Set(
                r.GetValueOrDefault("red", 0),
                r.GetValueOrDefault("green", 0),
                r.GetValueOrDefault("blue", 0)
            );
        }
    };

    public record GameDay2(long Id, [Split(";")] GameDay2Set[] Sets);

    static void Solve_2([Template("Game {Id}: {Sets}")] GameDay2[] input)
    {
        long Solve_Part1()
        {
            return input
                .Where(x => !x.Sets.Any(s => s.R > 12 || s.G > 13 || s.B > 14))
                .Select(x => x.Id)
                .Sum();
        }

        long Solve_Part2()
        {
            return input
                .Select(x => x.Sets.Max(s => s.R) * x.Sets.Max(s => s.G) *x.Sets.Max(s => s.B))
                .Sum();
        }

        Solve_Part1().Out("Part 1: ");
        Solve_Part2().Out("Part 2: ");
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
                        first: replacements.Select(x => (x.digit, p: line.IndexOf(x.str))).Where(x => x.p != -1)
                            .MinBy(r => r.p).digit,
                        last: replacements.Select(x => (x.digit, p: line.LastIndexOf(x.str))).Where(x => x.p != -1)
                            .MaxBy(r => r.p).digit
                    )
                )
                .Select(x => x.first * 10 + x.last)
                .Sum();
        }

        Solve(isPart2: false).Out("Part 1: ");
        Solve(isPart2: true).Out("Part 2: ");
    }
}