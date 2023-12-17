using System;
using System.Collections.Generic;
using System.Linq;
using aoc.Lib;
using aoc.ParseLib;
using aoc.ParseLib.Attributes;

namespace aoc;

public static class Program
{
    private static void Main()
    {
        Runner.RunFile("day17.txt", Solve_17);
        // Runner.RunFile("day16.txt", Solve_16);
        // Runner.RunFile("day15.txt", Solve_15);
        // Runner.RunFile("day14.txt", Solve_14);
        // Runner.RunFile("day13.txt", Solve_13);
        // Runner.RunFile("day12.txt", Solve_12);
        // Runner.RunFile("day11.txt", Solve_11);
        // Runner.RunFile("day10.txt", Solve_10);
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

    private static void Solve_17(Map<long> map)
    {
        SolvePart1().Out("Part 1: ");
        SolvePart2().Out("Part 2: ");
        return;

        long SolvePart1()
        {
            return Solve(minLen: 1, maxLen: 3);
        }

        long SolvePart2()
        {
            return Solve(minLen: 4, maxLen: 10);
        }

        long Solve(long minLen, long maxLen)
        {
            var queue = new Queue<(Walker walker, int len)>();
            var used = new Dictionary<(Walker walker, int len), long>();
            var start = (walker: new Walker(V.Zero, Dir.Right), len: 0);
            queue.Enqueue(start);
            used.Add(start, 0);
            var queuedStates = new HashSet<(Walker walker, int len)>();
            queuedStates.Add(start);

            while (queue.Count > 0)
            {
                var cur = queue.Dequeue();
                queuedStates.Remove(cur);
                var cur1 = used[cur];

                var nextStates = new[]
                {
                    (walker: cur.walker.Forward(), len: cur.len + 1),
                    (walker: cur.walker.TurnCW().Forward(), len: 1),
                    (walker: cur.walker.TurnCCW().Forward(), len: 1),
                };
                foreach (var next in nextStates)
                {
                    if (next.len > maxLen || next.walker.Dir != cur.walker.Dir && cur.len < minLen)
                        continue;
                    if (!next.walker.Inside(map))
                        continue;
                    if (used.TryGetValue(next, out var prev) && prev <= cur1 + map[next.walker.Pos])
                        continue;

                    used[next] = cur1 + map[next.walker.Pos];
                    if (queuedStates.Add(next))
                        queue.Enqueue(next);
                }
            }

            return used.Where(u => u.Key.walker.Pos == map.BottomRight && u.Key.len >= minLen).Min(u => u.Value);
        }
    }

    private static void Solve_16(Map<char> map)
    {
        SolvePart1().Out("Part 1: ");
        SolvePart2().Out("Part 2: ");
        return;

        long SolvePart1()
        {
            return CountEnergized2((V.Zero, V.right));
            // return CountEnergized(new Walker(V.Zero, Dir.Right));
        }

        long SolvePart2()
        {
            return new[]
                {
                    map.TopBorder().Select(v => new Walker(v, Dir.Down)),
                    map.BottomBorder().Select(v => new Walker(v, Dir.Up)),
                    map.LeftBorder().Select(v => new Walker(v, Dir.Right)),
                    map.RightBorder().Select(v => new Walker(v, Dir.Left)),
                }
                .Flatten()
                .Select(CountEnergized)
                .Max();
        }

        long CountEnergized(Walker startFrom)
        {
            return BfsHelpers.Bfs(
                    startFrom: new[] { startFrom },
                    cur => ((map[cur.Pos], cur.Dir) switch
                        {
                            ('.', _) => new[] { cur },
                            ('/', Dir.Right or Dir.Left) => new[] { cur.TurnCCW() },
                            ('/', Dir.Up or Dir.Down) => new[] { cur.TurnCW() },
                            ('\\', Dir.Right or Dir.Left) => new[] { cur.TurnCW() },
                            ('\\', Dir.Up or Dir.Down) => new[] { cur.TurnCCW() },
                            ('-', Dir.Left or Dir.Right) => new[] { cur },
                            ('|', Dir.Up or Dir.Down) => new[] { cur },
                            ('-' or '|', _) => new[] { cur.TurnCCW(), cur.TurnCW() },
                            _ => throw new Exception($"Invalid state: {cur}")
                        })
                        .Select(next => next.Forward())
                        .Where(next => next.Inside(map))
                )
                .Select(s => s.State.Pos)
                .Distinct()
                .Count();
        }

        long CountEnergized2((V pos, V dir) startFrom)
        {
            return BfsHelpers.Bfs(
                    startFrom: new[] { startFrom },
                    cur => ((map[cur.pos], cur.dir) switch
                        {
                            ('.', { } dir) => new[] { dir },
                            ('-', { Y: 0 } dir) => new[] { dir },
                            ('|', { X: 0 } dir) => new[] { dir },
                            ('/', { Y: 0 } dir) => new[] { dir.RotateCCW() },
                            ('/', { X: 0 } dir) => new[] { dir.RotateCW() },
                            ('\\', { Y: 0 } dir) => new[] { dir.RotateCW() },
                            ('\\', { X: 0 } dir) => new[] { dir.RotateCCW() },
                            (_, { } dir) => new[] { dir.RotateCW(), dir.RotateCCW() },
                        })
                        .Where(dir => map.Inside(cur.pos + dir))
                        .Select(dir => (cur.pos + dir, dir))
                )
                .Select(s => s.State.pos)
                .Distinct()
                .Count();
        }
    }

    private static void Solve_15(string input)
    {
        SolvePart1().Out("Part 1: ");
        SolvePart2().Out("Part 2: ");
        return;

        long Hash(string line) => line.Aggregate(0L, (current, c) => (current + c) * 17 % 256);

        long SolvePart1() => input.Split(',').Sum(Hash);

        (string name, char op, long value) ParseItem(string item)
        {
            var items = item.Split('-', '=');
            var op = item[items[0].Length];
            return (items[0], op, op == '-' ? 0L : long.Parse(items[1]));
        }

        long SolvePart2()
        {
            var boxes = Enumerable.Range(0, 256).Select(_ => new List<(string name, long value)>()).ToArray();
            var items = input.Split(',').Select(ParseItem);
            foreach (var (name, op, value) in items)
            {
                var box = boxes[Hash(name)];
                switch (op)
                {
                    case '-':
                        box.RemoveAll(bb => bb.name == name);
                        break;
                    case '=':
                        var index = box.FindIndex(item => item.name == name);
                        if (index < 0)
                            box.Add((name, value));
                        else
                            box[index] = (name, value);
                        break;
                }
            }

            return boxes
                .SelectMany((box, bi) => box.Select((item, i) => (bi + 1L) * (i + 1L) * item.value))
                .Sum();
        }
    }

    private static void Solve_14(Map<char> map)
    {
        SolvePart1().Out("Part 1: ");
        SolvePart2().Out("Part 2: ");

        return;

        long CalcLoad() => map.All().Where(v => map[v] == 'O').Sum(v => map.sizeY - v.Y);

        void MoveAlong(IEnumerable<V[]> lines)
        {
            foreach (var line in lines)
            {
                var start = 0;
                for (var i = 0; i < line.Length; i++)
                {
                    switch (map[line[i]])
                    {
                        case '#':
                            start = i + 1;
                            continue;
                        case 'O':
                            map.Swap(line[i], line[start]);
                            start++;
                            continue;
                    }
                }
            }
        }

        void MoveBackAlong(IEnumerable<V[]> lines) => MoveAlong(lines.Select(line => line.Reverse().ToArray()));

        void MoveN() => MoveAlong(map.Columns());
        void MoveS() => MoveBackAlong(map.Columns());
        void MoveW() => MoveAlong(map.Rows());
        void MoveE() => MoveBackAlong(map.Rows());

        long SolvePart1()
        {
            MoveN();
            return CalcLoad();
        }

        long SolvePart2()
        {
            return (hash: map.GetHashCode(), load: CalcLoad()).Generate(
                    _ =>
                    {
                        MoveN();
                        MoveW();
                        MoveS();
                        MoveE();
                        return (map.GetHashCode(), CalcLoad());
                    }
                )
                .ElementAtWithCycleTrack(1000000000L)
                .load;
        }
    }

    private static void Solve_13(params Map<char>[] maps)
    {
        SolvePart1().Out("Part 1: ");
        SolvePart2().Out("Part 2: ");
        return;

        long FindLeftToRightReflection(Map<char> map, long but = 0)
        {
            return Enumerable
                .Range(1, map.sizeX - 1)
                .FirstOrDefault(
                    leftCols => leftCols != but &&
                                Enumerable
                                    .Range(0, Math.Min(leftCols, map.sizeX - leftCols))
                                    .All(i => map.ColumnString(leftCols - i - 1) == map.ColumnString(leftCols + i))
                );
        }

        long FindTopToBottomReflection(Map<char> map, long but = 0)
        {
            return Enumerable
                .Range(1, map.sizeY - 1)
                .FirstOrDefault(
                    topRows => topRows != but &&
                               Enumerable
                                   .Range(0, Math.Min(topRows, map.sizeY - topRows))
                                   .All(i => map.RowString(topRows - i - 1) == map.RowString(topRows + i))
                );
        }

        long SolvePart1() => maps.Sum(map => FindLeftToRightReflection(map) + FindTopToBottomReflection(map) * 100);

        long SolvePart2() => maps
            .Select(
                map =>
                {
                    var original = (l2r: FindLeftToRightReflection(map), t2b: FindTopToBottomReflection(map));
                    var sum = map.All()
                        .Sum(
                            v =>
                            {
                                using (map.ChangeAt(v, c => c == '.' ? '#' : '.'))
                                    return FindLeftToRightReflection(map, but: original.l2r) + FindTopToBottomReflection(map, but: original.t2b) * 100L;
                            }
                        );
                    return sum / 2;
                }
            )
            .Sum();
    }

    private static void Solve_12((string source, int[] groups)[] input)
    {
        SolvePart1().Out("Part 1: ");
        SolvePart2().Out("Part 2: ");
        return;

        long SolvePart1()
        {
            return input.Sum(x => SolveOne(x.source, x.groups));
        }

        long SolvePart2()
        {
            return input
                .Select(
                    x => (
                        source: string.Join('?', Enumerable.Repeat(x.source, 5)),
                        groups: Enumerable.Repeat(x.groups, 5).SelectMany(v => v).ToArray()
                    )
                )
                .Sum(x => SolveOne(x.source, x.groups));
        }

        long SolveOne(string source, int[] groups)
        {
            return Count(source, source.Length, groups, groups.Length, 0, new Dictionary<(int, int, int), long>());
        }

        long Count(string source, int len, int[] groups, int glen, int used, Dictionary<(int, int, int), long> results)
        {
            if (results.TryGetValue((len, glen, used), out var res))
                return res;

            if (len == 0)
                return glen == 0 || (glen == 1 && used == groups[glen - 1]) ? 1 : 0;

            var resDot = source[len - 1] switch
            {
                '.' or '?' when glen == 0 || used == 0 => Count(source, len - 1, groups, glen, used, results),
                '.' or '?' when used == groups[glen - 1] => Count(source, len - 1, groups, glen - 1, 0, results),
                '.' or '?' => 0,
                _ => 0L,
            };

            var resSharp = source[len - 1] switch
            {
                '#' or '?' when glen == 0 || used == groups[glen - 1] => 0,
                '#' or '?' => Count(source, len - 1, groups, glen, used + 1, results),
                _ => 0L,
            };

            results[(len, glen, used)] = resDot + resSharp;
            return resDot + resSharp;
        }
    }

    private static void Solve_11(Map<char> map)
    {
        Solve(2).Out("Part 1: ");
        Solve(1_000_000).Out("Part 2: ");
        return;

        long Solve(long times)
        {
            var galaxies = map.All().Where(v => map[v] == '#').ToArray();
            var emptyRows = map.Rows().Where(row => row.All(v => map[v] == '.')).Select(r => r[0].Y).ToArray();
            var emptyCols = map.Columns().Where(col => col.All(v => map[v] == '.')).Select(r => r[0].X).ToArray();
            galaxies = galaxies
                .Select(
                    v => (
                        v,
                        emptyColsCount: emptyCols.Count(e => e < v.X),
                        emptyRowsCount: emptyRows.Count(e => e < v.Y)
                    )
                )
                .Select(p => p.v + (times - 1) * new V(p.emptyColsCount, p.emptyRowsCount))
                .ToArray();
            return galaxies
                .Combinations(2)
                .Sum(c => c[0].MDistTo(c[1]));
        }
    }

    private static void Solve_10(Map<char> map)
    {
        var pipe = BuildPipe();

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

        List<V> BuildPipe()
        {
            var start = map.All().Single(v => map[v] == 'S');
            return new[] { '-', '|', 'L', 'J', 'F', '7' }
                .Select(startSegment => TryBuildPipe(start, startSegment))
                .First(x => x != null)!;
        }

        List<V>? TryBuildPipe(V start, char startSegment)
        {
            const int UP = 0;
            const int RIGHT = 1;
            const int DOWN = 2;
            const int LEFT = 3;

            var shifts = new[]
            {
                new V(0, -1),
                new V(1, 0),
                new V(0, 1),
                new V(-1, 0),
            };

            var dirs = new Dictionary<char, int[]>
            {
                { '-', new[] { RIGHT, LEFT } },
                { '|', new[] { UP, DOWN } },
                { 'L', new[] { UP, RIGHT } },
                { 'J', new[] { UP, LEFT } },
                { 'F', new[] { DOWN, RIGHT } },
                { '7', new[] { DOWN, LEFT } },
            };

            var moves = dirs
                .SelectMany(
                    kvp => new[]
                    {
                        (kvp.Key, inDir: (kvp.Value[0] + 2) % 4, outDir: kvp.Value[1]),
                        (kvp.Key, inDir: (kvp.Value[1] + 2) % 4, outDir: kvp.Value[0]),
                    }
                )
                .ToDictionary(x => (x.Key, x.inDir), x => x.outDir);

            map[start] = startSegment;

            var pipe = new List<V> { start };
            var cur = start;
            var inDir = moves.First(m => m.Key.Key == map[start]).Key.inDir;
            while (true)
            {
                if (!moves.TryGetValue((map[cur], inDir), out var outDir))
                    return null;

                if (pipe.Count > 1 && cur == start)
                    return pipe;

                cur += shifts[outDir];
                inDir = outDir;
                if (!map.Inside(cur))
                    return null;

                pipe.Add(cur);
            }
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
                    .Generate(x => x.SlidingWindow(2).Select(w => w[1] - w[0]).ToArray())
                    .TakeWhile(x => x.Any(e => e != 0))
                    .Select(x => x[^1])
                    .Sum()
            );
        }

        long SolvePart2()
        {
            return input.Sum(
                data => data
                    .Generate(x => x.SlidingWindow(2).Select(w => w[1] - w[0]).ToArray())
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

            return MathHelpers.Lcm(periods);
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
