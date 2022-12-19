using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Versioning;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using static System.Math;

namespace aoc;

public class Program
{
    static void Main()
    {
        Runner.RunFile("day19.txt", Solve_19);
    }

    record NumberDay19(int Ore, int Clay, int Obsidian, int Geode)
    {
        public static NumberDay19 operator -(NumberDay19 a) => new(-a.Ore, -a.Clay, -a.Obsidian, -a.Geode);

        public static NumberDay19 operator +(NumberDay19 a, NumberDay19 b) =>
            new(a.Ore + b.Ore, a.Clay + b.Clay, a.Obsidian + b.Obsidian, a.Geode + b.Geode);

        public static NumberDay19 operator *(NumberDay19 a, int coeff) =>
            new(a.Ore * coeff, a.Clay * coeff, a.Obsidian * coeff, a.Geode * coeff);

        public static NumberDay19 operator *(int coeff, NumberDay19 a) =>
            new(a.Ore * coeff, a.Clay * coeff, a.Obsidian * coeff, a.Geode * coeff);

        public static NumberDay19 operator -(NumberDay19 a, NumberDay19 b) => a + (-b);

        public bool IsNegative() => Ore < 0 || Clay < 0 || Obsidian < 0 || Geode < 0;

        public bool Covers(NumberDay19 other) => !(this - other).IsNegative();

        public override string ToString()
        {
            return $"{Ore}:{Clay}:{Obsidian}:{Geode}";
        }
    }

    record BlueprintDay19(int Id, int OreRobotCostOre, int ClayRobotCostOre, int ObsidianRobotCostOre,
        int ObsidianRobotCostClay,
        int GeodeRobotCostOre, int GeodeRobotCostObsidian)
    {
        public NumberDay19 OreRobotCost => new(OreRobotCostOre, 0, 0, 0);
        public NumberDay19 ClayRobotCost => new(ClayRobotCostOre, 0, 0, 0);
        public NumberDay19 ObsidianRobotCost => new(ObsidianRobotCostOre, ObsidianRobotCostClay, 0, 0);
        public NumberDay19 GeodeRobotCost => new(GeodeRobotCostOre, 0, GeodeRobotCostObsidian, 0);
    }

    static void Solve_19(
        [Template("Blueprint {Id}: " +
                  "Each ore robot costs {OreRobotCostOre} ore. " +
                  "Each clay robot costs {ClayRobotCostOre} ore. " +
                  "Each obsidian robot costs {ObsidianRobotCostOre} ore and {ObsidianRobotCostClay} clay. " +
                  "Each geode robot costs {GeodeRobotCostOre} ore and {GeodeRobotCostObsidian} obsidian.")]
        BlueprintDay19[] input)
    {
        SolveTime(input, 24).Sum(x => x.id * x.score).Out("Part 1: ");
        SolveTime(input.Take(3).ToArray(), 32).Select(x => x.score).Aggregate((a, b) => a * b).Out("Part 2: ");

        List<(int id, int score)> SolveTime(BlueprintDay19[] blueprints, int time)
        {
            var result = new List<(int id, int score)>();
            var tasks = new List<Task>();
            var b = 0;
            for (int tt = 0; tt < Environment.ProcessorCount; tt++)
            {
                tasks.Add(Task.Run(() =>
                {
                    while (true)
                    {
                        var index = Interlocked.Increment(ref b) - 1;
                        if (index >= blueprints.Length)
                            return;
                        var blueprint = blueprints[index];
                        var score = Solve(
                            blueprint: blueprint,
                            turn: time,
                            resources: new NumberDay19(0, 0, 0, 0),
                            robots: new NumberDay19(1, 0, 0, 0),
                            results: new Dictionary<(int, NumberDay19, NumberDay19), int>(),
                            bestKnownResult: 0,
                            forbidden: new NumberDay19(0, 0, 0, 0));
                        lock(result)
                            result.Add((blueprint.Id, score));
                        Console.Out.WriteLine($"{blueprint.Id}: {score}");
                    }
                }));
            }

            Task.WaitAll(tasks.ToArray());
            return result;
        }

        int Solve(BlueprintDay19 blueprint, int turn, NumberDay19 resources, NumberDay19 robots,
            Dictionary<(int, NumberDay19, NumberDay19), int> results, int bestKnownResult, NumberDay19 forbidden)
        {
            if (turn == 0)
                return resources.Geode > bestKnownResult ? resources.Geode : bestKnownResult;

            var key = (turn, resources, robots);
            if (results.ContainsKey(key))
                return results[key];

            var possibleGeodes = resources.Geode;
            var possibleGeodeRobots = robots.Geode;
            for (int i = 0; i < turn; i++)
            {
                possibleGeodes += possibleGeodeRobots;
                possibleGeodeRobots++;
            }
            
            if (possibleGeodes <= bestKnownResult)
            {
                results[key] = bestKnownResult;
                return bestKnownResult;
            }

            var bestResult = bestKnownResult;
            var canGeode = resources.Covers(blueprint.GeodeRobotCost);
            var canObsidian = resources.Covers(blueprint.ObsidianRobotCost);
            var canClay = resources.Covers(blueprint.ClayRobotCost);
            var canOre = resources.Covers(blueprint.OreRobotCost);

            if (canGeode && forbidden.Geode == 0)
            {
                var newResult = Solve(blueprint, turn - 1, resources - blueprint.GeodeRobotCost + robots,
                    robots + new NumberDay19(0, 0, 0, 1), results, bestResult, new NumberDay19(0, 0, 0, 0));
                if (newResult > bestResult)
                    bestResult = newResult;
            }

            if (!canGeode && canObsidian && forbidden.Obsidian == 0)
            {
                var newResult = Solve(blueprint, turn - 1, resources - blueprint.ObsidianRobotCost + robots,
                    robots + new NumberDay19(0, 0, 1, 0), results, bestResult, new NumberDay19(0, 0, 0, 0));
                if (newResult > bestResult)
                    bestResult = newResult;
            }

            if (!canGeode && canClay && forbidden.Clay == 0)
            {
                var newResult = Solve(blueprint, turn - 1, resources - blueprint.ClayRobotCost + robots,
                    robots + new NumberDay19(0, 1, 0, 0), results, bestResult, new NumberDay19(0, 0, 0, 0));
                if (newResult > bestResult)
                    bestResult = newResult;
            }

            if (!canGeode && canOre && forbidden.Ore == 0)
            {
                var newResult = Solve(blueprint, turn - 1, resources - blueprint.OreRobotCost + robots,
                    robots + new NumberDay19(1, 0, 0, 0), results, bestResult, new NumberDay19(0, 0, 0, 0));
                if (newResult > bestResult)
                    bestResult = newResult;
            }

            if (!canGeode)
            {
                var newResult = Solve(blueprint, turn - 1, resources + robots, robots, results, bestResult,
                    new NumberDay19(canOre ? 1 : 0, canClay ? 1 : 0, canObsidian ? 1 : 0, canGeode ? 1 : 0));
                if (newResult > bestResult)
                    bestResult = newResult;
            }

            results[key] = bestResult;
            return bestResult;
        }
    }

