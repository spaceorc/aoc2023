using System;
using System.Collections.Generic;
using System.Linq;
using aoc.ParseLib;

namespace aoc;

public class Program
{
    private static void Main()
    {
        Runner.RunFile("day5.txt", Solve_5_1);
        Runner.RunFile("day5.txt", Solve_5_2);
    }

    private static void Solve_5_2(
        [NonArray] [Template("seeds: {seeds}")] (long start, long len)[] seeds,
        [NonArray] [Template("{?}: {mapping}")] params (long dest, long src, long len)[][] mapping
    )
    {
        Solve().Out("Part 2: ");
        return;

        long Solve()
        {
            var cur = seeds;
            foreach (var m in mapping)
            {
                var next = new List<(long start, long len)>();

                foreach (var (start, len) in cur)
                {
                    var used = new List<(long start, long len)>();
                    foreach (var (mDest, mSrc, mLen) in m)
                    {
                        var maxStart = Math.Max(start, mSrc);
                        var minEnd = Math.Min(start + len, mSrc + mLen);
                        if (minEnd > maxStart)
                        {
                            used.Add((maxStart, minEnd - maxStart));
                            next.Add((mDest + maxStart - mSrc, minEnd - maxStart));
                        }
                    }

                    used.Sort((a, b) => a.start.CompareTo(b.start));

                    var s = start;
                    foreach (var (usedStart, usedlen) in used)
                    {
                        if (s < usedStart)
                            next.Add((s, usedStart - s));
                        s = usedStart + usedlen;
                    }

                    if (s < start + len)
                        next.Add((s, start + len - s));
                }

                cur = next.ToArray();
            }

            return cur.Min(c => c.start);
        }
    }

    private static void Solve_5_1(
        [NonArray] [Template("seeds: {seeds}")] long[] seeds,
        [NonArray] [Template("{?}: {mapping}")] params (long dest, long src, long len)[][] mapping
    )
    {
        Solve().Out("Part 1: ");
        return;

        long Solve()
        {
            return seeds
                .Select(
                    seed => mapping.Aggregate(
                        seed,
                        (cur, m) =>
                        {
                            var (dest, src, _) = m.SingleOrDefault(mm => cur >= mm.src && cur < mm.src + mm.len, (cur, cur, 1));
                            return cur + dest - src;
                        }
                    )
                )
                .Min();
        }
    }

    private static void Solve_4([Template("Card {id}: {wins} | {nums}")] (long id, long[] wins, long[] nums)[] input)
    {
        SolvePart1().Out("Part 1: ");
        SolvePart2().Out("Part 2: ");
        return;

        long SolvePart1()
        {
            return input.Sum(
                line =>
                {
                    var winCount = line.nums.Where(line.wins.Contains).Count();
                    return winCount == 0 ? 0 : 1L << (winCount - 1);
                }
            );
        }

        long SolvePart2()
        {
            var counts = input.ToDictionary(x => x.id, _ => 1L);
            foreach (var (id, wins, nums) in input)
            {
                var winCount = nums.Where(wins.Contains).Count();
                for (var i = 0; i < winCount; i++)
                    counts[id + i + 1] += counts[id];
            }

            return counts.Sum(x => x.Value);
        }
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
            for (var y = 0; y < map.sizeY; y++)
            {
                var cur = 0L;
                var start = -1;
                for (var x = 0; x < map.sizeX; x++)
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
            for (var x = start - 1; x <= end + 1; x++)
            {
                var v = new V(x, y - 1);
                if (map.Inside(v) && map[v] != '.')
                    yield return v;
            }

            for (var x = start - 1; x <= end + 1; x++)
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

    private static void Solve_2([Template("Game {id}: {sets}")] [Split(";", Target = "sets")] (long id, (long n, string color)[][] sets)[] input)
    {
        var games = input
            .Select(
                x => (
                    x.id,
                    sets: x.sets
                        .Select(s => s.ToLookup(i => i.color, i => i.n))
                        .Select(
                            l => (
                                R: l["red"].SingleOrDefault(),
                                G: l["green"].SingleOrDefault(),
                                B: l["blue"].SingleOrDefault()
                            )
                        ))
            )
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
                .Select(
                    line => (
                        first: replacements.Select(x => (x.digit, p: line.IndexOf(x.str)))
                            .Where(x => x.p != -1)
                            .MinBy(r => r.p)
                            .digit,
                        last: replacements.Select(x => (x.digit, p: line.LastIndexOf(x.str)))
                            .Where(x => x.p != -1)
                            .MaxBy(r => r.p)
                            .digit
                    )
                )
                .Select(x => x.first * 10 + x.last)
                .Sum();
        }
    }
}
