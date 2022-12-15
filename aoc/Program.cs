using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using static System.Math;

namespace aoc;

public class Program
{
    static void Main()
    {
        Runner.RunFile("day15.txt", Solve_15_1);
        Runner.RunFile("day15.txt", Solve_15_2);
    }

    record Sensor(long x, long y, long bx, long by)
    {
        public V Pos => new(x, y);
        public V Beacon => new(bx, by);
    }

    static void Solve_15_2(string[] input)
    {
        var sensors = input.ParseAllWithTemplate<Sensor>("Sensor at x={x}, y={y}: closest beacon is at x={bx}, y={by}");
        const int limit = 4000000;
        for (int ty = 0; ty < limit; ty++)
        {
            var ranges = new List<R>();

            foreach (var sensor in sensors)
            {
                var dist = sensor.Pos.MDistTo(sensor.Beacon);
                var dy = Abs(ty - sensor.Pos.Y);
                var dx = dist - dy;
                if (dx < 0)
                    continue;
                var ts = new V(sensor.x - dx, ty);
                var te = new V(sensor.x + dx, ty);
                ranges.Add(new R(ts.X, te.X));
            }

            ranges = ranges.Pack();

            var super = new R(0, limit);
            for (int rr = ranges.Count - 1; rr >= 0; rr--)
            {
                if (ranges[rr].Overlaps(super))
                    ranges[rr] = ranges[rr].IntersectWith(super);
                else
                    ranges.RemoveAt(rr);
            }

            if (ranges.Sum(x => x.Length) != super.Length)
            {
                if (ranges.Count != 2)
                    throw new Exception();
                if (ranges.Sum(x => x.Length) != super.Length - 1)
                    throw new Exception();
                var result = new V(ranges[0].End + 1, ty);
                Console.Out.WriteLine(result);
                Console.Out.WriteLine(result.X * limit + result.Y);
                return;
            }
        }
    }

    static void Solve_15_1(string[] input)
    {
        var sensors = input.ParseAllWithTemplate<Sensor>("Sensor at x={x}, y={y}: closest beacon is at x={bx}, y={by}");
        var ty = 2000000L;

        var ranges = new List<R>();

        foreach (var sensor in sensors)
        {
            var dist = sensor.Pos.MDistTo(sensor.Beacon);
            var dy = Abs(ty - sensor.Pos.Y);
            var dx = dist - dy;
            if (dx < 0)
                continue;
            var ts = new V(sensor.x - dx, ty);
            var te = new V(sensor.x + dx, ty);
            if (sensor.by == ty)
            {
                if (ts == te)
                    continue;
                if (sensor.Beacon == ts)
                    ts += new V(1, 0);
                if (sensor.Beacon == te)
                    te += new V(-1, 0);
            }

            ranges.Add(new R(ts.X, te.X));
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
            for (int i = 1; i < line.Length; i++)
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
            for (int i = 1; i < line.Length; i++)
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
        public abstract int CompareTo(EntryDay13 other);

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
        public override int CompareTo(EntryDay13 other)
        {
            var otherList = other as ListEntryDay13 ?? new ListEntryDay13(new List<EntryDay13> { other });
            for (int i = 0; i < Entries.Count; i++)
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
        public override int CompareTo(EntryDay13 other)
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
            for (int i = 0; i < Min(aList.Count, bList.Count); i++)
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
        for (int i = 0; i < regions.Length; i++)
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
            .First(x => x.Pos == e)
            .Distance
            .Out("Part 1: ");

        map.Bfs4(map.All().Where(v => map[v] == 'a'), (c, n) => n - c > 1)
            .First(x => x.Pos == e)
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
        Console.WriteLine($"Part 2: {lines.Count(x => x.Item1.Overlaps(x.Item2))}");
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