    static void Solve_18(V3[] cubes)
    {
        long SurfaceSquare(IEnumerable<V3> cs)
        {
            var set = cs.ToHashSet();
            return set.Sum(v => 6 - v.Neighbors().Count(set.Contains));
        }

        SurfaceSquare(cubes).Out("Part 1: ");

        var cubesSet = cubes.ToHashSet();
        var range = cubes.BoundingBox().Grow(1);
        var water = Helpers.Bfs(
                startFrom: range.Border(),
                getNextStates: v => v.Neighbors()
                    .Where(n => range.Contains(n)
                                && !cubesSet.Contains(n))
            )
            .Select(s => s.State);

        var holes = range.All()
            .Except(water)
            .Except(cubes);

        (SurfaceSquare(cubes) - SurfaceSquare(holes)).Out("Part 2: ");
    }

    static void Solve_17_2(string[] lines)
    {
        var input = lines[0];

        var figures = new[]
        {
            new V[] { new(0, 0), new(1, 0), new(2, 0), new(3, 0) },
            new V[] { new(1, 0), new(0, 1), new(1, 1), new(2, 1), new(1, 2) },
            new V[] { new(0, 0), new(1, 0), new(2, 0), new(2, 1), new(2, 2) },
            new V[] { new(0, 0), new(0, 1), new(0, 2), new(0, 3) },
            new V[] { new(0, 0), new(1, 0), new(1, 1), new(0, 1) },
        };
        var figureHeight = new[] { 1, 3, 3, 4, 2 };
        var figureWidth = new[] { 4, 3, 3, 1, 2 };
        var used = new HashSet<V>(Enumerable.Range(0, 7).Select(x => new V(x, 0)));
        var maxY = 0L;

        long fi = 0;
        long fc = 0;
        V curV = V.Zero;
        Appear();

        var fcc = 1000000000000L;

        for (int i = 0; i < input.Length; i++)
            Next(i);
        var prevMaxY = maxY;
        var prevFc = fc;
        for (int i = 0; i < input.Length; i++)
            Next(i);
        Console.Out.WriteLine($"delta maxY={maxY - prevMaxY}");
        Console.Out.WriteLine($"delta fc={fc - prevFc}");
        Print();

        var times = (fcc - fc) / (fc - prevFc);
        var realMaxY = maxY + times * (maxY - prevMaxY);
        fc += times * (fc - prevFc);

        var old = maxY;
        for (int i = 0; i < input.Length; i++)
        {
            Next(i);
            if (fc == fcc)
            {
                Console.WriteLine(realMaxY + maxY - old);
                return;
            }
        }


        void Next(int index)
        {
            if (input[index] == '<')
                PushLeft();
            else
                PushRight();
            if (!PushDown())
            {
                Rest();
                Appear();
                fi = (fi + 1) % figures.Length;
                fc++;
            }
        }

        void PushRight()
        {
            if (curV.X + figureWidth[fi] == 7)
                return;
            foreach (var v in figures[fi])
            {
                var nv = curV + v + new V(1, 0);
                if (used.Contains(nv))
                    return;
            }

            curV += new V(1, 0);
        }

        void Rest()
        {
            foreach (var v in figures[fi])
            {
                used.Add(curV + v);
                if (curV.Y + v.Y > maxY)
                    maxY = curV.Y + v.Y;
            }
        }

        bool PushDown()
        {
            foreach (var v in figures[fi])
            {
                var nv = curV + v + new V(0, -1);
                if (used.Contains(nv))
                    return false;
            }

            curV += new V(0, -1);
            return true;
        }

        void PushLeft()
        {
            if (curV.X == 0)
                return;
            foreach (var v in figures[fi])
            {
                var nv = curV + v - new V(1, 0);
                if (used.Contains(nv))
                    return;
            }

            curV -= new V(1, 0);
        }

        void Appear()
        {
            curV = new V(2, maxY + 4);
        }

        void Print()
        {
            Console.WriteLine();
            var my = Max(maxY, curV.Y + figureHeight[fi] - 1);
            for (long y = my; y >= Max(0, my - 40); y--)
            {
                Console.Write("|");
                for (int x = 0; x < 7; x++)
                {
                    var v = new V(x, y);
                    if (used.Contains(v))
                        Console.Write("#");
                    else
                    {
                        if (figures[fi].Any(fv => curV + fv == v))
                            Console.Write('@');
                        else
                            Console.Write(' ');
                    }
                }

                Console.WriteLine("|");
            }
        }
    }

