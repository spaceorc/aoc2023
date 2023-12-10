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
        Runner.RunFile("day10.txt", Solve_10);
        // Runner.RunFile("day9.txt", Solve_9);
        // Runner.RunFile("day8.txt", Solve_8);
        // Runner.RunFile("day6.txt", Solve_6);
        // Runner.RunFile("day5.txt", Solve_5_1);
        // Runner.RunFile("day5.txt", Solve_5_2);
        // Runner.RunFile("day4.txt", Solve_4);
        // Runner.RunFile("day3.txt", Solve_3);
        // Runner.RunFile("day2.txt", Solve_2);
        // Runner.RunFile("day1.txt", Solve_1);
    }

    private static void Solve_10(Map<char> map)
    {
        var pipe = GetPipe();

        (pipe.Count / 2).Out("Part 1: ");
        CountInside().Out("Part 2: ");
        return;

        long CountInside()
        {
            const int OUTSIDE = 0;
            const int TOP_BORDER = 1;
            const int BOTTOM_BORDER = 2;
            const int INSIDE = 3;

            var border = pipe.ToHashSet();
            var count = 0;
            for (var y = 0; y < map.sizeY; y++)
            {
                var position = OUTSIDE;
                for (var x = 0; x < map.sizeX; x++)
                {
                    var v = new V(x, y);

                    if (border.Contains(v))
                    {
                        position = position switch
                        {
                            OUTSIDE => map[v] switch
                            {
                                '|' => INSIDE,
                                'F' => TOP_BORDER,
                                'L' => BOTTOM_BORDER,
                            },
                            TOP_BORDER => map[v] switch
                            {
                                'J' => INSIDE,
                                '7' => OUTSIDE,
                                '-' => position,
                            },
                            BOTTOM_BORDER => map[v] switch
                            {
                                'J' => OUTSIDE,
                                '7' => INSIDE,
                                '-' => position,
                            },
                            INSIDE => map[v] switch
                            {
                                '|' => OUTSIDE,
                                'F' => BOTTOM_BORDER,
                                'L' => TOP_BORDER,
                            },
                        };
                    }
                    else
                    {
                        if (position == INSIDE)
                            count++;
                    }
                }
            }

            return count;
        }

        List<V> GetPipe()
        {
            var start = map.All().Single(v => map[v] == 'S');
            foreach (var c in new[] { '-', '|', 'L', 'J', 'F', '7' })
            {
                var pipe = new List<V> { start };
                map[start] = c;
                var cur = start;
                var dir = map[start] switch
                {
                    '|' => new V(0, 1),
                    'J' => new V(0, 1),
                    'L' => new V(0, 1),
                    '-' => new V(1, 0),
                    'F' => new V(-1, 0),
                    '7' => new V(1, 0),
                    _ => throw new Exception($"start {map[start]}"),
                };
                var valid = true;
                while (true)
                {
                    if (dir == new V(0, 1))
                    {
                        switch (map[cur])
                        {
                            case '|':
                                break;
                            case 'L':
                                dir = new V(1, 0);
                                break;
                            case 'J':
                                dir = new V(-1, 0);
                                break;
                            default:
                                valid = false;
                                break;
                        }
                    }
                    else if (dir == new V(0, -1))
                    {
                        switch (map[cur])
                        {
                            case '|':
                                break;
                            case 'F':
                                dir = new V(1, 0);
                                break;
                            case '7':
                                dir = new V(-1, 0);
                                break;
                            default:
                                valid = false;
                                break;
                        }
                    }
                    else if (dir == new V(1, 0))
                    {
                        switch (map[cur])
                        {
                            case '-':
                                break;
                            case '7':
                                dir = new V(0, 1);
                                break;
                            case 'J':
                                dir = new V(0, -1);
                                break;
                            default:
                                valid = false;
                                break;
                        }
                    }
                    else if (dir == new V(-1, 0))
                    {
                        switch (map[cur])
                        {
                            case '-':
                                break;
                            case 'F':
                                dir = new V(0, 1);
                                break;
                            case 'L':
                                dir = new V(0, -1);
                                break;
                            default:
                                valid = false;
                                break;
                        }
                    }

                    if (!valid)
                        break;
                    if (pipe.Count > 1 && cur == start)
                        break;
                    cur += dir;
                    if (!map.Inside(cur))
                    {
                        valid = false;
                        break;
                    }

                    pipe.Add(cur);
                }

                if (valid)
                    return pipe;
            }

            throw new Exception("No pipe");
        }
    }

    private static void Solve_9(long[][] input)
    {
        SolvePart1().Out("Part 1: ");
        SolvePart2().Out("Part 2: ");
        return;

        long SolvePart1()
        {
            return input.Sum(
                data => data
                    .Generate(x => x.Window(2).Select(w => w[1] - w[0]).ToArray())
                    .TakeWhile(x => x.Any(e => e != 0))
                    .Select(x => x[^1])
                    .Sum()
            );
        }

        long SolvePart2()
        {
            return input.Sum(
                data => data
                    .Generate(x => x.Window(2).Select(w => w[1] - w[0]).ToArray())
                    .TakeWhile(x => x.Any(e => e != 0))
                    .Select(x => x[0])
                    .Reverse()
                    .Aggregate(0L, (acc, cur) => cur - acc)
            );
        }
    }

    private static void Solve_8(
        string moves,
        (string id, string left, string right)[] nodes
    )
    {
        SolvePart1().Out("Part 1: ");
        SolvePart2().Out("Part 2: ");
        return;

        long SolvePart1()
        {
            var graph = nodes.ToDictionary(n => n.id);
            var curNode = "AAA";
            var cur = 0;
            var count = 0;
            while (curNode != "ZZZ")
            {
                curNode = moves[cur] == 'L' ? graph[curNode].left : graph[curNode].right;
                cur = (cur + 1) % moves.Length;
                count++;
            }

            return count;
        }

        long SolvePart2()
        {
            var graph = nodes.ToDictionary(n => n.id);
            var startNodes = nodes.Select(n => n.id).Where(x => x[^1] == 'A').ToArray();
            var periods = startNodes.Select(_ => 0L).ToArray();
            for (var i = 0; i < startNodes.Length; i++)
            {
                var curNode = startNodes[i];
                var cur = 0;
                periods[i] = 0;
                while (curNode[^1] != 'Z')
                {
                    curNode = moves[cur] == 'L' ? graph[curNode].left : graph[curNode].right;
                    cur = (cur + 1) % moves.Length;
                    periods[i]++;
                }
            }

            return Helpers.Lcm(periods);
        }
    }

    private static void Solve_7((string hand, long bid)[] input)
    {
        input
            .Select(
                x => (
                    x.bid,
                    kind: x.hand.GroupBy(c => c)
                        .Select(g => g.Count())
                        .OrderDescending()
                        .Concat(x.hand.Select(c => "23456789TJQKA".IndexOf(c)))
                        .ToArray()
                )
            )
            .OrderBy(x => x.kind, ArrayComparer.Create<int>())
            .Select((x, i) => (x.bid, rank: i + 1))
            .Sum(x => x.bid * x.rank)
            .Out("Part 1: ");

        input
            .Select(
                x => (
                    x.bid,
                    kind: "23456789TQKA"
                        .Select(
                            j => x.hand.Replace('J', j)
                                .GroupBy(c => c)
                                .Select(g => g.Count())
                                .OrderDescending()
                                .Concat(x.hand.Select(c => "J23456789TQKA".IndexOf(c)))
                                .ToArray()
                        )
                        .Max(ArrayComparer.Create<int>())!
                )
            )
            .OrderBy(x => x.kind, ArrayComparer.Create<int>())
            .Select((x, i) => (x.bid, rank: i + 1))
            .Sum(x => x.bid * x.rank)
            .Out("Part 2: ");
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
