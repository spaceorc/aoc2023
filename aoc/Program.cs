﻿using System.Collections.Generic;
using System.Linq;

namespace aoc
{
    public class Program
    {
        private static void Main()
        {
            Runner.RunFile("day3.txt", Solve_3);
        }

        private static void Solve_3(Map<char> map)
        {
            SolvePart1().Out("Part 1: ");
            SolvePart2().Out("Part 2: ");
            return;

            long SolvePart1()
            {
                return GetNumbers()
                    .Where(n => Nears(n.y, n.start, n.end).Any())
                    .Sum(n => n.num);
            }

            long SolvePart2()
            {
                return GetNumbers()
                    .SelectMany(n => Nears(n.y, n.start, n.end).Where(v => map[v] == '*').Select(gv => (gv, n.num)))
                    .GroupBy(g => g.gv, g => g.num)
                    .Where(g => g.Count() == 2)
                    .Sum(g => g.ElementAt(0) * g.ElementAt(1));
            }

            IEnumerable<(long num, int y, int start, int end)> GetNumbers()
            {
                for (int y = 0; y < map.sizeY; y++)
                {
                    var cur = 0L;
                    var start = -1;
                    for (int x = 0; x < map.sizeX; x++)
                    {
                        var v = new V(x, y);
                        if (char.IsDigit(map[v]))
                        {
                            cur = cur * 10 + (map[v] - '0');
                            if (start == -1)
                                start = x;
                        }
                        else
                        {
                            if (start != -1)
                                yield return (cur, y, start, x - 1);

                            start = -1;
                            cur = 0L;
                        }
                    }

                    if (start != -1)
                        yield return (cur, y, start, map.sizeX - 1);
                }
            }

            IEnumerable<V> Nears(int y, int start, int end)
            {
                for (int x = start - 1; x <= end + 1; x++)
                {
                    var v = new V(x, y - 1);
                    if (map.Inside(v) && map[v] != '.')
                        yield return v;
                }

                for (int x = start - 1; x <= end + 1; x++)
                {
                    var v = new V(x, y + 1);
                    if (map.Inside(v) && map[v] != '.')
                        yield return v;
                }

                var vv = new V(start - 1, y);
                if (map.Inside(vv) && map[vv] != '.')
                    yield return vv;

                vv = new V(end + 1, y);
                if (map.Inside(vv) && map[vv] != '.')
                    yield return vv;
            }
        }

        private static void Solve_2(
            [StructuredTemplate("Game {id}: {sets:[;]{item:[,]}}")]
            (long id, (long n, string color)[][] sets)[] input
        )
        {
            var games = input
                .Select(x => (
                    x.id,
                    sets: x.sets
                        .Select(s => s.ToLookup(i => i.color, i => i.n))
                        .Select(l => (
                                R: l["red"].SingleOrDefault(),
                                G: l["green"].SingleOrDefault(),
                                B: l["blue"].SingleOrDefault()
                            )
                        )))
                .ToList();

            SolvePart1().Out("Part 1: ");
            SolvePart2().Out("Part 2: ");
            return;

            long SolvePart1()
            {
                return games
                    .Where(x => x.sets.All(s => s is { R: <= 12, G: <= 13, B: <= 14 }))
                    .Select(x => x.id)
                    .Sum();
            }

            long SolvePart2()
            {
                return games
                    .Select(x => x.sets.Max(s => s.R) * x.sets.Max(s => s.G) * x.sets.Max(s => s.B))
                    .Sum();
            }
        }

        private static void Solve_1(string[] input)
        {
            Solve(false).Out("Part 1: ");
            Solve(true).Out("Part 2: ");
            return;

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
        }
    }
}