    static void Solve_17_1(string[] lines)
    {
        var input = lines[0];

        var figures = new[]
        {
            new V[] { new(0, 0), new(1, 0), new(2, 0), new(3, 0) },
            new V[] { new(1, 0), new(0, 1), new(1, 1), new(2, 1), new(1, 2) },
            new V[] { new(0, 0), new(1, 0), new(2, 0), new(2, 1), new(2, 2) },
            new V[] { new(0, 0), new(0, 1), new(0, 2), new(0, 3) },
            new V[] { new(0, 0), new(1, 0), new(1, 1), new(0, 1) },
        };
        var figureHeight = new[] { 1, 3, 3, 4, 2 };
        var figureWidth = new[] { 4, 3, 3, 1, 2 };
        var used = new HashSet<V>(Enumerable.Range(0, 7).Select(x => new V(x, 0)));
        var maxY = 0L;

        var fi = 0;
        var fc = 0;
        V curV = V.Zero;
        Appear();
        // Print();

        while (true)
        {
            for (int i = 0; i < input.Length; i++)
            {
                Next(i);
                // Print();
                if (fc == 2022)
                {
                    Console.WriteLine(maxY);
                    return;
                }
            }
        }

        void Next(int index)
        {
            if (input[index] == '<')
                PushLeft();
            else
                PushRight();
            if (!PushDown())
            {
                Rest();
                Appear();
                fi = (fi + 1) % figures.Length;
                fc++;
            }
        }

        void PushRight()
        {
            if (curV.X + figureWidth[fi] == 7)
                return;
            foreach (var v in figures[fi])
            {
                var nv = curV + v + new V(1, 0);
                if (used.Contains(nv))
                    return;
            }

            curV += new V(1, 0);
        }

        void Rest()
        {
            foreach (var v in figures[fi])
            {
                used.Add(curV + v);
                if (curV.Y + v.Y > maxY)
                    maxY = curV.Y + v.Y;
            }
        }

        bool PushDown()
        {
            foreach (var v in figures[fi])
            {
                var nv = curV + v + new V(0, -1);
                if (used.Contains(nv))
                    return false;
            }

            curV += new V(0, -1);
            return true;
        }

        void PushLeft()
        {
            if (curV.X == 0)
                return;
            foreach (var v in figures[fi])
            {
                var nv = curV + v - new V(1, 0);
                if (used.Contains(nv))
                    return;
            }

            curV -= new V(1, 0);
        }

        void Appear()
        {
            curV = new V(2, maxY + 4);
        }

        void Print()
        {
            Console.WriteLine();
            var my = Max(maxY, curV.Y + figureHeight[fi] - 1);
            for (long y = my; y >= 0; y--)
            {
                Console.Write("|");
                for (int x = 0; x < 7; x++)
                {
                    var v = new V(x, y);
                    if (used.Contains(v))
                        Console.Write("#");
                    else
                    {
                        if (figures[fi].Any(fv => curV + fv == v))
                            Console.Write('@');
                        else
                            Console.Write(' ');
                    }
                }

                Console.WriteLine("|");
            }
        }
    }

    record ItemDay16(string from, int rate, string[] tos)
    {
        public override string ToString()
        {
            return $"{from} ({rate}) -> [{string.Join(", ", tos)}]";
        }
    }

    static void Solve_16_2(
        [Template(
            "^Valve (?<from>.*) has flow rate=(?<rate>.*); tunnels? leads? to valves? (?<tos>.*)$",
            IsRegex = true)]
        ItemDay16[] input)
    {
        var indexes = input.OrderBy(x => x.from).Select((x, i) => (x.from, i)).ToDictionary(x => x.from, x => x.i);
        var moves = input.ToDictionary(x => indexes[x.from], x => x.tos.Select(to => indexes[to]).ToArray())
            .OrderBy(x => x.Key).Select(x => x.Value).ToArray();
        var rates = input.ToDictionary(x => indexes[x.from], x => x.rate).OrderBy(x => x.Key).Select(x => x.Value)
            .ToArray();

        var pairMoves = new Dictionary<int, List<int>>();
        for (var from1 = 0; from1 < moves.Length; from1++)
        for (var from2 = from1; from2 < moves.Length; from2++)
        {
            var pairMovesFromPos = new HashSet<int>();
            foreach (var to1 in moves[from1])
            foreach (var to2 in moves[from2])
                pairMovesFromPos.Add(MakePairPos(to1, to2));
            pairMoves[MakePairPos(from1, from2)] = pairMovesFromPos.ToList();
        }

        long MakeState(int pos1, int pos2, long mask)
        {
            if (pos1 > pos2)
                (pos1, pos2) = (pos2, pos1);
            return ((long)pos1 << 50) | ((long)pos2 << 56) | mask;
        }

        long MakeStateFromPairPos(int pairPos, long mask)
        {
            return ((long)pairPos << 50) | mask;
        }

        int MakePairPos(int pos1, int pos2)
        {
            if (pos1 > pos2)
                (pos1, pos2) = (pos2, pos1);
            return (pos2 << 6) | pos1;
        }

        void ParseState(long state, out int pos1, out int pos2, out int pairPos, out long mask)
        {
            mask = state & ((1L << 50) - 1);
            pos1 = (int)((state >> 50) & 0b111111);
            pos2 = (int)((state >> 56) & 0b111111);
            pairPos = (int)(state >> 50);
        }

        long MakePriority(int rate, int timeLeft)
        {
            return (long)rate << 32 | (uint)timeLeft;
        }

        void ParsePriority(long priority, out int rate, out int timeLeft)
        {
            timeLeft = (int)(priority & 0xFFFFFFFF);
            rate = (int)(priority >> 32);
        }

        bool IsClosed(long mask, int pos)
        {
            return (mask & (1L << pos)) == 0;
        }

        long Open(long mask, int pos)
        {
            return mask | (1L << pos);
        }

        var queue = new Queue<long>();
        var used = new Dictionary<long, long>();
        AddState(MakeState(0, 0, 0), MakePriority(0, 26));

        void AddState(long state, long priority)
        {
            if (!used.TryGetValue(state, out var prevPriority) || prevPriority < priority)
            {
                queue.Enqueue(state);
                used[state] = priority;
            }
        }

        var maxRate = 0L;
        var iteration = 0;
        var sw = Stopwatch.StartNew();
        while (queue.Count > 0)
        {
            if (iteration++ % 1000000 == 0)
                Console.WriteLine($"used={used.Count}, queue={queue.Count}");

            var curState = queue.Dequeue();
            ParseState(curState, out var curPos1, out var curPos2, out var curPairPos, out var curMask);

            var curPriority = used[curState];
            ParsePriority(curPriority, out var curRate, out var curTimeLeft);
            if (curRate > maxRate)
            {
                maxRate = curRate;
                Console.WriteLine($"used={used.Count}, queue={queue.Count}, max={maxRate}");
            }

            if (curTimeLeft == 0)
                continue;

            foreach (var nextPairPos in pairMoves[curPairPos])
            {
                var nextState = MakeStateFromPairPos(nextPairPos, curMask);
                var nextPriority = MakePriority(curRate, curTimeLeft - 1);
                AddState(nextState, nextPriority);
            }

            if (IsClosed(curMask, curPos1))
            {
                var addRate1 = (curTimeLeft - 1) * rates[curPos1];
                if (addRate1 > 0)
                {
                    foreach (var nextPos2 in moves[curPos2])
                    {
                        var nextState = MakeState(curPos1, nextPos2, Open(curMask, curPos1));
                        var nextPriority = MakePriority(curRate + addRate1, curTimeLeft - 1);
                        AddState(nextState, nextPriority);
                    }

                    if (curPos1 != curPos2 && IsClosed(curMask, curPos2))
                    {
                        var addRate2 = (curTimeLeft - 1) * rates[curPos2];
                        if (addRate2 > 0)
                        {
                            var nextState = MakeStateFromPairPos(curPairPos, Open(Open(curMask, curPos1), curPos2));
                            var nextPriority = MakePriority(curRate + addRate1 + addRate2, curTimeLeft - 1);
                            AddState(nextState, nextPriority);
                        }
                    }
                }
            }

            if (IsClosed(curMask, curPos2))
            {
                var addRate2 = (curTimeLeft - 1) * rates[curPos2];
                if (addRate2 > 0)
                {
                    foreach (var nextPos1 in moves[curPos1])
                    {
                        var nextState = MakeState(nextPos1, curPos2, Open(curMask, curPos2));
                        var nextPriority = MakePriority(curRate + addRate2, curTimeLeft - 1);
                        AddState(nextState, nextPriority);
                    }
                }
            }
        }

        sw.Elapsed.Out("Elapsed: ");
        maxRate.Out("Part 2: ");
    }

