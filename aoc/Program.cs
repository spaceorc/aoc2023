using System;
using System.Collections.Generic;
using System.Linq;
using aoc.ParseLib;
using aoc.ParseLib.Attributes;

namespace aoc;

public static class Program
{
    private static void Main()
    {
        Runner.RunFile("day7.txt", Solve_7_1);
        Runner.RunFile("day7.txt", Solve_7_2);
        // Runner.RunFile("day6.txt", Solve_6);
        // Runner.RunFile("day5.txt", Solve_5_1);
        // Runner.RunFile("day5.txt", Solve_5_2);
        // Runner.RunFile("day4.txt", Solve_4);
        // Runner.RunFile("day3.txt", Solve_3);
        // Runner.RunFile("day2.txt", Solve_2);
        // Runner.RunFile("day1.txt", Solve_1);
    }

    private static void Solve_7_2((string hand, long bid)[] input)
    {
        Solve().Out("Part 2: ");
        return;

        long Solve()
        {
            return input
                .Select(x => (x.bid, kind: GetKind(x.hand)))
                .OrderBy(x => GetSort(x.kind))
                .Select((x, i) => (x.bid, rank: i + 1))
                .Sum(x => x.bid * x.rank);
        }

        int GetCardScore(char k) => "J23456789TQKA".IndexOf(k) + 1;
        
        long GetSort(int[] kind) => kind.Aggregate(0L, (current, k) => current * 100 + k);
        
        int[] GetKind(string hand) => "23456789TQKA".Select(c => GetKindJ(hand, c)).MaxBy(GetSort)!;

        int[] GetKindJ(string hand, char j)
        {
            var g = hand.Replace('J', j)
                .GroupBy(c => c)
                .ToLookup(g => g.Count());

            return new[]
            {
                g[5].Count(),
                g[4].Count(),
                g[3].Count(),
                g[2].Count(),
                GetCardScore(hand[0]),
                GetCardScore(hand[1]),
                GetCardScore(hand[2]),
                GetCardScore(hand[3]),
                GetCardScore(hand[4]),
            };
        }
    }

    private static void Solve_7_1((string hand, long bid)[] input)
    {
        Solve().Out("Part 1: ");
        return;

        long Solve()
        {
            return input
                .Select(x => (x.bid, kind: GetKind(x.hand)))
                .OrderBy(x => GetSort(x.kind))
                .Select((x, i) => (x.bid, rank: i + 1))
                .Sum(x => x.bid * x.rank);
        }

        int GetCardScore(char k) => "23456789TJQKA".IndexOf(k) + 2;

        long GetSort(int[] kind) => kind.Aggregate(0L, (current, k) => current * 100 + k);

        int[] GetKind(string hand)
        {
            var g = hand
                .GroupBy(c => c)
                .ToLookup(g => g.Count());

            return new[]
            {
                g[5].Count(),
                g[4].Count(),
                g[3].Count(),
                g[2].Count(),
                GetCardScore(hand[0]),
                GetCardScore(hand[1]),
                GetCardScore(hand[2]),
                GetCardScore(hand[3]),
                GetCardScore(hand[4]),
            };
        }
    }

    [Template(
        """
        Time: {times}
        Distance: {distances}
        """
    )]
    private static void Solve_6(long[] times, long[] distances)
    {
        SolvePart1(SolveSqEq).Out("Part 1 (square equation): ");
        SolvePart2(SolveSqEq).Out("Part 2 (square equation): ");

        SolvePart1(SolveBruteforce).Out("Part 1 (bruteforce): ");
        SolvePart2(SolveBruteforce).Out("Part 2 (bruteforce): ");

        return;

        long SolveSqEq(long time, long distance)
        {
            // d = t * (T - t) = Tt - t^2 = D
            // t^2 - Tt + D = 0
            // x = (T +- sqrt(T^2 - 4D)) / 2;
            var x1 = (long)Math.Floor((time - Math.Sqrt(time * time - 4 * distance)) / 2) + 1;
            var x2 = (long)Math.Ceiling((time + Math.Sqrt(time * time - 4 * distance)) / 2) - 1;
            return x2 - x1 + 1;
        }

        long SolveBruteforce(long time, long distance)
        {
            return Enumerable.Range(0, (int)time + 1).Count(t => t * (time - t) > distance);
        }

        long SolvePart1(Func<long, long, long> solve)
        {
            return times
                .Select((time, i) => solve(time, distances[i]))
                .Product();
        }

        long SolvePart2(Func<long, long, long> solve)
        {
            var time = long.Parse(string.Join("", times));
            var distance = long.Parse(string.Join("", distances));
            return solve(time, distance);
        }
    }

    private static void Solve_5_2(
        [NonArray] [Template("seeds: {seeds}")] R[] seeds,
        [NonArray] [Template("{?}: {mappings}")] params (long dest, R src)[][] mappings
    )
    {
        Solve().Out("Part 2: ");
        return;

        long Solve()
        {
            var cur = seeds;
            foreach (var mapping in mappings)
            {
                var next = new List<R>();
                foreach (var c in cur)
                {
                    var used = mapping
                        .Select(m => (r: c.IntersectWith(m.src), shift: m.dest - m.src.Start))
                        .Where(x => !x.r.IsEmpty)
                        .ToList();

                    next.AddRange(used.Select(x => x.r.Shift(x.shift)));
                    next.AddRange(c.ExceptWith(used.Select(x => x.r)));
                }

                cur = next.ToArray();
            }

            return cur.Min(c => c.Start);
        }
    }

    private static void Solve_5_1(
        [NonArray] [Template("seeds: {seeds}")] long[] seeds,
        [NonArray] [Template("{?}: {mappings}")] params (long dest, long src, long len)[][] mappings
    )
    {
        Solve().Out("Part 1: ");
        return;

        long Solve()
        {
            return seeds
                .Select(
                    seed => mappings.Aggregate(
                        seed,
                        (cur, mapping) =>
                        {
                            var (dest, src, _) = mapping.SingleOrDefault(m => cur >= m.src && cur < m.src + m.len, (cur, cur, 1));
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