    record State2Day16(string pos, string pos2, long openMask)
    {
        public static State2Day16 Create(string pos, string pos2, long openMask)
        {
            if (pos.CompareTo(pos2) > 0)
            {
                return new State2Day16(pos2, pos, openMask);
            }

            return new State2Day16(pos, pos2, openMask);
        }
    }

    static void Solve_16_2_slow(
        [Template(
            "^Valve (?<from>.*) has flow rate=(?<rate>.*); (tunnels lead to valves|tunnel leads to valve) (?<tos>.*)$",
            IsRegex = true)]
        ItemDay16[] input)
    {
        var indexes = input.Select((x, i) => (x.from, i)).ToDictionary(x => x.from, x => x.i);

        var map = input.ToDictionary(x => x.from);

        var queue = new Queue<State2Day16>();
        var start = State2Day16.Create("AA", "AA", 0L);
        queue.Enqueue(start);

        var used = new Dictionary<State2Day16, (int rate, int time)>();
        used.Add(start, (0, 4));

        var i = 0;
        var sw = Stopwatch.StartNew();
        while (queue.Count > 0)
        {
            if ((i++) % 1000000 == 0)
            {
                Console.Out.WriteLine($"q={queue.Count} u={used.Count}");
            }

            var cur = queue.Dequeue();
            var (curRate, curTime) = used[cur];
            if (curTime >= 30)
                continue;

            foreach (var (next, addRate) in Nexts())
            {
                if (!used.TryGetValue(next, out var prev) || prev.rate < curRate + addRate ||
                    prev.rate == curRate + addRate && prev.time > curTime + 1)
                {
                    queue.Enqueue(next);
                    used[next] = (curRate + addRate, curTime + 1);
                }
            }

            IEnumerable<(State2Day16 next, int nextRate)> Nexts()
            {
                foreach (var to in map[cur.pos].tos)
                {
                    foreach (var to2 in map[cur.pos2].tos)
                        yield return (State2Day16.Create(to, to2, cur.openMask), 0);

                    var index2 = indexes[cur.pos2];
                    if ((cur.openMask & (1L << index2)) == 0L)
                    {
                        var addRate = (30 - (curTime + 1)) * map[cur.pos2].rate;
                        if (addRate > 0)
                            yield return (State2Day16.Create(to, cur.pos2, cur.openMask | (1L << index2)), addRate);
                    }
                }

                var index1 = indexes[cur.pos];
                if ((cur.openMask & (1L << index1)) == 0L)
                {
                    var addRate = (30 - (curTime + 1)) * map[cur.pos].rate;
                    if (addRate > 0)
                    {
                        foreach (var to2 in map[cur.pos2].tos)
                            yield return (State2Day16.Create(cur.pos, to2, cur.openMask | (1L << index1)), addRate);

                        var index2 = indexes[cur.pos2];
                        if (index2 != index1 && (cur.openMask & (1L << index2)) == 0L)
                        {
                            var addRate2 = (30 - (curTime + 1)) * map[cur.pos2].rate;
                            if (addRate2 > 0)
                                yield return (cur with { openMask = cur.openMask | (1L << index1) | (1L << index2) },
                                    addRate + addRate2);
                        }
                    }
                }
            }
        }

        sw.Elapsed.Out("Elapsed: ");
        used.Max(x => x.Value.rate).Out("Part 2 (slow): ");
    }

    record StateDay16(string pos, long openMask);

    static void Solve_16_1(
        [Template(
            "^Valve (?<from>.*) has flow rate=(?<rate>.*); (tunnels lead to valves|tunnel leads to valve) (?<tos>.*)$",
            IsRegex = true)]
        ItemDay16[] input)
    {
        var indexes = input.Select((x, i) => (x.from, i)).ToDictionary(x => x.from, x => x.i);

        var map = input.ToDictionary(x => x.from);

        var queue = new Queue<StateDay16>();
        var start = new StateDay16("AA", 0L);
        queue.Enqueue(start);

        var used = new Dictionary<StateDay16, (int rate, int time)>();
        used.Add(start, (0, 0));

        while (queue.Count > 0)
        {
            var cur = queue.Dequeue();
            var (curRate, curTime) = used[cur];
            if (curTime >= 30)
                continue;

            var curMap = map[cur.pos];
            foreach (var to in curMap.tos)
            {
                var next = cur with { pos = to };
                if (!used.TryGetValue(next, out var prev) || prev.rate < curRate ||
                    prev.rate == curRate && prev.time > curTime + 1)
                {
                    queue.Enqueue(next);
                    used[next] = (curRate, curTime + 1);
                }
            }

            var index = indexes[cur.pos];
            if ((cur.openMask & (1L << index)) == 0L)
            {
                var addRate = (30 - (curTime + 1)) * curMap.rate;
                if (addRate > 0)
                {
                    var next = cur with { openMask = cur.openMask | (1L << index) };
                    if (!used.TryGetValue(next, out var prev) || prev.rate < curRate + addRate ||
                        prev.rate == curRate + addRate && prev.time > curTime + 1)
                    {
                        queue.Enqueue(next);
                        used[next] = (curRate + addRate, curTime + 1);
                    }
                }
            }
        }

        used.Max(x => x.Value.rate).Out("Part 1: ");
    }

    record SensorDay15(long x, long y, long bx, long by)
    {
        public V Pos => new(x, y);
        public V Beacon => new(bx, by);
    }

    static void Solve_15_2(
        [Template("Sensor at x={x}, y={y}: closest beacon is at x={bx}, y={by}")]
        SensorDay15[] sensors)
    {
        const int limit = 4000000;
        var super = new R(0, limit);

        for (var ty = 0; ty < limit; ty++)
        {
            var ranges = new List<R>();

            foreach (var sensor in sensors)
            {
                var dist = sensor.Pos.MDistTo(sensor.Beacon);
                var dy = Abs(ty - sensor.Pos.Y);
                var dx = dist - dy;
                if (dx < 0)
                    continue;

                var r = new R(sensor.x - dx, sensor.x + dx);
                if (!r.Intersects(super))
                    continue;

                ranges.Add(r.IntersectWith(super));
            }

            ranges = ranges.Pack();
            if (ranges.Sum(x => x.Length) == super.Length)
                continue;

            if (ranges.Count != 2)
                throw new Exception();
            if (ranges.Sum(x => x.Length) != super.Length - 1)
                throw new Exception();

            var result = new V(ranges[0].End + 1, ty);
            Console.Out.WriteLine($"pos = {result}, result = {result.X * limit + result.Y}");
            return;
        }
    }

    static void Solve_15_1(
        [Template("Sensor at x={x}, y={y}: closest beacon is at x={bx}, y={by}")]
        SensorDay15[] sensors)
    {
        const long ty = 2000000L;

        var ranges = new List<R>();

        foreach (var sensor in sensors)
        {
            var dist = sensor.Pos.MDistTo(sensor.Beacon);
            var dy = Abs(ty - sensor.Pos.Y);
            var dx = dist - dy;
            if (dx < 0)
                continue;

            var s = sensor.x - dx;
            var e = sensor.x + dx;
            if (sensor.by == ty)
            {
                if (s == e)
                    continue;
                if (sensor.bx == s)
                    s++;
                if (sensor.bx == e)
                    e--;
            }

            ranges.Add(new R(s, e));
        }

        ranges = ranges.Pack();
        Console.Out.WriteLine(ranges.Sum(x => x.Length));
    }

    static void Solve_14_2(string[] input)
    {
        var lines = input
            .Select(x => x.Split(new[] { ' ', '-', '>' }, StringSplitOptions.RemoveEmptyEntries))
            .Select(x => x.Select(i => i.Parse<V>()).ToArray())
            .ToArray();

        var map = new HashSet<V>();
        foreach (var line in lines)
        {
            for (var i = 1; i < line.Length; i++)
            {
                var dv = (line[i] - line[i - 1]).Dir;
                for (var v = line[i - 1]; v != line[i]; v += dv)
                    map.Add(v);
                map.Add(line[i]);
            }
        }

        var floorY = map.Max(v => v.Y) + 2;

        var count = 0;
        while (NextDrop())
            count++;

        count.Out("");

        bool NextDrop()
        {
            var v = new V(500, 0);
            if (map.Contains(v))
                return false;

            while (true)
            {
                var next = v + new V(0, 1);
                if (next.Y == floorY)
                {
                    map.Add(v);
                    return true;
                }

                if (map.Contains(next))
                {
                    next = v + new V(-1, 1);
                    if (map.Contains(next))
                    {
                        next = v + new V(1, 1);
                        if (map.Contains(next))
                        {
                            map.Add(v);
                            return true;
                        }
                    }
                }

                v = next;
            }
        }
    }

    static void Solve_14_1(string[] input)
    {
        var lines = input
            .Select(x => x.Split(new[] { ' ', '-', '>' }, StringSplitOptions.RemoveEmptyEntries))
            .Select(x => x.Select(i => i.Parse<V>()).ToArray())
            .ToArray();

        var map = new HashSet<V>();
        foreach (var line in lines)
        {
            for (var i = 1; i < line.Length; i++)
            {
                var dv = (line[i] - line[i - 1]).Dir;
                for (var v = line[i - 1]; v != line[i]; v += dv)
                    map.Add(v);
                map.Add(line[i]);
            }
        }

        var minY = 0L;
        var maxY = map.Max(v => v.Y);
        var maxX = map.Max(v => v.X);
        var minX = map.Min(v => v.X);
        var mapRange = new Range(minX, minY, maxX, maxY);

        var count = 0;
        while (NextDrop())
            count++;

        count.Out("");

        bool NextDrop()
        {
            var v = new V(500, 0);
            if (map.Contains(v))
                return false;

            while (v.InRange(mapRange))
            {
                var next = v + new V(0, 1);
                if (map.Contains(next))
                {
                    next = v + new V(-1, 1);
                    if (map.Contains(next))
                    {
                        next = v + new V(1, 1);
                        if (map.Contains(next))
                        {
                            map.Add(v);
                            return true;
                        }
                    }
                }

                v = next;
            }

            return false;
        }
    }

    abstract record EntryDay13 : IComparable<EntryDay13>
    {
        public abstract int CompareTo(EntryDay13? other);

        public static EntryDay13 Parse(string s)
        {
            var start = 0;
            return Parse(s, ref start);
        }

        private static EntryDay13 Parse(string s, ref int cur)
        {
            if (s[cur] == '[')
            {
                var res = new ListEntryDay13(new List<EntryDay13>());
                cur++;
                while (s[cur] != ']')
                {
                    var item = Parse(s, ref cur);
                    res.Entries.Add(item);
                    if (s[cur] == ',')
                    {
                        cur++;
                        continue;
                    }

                    if (s[cur] != ']')
                        throw new Exception($"Bad list at {cur} - bad symbol {s[cur]}");
                }

                cur++;
                return res;
            }

            if (!char.IsDigit(s[cur]))
                throw new Exception($"Bad value at {cur} - bad symbol {s[cur]}");
            var value = 0;
            while (char.IsDigit(s[cur]))
            {
                value = value * 10 + (s[cur] - '0');
                cur++;
            }

            return new ValueEntryDay13(value);
        }
    }

    record ListEntryDay13(List<EntryDay13> Entries) : EntryDay13
    {
        public override int CompareTo(EntryDay13? other)
        {
            var otherList = other as ListEntryDay13 ?? new ListEntryDay13(new List<EntryDay13> { other! });
            for (var i = 0; i < Entries.Count; i++)
            {
                if (i < otherList.Entries.Count)
                {
                    var res = Entries[i].CompareTo(otherList.Entries[i]);
                    if (res != 0)
                        return res;
                }
                else
                    return 1;
            }

            if (Entries.Count < otherList.Entries.Count)
                return -1;

            return 0;
        }

        public override string ToString()
        {
            return $"[{string.Join(",", Entries)}]";
        }
    }

    record ValueEntryDay13(int Value) : EntryDay13
    {
        public override int CompareTo(EntryDay13? other)
        {
            if (other is ValueEntryDay13 valueEntry)
                return Comparer<int>.Default.Compare(Value, valueEntry.Value);
            return new ListEntryDay13(new List<EntryDay13> { this }).CompareTo(other);
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    static void Solve_13_2(params EntryDay13[][] regions)
    {
        var entries = regions.SelectMany(x => x).ToList();
        var divider1 = EntryDay13.Parse("[[2]]");
        var divider2 = EntryDay13.Parse("[[6]]");
        entries.Add(divider1);
        entries.Add(divider2);
        entries.Sort();
        Console.Out.WriteLine((entries.IndexOf(divider1) + 1) * (entries.IndexOf(divider2) + 1));
    }

    static void Solve_13_2_alt(params string[] input)
    {
        int Compare(JsonElement a, JsonElement b)
        {
            if (a.ValueKind == JsonValueKind.Number && b.ValueKind == JsonValueKind.Number)
                return Comparer<int>.Default.Compare(a.GetInt32(), b.GetInt32());

            var aList = a.ValueKind == JsonValueKind.Array ? a.EnumerateArray().ToList() : new List<JsonElement> { a };
            var bList = b.ValueKind == JsonValueKind.Array ? b.EnumerateArray().ToList() : new List<JsonElement> { b };
            for (var i = 0; i < Min(aList.Count, bList.Count); i++)
            {
                var res = Compare(aList[i], bList[i]);
                if (res != 0)
                    return res;
            }

            return Comparer<int>.Default.Compare(aList.Count, bList.Count);
        }

        var entries = input
            .Where(x => !string.IsNullOrEmpty(x))
            .Select(x => JsonDocument.Parse(x).RootElement)
            .ToList();
        var divider1 = JsonDocument.Parse("[[2]]").RootElement;
        var divider2 = JsonDocument.Parse("[[6]]").RootElement;
        entries.Add(divider1);
        entries.Add(divider2);
        entries.Sort(Compare);
        Console.Out.WriteLine((entries.IndexOf(divider1) + 1) * (entries.IndexOf(divider2) + 1));
    }

    static void Solve_13_1(params EntryDay13[][] regions)
    {
        var s = 0;
        for (var i = 0; i < regions.Length; i++)
        {
            var left = regions[i][0];
            var right = regions[i][1];
            var res = left.CompareTo(right);
            if (res == -1)
                s += i + 1;
        }

        Console.Out.WriteLine(s);
    }

    static void Solve_12(Map<char> map)
    {
        var s = map.All().Single(v => map[v] == 'S');
        var e = map.All().Single(v => map[v] == 'E');
        map[s] = 'a';
        map[e] = 'z';

        map.Bfs4(s, (c, n) => n - c > 1)
            .First(x => x.State == e)
            .Distance
            .Out("Part 1: ");

        map.Bfs4(map.All().Where(v => map[v] == 'a'), (c, n) => n - c > 1)
            .First(x => x.State == e)
            .Distance
            .Out("Part 2: ");
    }

    record Monkey(int Index, long[] Items, char Op, string Arg, long DivisibleBy, long IfTrue, long IfFalse);

    static void Solve_11_2([Template(@"Monkey {Index}:
  Starting items: {Items}
  Operation: new = old {Op} {Arg}
  Test: divisible by {DivisibleBy}
    If true: throw to monkey {IfTrue}
    If false: throw to monkey {IfFalse}")]
        params Monkey[] monkeysSource)
    {
        var monkeys = monkeysSource
            .Select(r =>
            {
                var queue = new Queue<long>(r.Items);
                return (queue, r.Op, Arg: r.Arg == "old" ? -1 : long.Parse(r.Arg), r.DivisibleBy, r.IfTrue, r.IfFalse);
            })
            .ToArray();

        var modulus = monkeys.Select(m => m.DivisibleBy).Aggregate((a, b) => a * b);

        var counters = new long[monkeys.Length];

        for (var i = 0; i < 10000; i++)
            NextRound();

        Console.Out.WriteLine(counters.OrderDescending().Take(2).Aggregate((a, b) => a * b));

        void NextRound()
        {
            var i = 0;
            foreach (var (queue, op, arg, divisibleBy, ifTrue, ifFalse) in monkeys)
            {
                while (queue.Count > 0)
                {
                    counters[i]++;
                    var level = queue.Dequeue();
                    var argValue = arg == -1 ? level : arg;
                    level = op switch
                    {
                        '+' => level + argValue,
                        '*' => level * argValue,
                        _ => throw new Exception()
                    };
                    level %= modulus;
                    if (level % divisibleBy == 0)
                        monkeys[ifTrue].queue.Enqueue(level);
                    else
                        monkeys[ifFalse].queue.Enqueue(level);
                }

                i++;
            }
        }
    }

    static void Solve_11_1([Template(@"Monkey {Index}:
  Starting items: {Items}
  Operation: new = old {Op} {Arg}
  Test: divisible by {DivisibleBy}
    If true: throw to monkey {IfTrue}
    If false: throw to monkey {IfFalse}")]
        params Monkey[] monkeysSource)
    {
        var monkeys = monkeysSource
            .Select(r =>
            {
                var queue = new Queue<long>(r.Items);
                return (queue, r.Op, Arg: r.Arg == "old" ? -1 : long.Parse(r.Arg), r.DivisibleBy, r.IfTrue, r.IfFalse);
            })
            .ToArray();

        var counters = new long[monkeys.Length];

        for (var i = 0; i < 20; i++)
            NextRound();

        Console.Out.WriteLine(counters.OrderDescending().Take(2).Aggregate((a, b) => a * b));

        void NextRound()
        {
            var i = 0;
            foreach (var (queue, op, arg, divisibleBy, ifTrue, ifFalse) in monkeys)
            {
                while (queue.Count > 0)
                {
                    counters[i]++;
                    var level = queue.Dequeue();
                    var argValue = arg == -1 ? level : arg;
                    level = op switch
                    {
                        '+' => level + argValue,
                        '*' => level * argValue,
                        _ => throw new Exception()
                    };
                    level /= 3;
                    if (level % divisibleBy == 0)
                        monkeys[ifTrue].queue.Enqueue(level);
                    else
                        monkeys[ifFalse].queue.Enqueue(level);
                }

                i++;
            }
        }
    }

    static void Solve_10(string[] input)
    {
        var lines = input
            .Select(x => x.Split())
            .ToArray();

        IEnumerable<int> Run()
        {
            var x = 1;
            foreach (var line in lines)
            {
                yield return x;
                if (line[0] == "addx")
                {
                    yield return x;
                    x += int.Parse(line[1]);
                }
            }
        }

        var checkSum = Run()
            .Select((x, i) => x * (i + 1))
            .TakeEvery(40, startFrom: 19)
            .Sum();
        Console.WriteLine($"Part 1: {checkSum}");

        Console.WriteLine("Part 2:");
        foreach (var xs in Run().Batch(40))
            Console.WriteLine(new string(xs.Select((x, i) => Abs(i % 40 - x) <= 1 ? '#' : '.').ToArray()));
    }

    static void Solve_9((char dir, int c)[] lines)
    {
        Console.WriteLine($"Part 1: {Simulate(2)}");
        Console.WriteLine($"Part 2: {Simulate(10)}");

        int Simulate(int knotsCount)
        {
            var knots = Enumerable.Repeat(new V(), knotsCount).ToArray();
            var used = new HashSet<V> { knots.Last() };
            foreach (var (dir, c) in lines)
            {
                for (var i = 0; i < c; i++)
                {
                    knots[0] += dir switch
                    {
                        'R' => new V(1, 0),
                        'L' => new V(-1, 0),
                        'D' => new V(0, 1),
                        'U' => new V(0, -1),
                        _ => throw new Exception()
                    };
                    for (var k = 1; k < knots.Length; k++)
                    {
                        var delta = knots[k - 1] - knots[k];
                        if (delta.CLen() > 1)
                            knots[k] += delta.Dir;
                    }

                    used.Add(knots.Last());
                }
            }

            return used.Count;
        }
    }

    static void Solve_8_2(Map<int> map)
    {
        var max = map
            .AllButBorder()
            .Select(v => map.Column(v.X).Skip((int)v.Y + 1).TakeUntil(n => map[n] >= map[v]).Count() *
                         map.Column(v.X).Take((int)v.Y).Reverse().TakeUntil(n => map[n] >= map[v]).Count() *
                         map.Row(v.Y).Skip((int)v.X + 1).TakeUntil(n => map[n] >= map[v]).Count() *
                         map.Row(v.Y).Take((int)v.X).Reverse().TakeUntil(n => map[n] >= map[v]).Count())
            .Max();

        Console.WriteLine(max);
    }

    static void Solve_8_1(Map<int> map)
    {
        var visible = new Map<bool>(map.sizeX, map.sizeY);

        foreach (var row in map.Rows())
        {
            var cur = -1;
            foreach (var v in row)
            {
                var next = map[v];
                if (next > cur)
                {
                    cur = next;
                    visible[v] = true;
                }
            }

            cur = -1;
            foreach (var v in row.Reverse())
            {
                var next = map[v];
                if (next > cur)
                {
                    cur = next;
                    visible[v] = true;
                }
            }
        }

        foreach (var column in map.Columns())
        {
            var cur = -1;
            foreach (var v in column)
            {
                var next = map[v];
                if (next > cur)
                {
                    cur = next;
                    visible[v] = true;
                }
            }

            cur = -1;
            foreach (var v in column.Reverse())
            {
                var next = map[v];
                if (next > cur)
                {
                    cur = next;
                    visible[v] = true;
                }
            }
        }

        Console.WriteLine(visible.All().Count(v => visible[v]));
    }

    class EntryDay7
    {
        public EntryDay7(bool isDir, EntryDay7? parent)
        {
            IsDir = isDir;
            Parent = parent;
            FlattenDirs = parent?.FlattenDirs ?? new List<EntryDay7>();
            if (isDir)
                FlattenDirs.Add(this);
        }

        public List<EntryDay7> FlattenDirs { get; }
        public bool IsDir { get; }
        public EntryDay7? Parent { get; }
        public long Size { get; set; }
        public Dictionary<string, EntryDay7> Children { get; } = new();
    }

    static void Solve_7(string[] input)
    {
        var lines = input
            .Select(x => x.Split())
            .ToArray();

        var root = new EntryDay7(true, null);
        var cur = root;
        foreach (var line in lines)
        {
            if (line[0] == "$")
            {
                if (line[1] == "cd")
                {
                    cur = line[2] switch
                    {
                        "/" => root,
                        ".." => cur.Parent!,
                        _ => cur.Children[line[2]]
                    };
                }
            }
            else
            {
                if (cur.Children.ContainsKey(line[1]))
                    continue;
                if (line[0] == "dir")
                    cur.Children.Add(line[1], new EntryDay7(true, cur));
                else
                {
                    var size = long.Parse(line[0]);
                    cur.Children.Add(line[1], new EntryDay7(false, cur) { Size = size });
                    for (var c = cur; c != null; c = c.Parent)
                        c.Size += size;
                }
            }
        }

        Console.WriteLine($"Part 1: {root.FlattenDirs.Where(x => x.Size <= 100000).Sum(x => x.Size)}");

        var spaceLeft = 70000000L - root.Size;
        var spaceToFree = 30000000L - spaceLeft;
        var dirToRemove = root.FlattenDirs.OrderBy(x => x.Size).SkipWhile(x => x.Size < spaceToFree).First();

        Console.WriteLine($"Part 2: {dirToRemove.Size}");
    }

    static void Solve_6(string[] lines)
    {
        var input = lines[0];

        Console.WriteLine($"Part 1: {Solve(4)}");
        Console.WriteLine($"Part 2: {Solve(14)}");

        int Solve(int count)
        {
            for (var i = 0; i < input.Length; i++)
            {
                if (input.Substring(i, count).ToHashSet().Count == count)
                    return i + count;
            }

            throw new Exception("No solution");
        }
    }

    static void Solve_5_2(string[] stackLines, (string, long num, string, long from, string, long to)[] moves)
    {
        var stacks = stackLines
            .SkipLast(1)
            .RotateCW()
            .TakeEvery(4, startFrom: 1)
            .Select(x => new Stack<char>(x.Where(c => c != ' ')))
            .ToArray();

        foreach (var (_, num, _, from, _, to) in moves)
        {
            var tmp = new Stack<char>();
            for (var i = 0; i < num; i++)
                tmp.Push(stacks[from - 1].Pop());
            for (var i = 0; i < num; i++)
                stacks[to - 1].Push(tmp.Pop());
        }

        Console.WriteLine(new string(stacks.Select(x => x.First()).ToArray()));
    }

    static void Solve_5_1(string[] stackLines, (string, long num, string, long from, string, long to)[] moves)
    {
        var stacks = stackLines
            .SkipLast(1)
            .RotateCW()
            .TakeEvery(4, startFrom: 1)
            .Select(x => new Stack<char>(x.Where(c => c != ' ')))
            .ToArray();

        foreach (var (_, num, _, from, _, to) in moves)
            for (var i = 0; i < num; i++)
                stacks[to - 1].Push(stacks[from - 1].Pop());

        Console.WriteLine(new string(stacks.Select(x => x.First()).ToArray()));
    }

    static void Solve_4((R, R)[] lines)
    {
        Console.WriteLine($"Part 2: {lines.Count(x => x.Item1.Contains(x.Item2) || x.Item2.Contains(x.Item1))}");
        Console.WriteLine($"Part 2: {lines.Count(x => x.Item1.Intersects(x.Item2))}");
    }

    static void Solve_3_2(string[] lines)
    {
        Console.WriteLine(lines
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

    static void Solve_3_1(string[] lines)
    {
        Console.WriteLine(lines
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

    static void Solve_2_2((char v1, char outcome)[] lines)
    {
        Console.WriteLine(lines
            .Select(x => (v1: x.v1 - 'A', outcome: x.outcome - 'X'))
            .Select(x => (x.v1, v2: (x.v1 + x.outcome + 2) % 3))
            .Select(x => x.v2 + 1 + (x.v2 - x.v1 + 4) % 3 * 3)
            .Sum());
    }

    static void Solve_2_1((char v1, char v2)[] lines)
    {
        Console.WriteLine(lines
            .Select(x => (v1: x.v1 - 'A', v2: x.v2 - 'X'))
            .Select(x => x.v2 + 1 + (x.v2 - x.v1 + 4) % 3 * 3)
            .Sum());
    }

    static void Solve_1(params long[][] regions)
    {
        Console.WriteLine($"Part 1: {regions.Select(x => x.Sum()).Max()}");
        Console.WriteLine($"Part 2: {regions.Select(x => x.Sum()).OrderDescending().Take(3).Sum()}");
    }
}