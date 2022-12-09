using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static System.Math;

namespace aoc;

public class Program
{
    static void Main()
    {
        Main_4();
    }

    static void Main_9()
    {
        var lines = File
            .ReadAllLines("day9.txt")
            .ParseAll<(char dir, int c)>()
            .ToArray();

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

    static void Main_8_2()
    {
        var map = File
            .ReadAllLines("day8.txt")
            .ToMap<int>();

        var max = map
            .AllButBorder()
            .Select(v => map.Column(v.X).Skip((int)v.Y + 1).TakeUntil(n => map[n] >= map[v]).Count() *
                         map.Column(v.X).Take((int)v.Y).Reverse().TakeUntil(n => map[n] >= map[v]).Count() *
                         map.Row(v.Y).Skip((int)v.X + 1).TakeUntil(n => map[n] >= map[v]).Count() *
                         map.Row(v.Y).Take((int)v.X).Reverse().TakeUntil(n => map[n] >= map[v]).Count())
            .Max();

        Console.WriteLine(max);
    }

    static void Main_8_1()
    {
        var map = File
            .ReadAllLines("day8.txt")
            .ToMap<int>();

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

    class Entry
    {
        public Entry(bool isDir, Entry? parent)
        {
            IsDir = isDir;
            Parent = parent;
            FlattenDirs = parent?.FlattenDirs ?? new List<Entry>();
            if (isDir)
                FlattenDirs.Add(this);
        }

        public List<Entry> FlattenDirs { get; }
        public bool IsDir { get; }
        public Entry? Parent { get; }
        public long Size { get; set; }
        public Dictionary<string, Entry> Children { get; } = new();
    }

    static void Main_7()
    {
        var lines = File
            .ReadAllLines("day7.txt")
            .Select(x => x.Split())
            .ToArray();

        var root = new Entry(true, null);
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
                    cur.Children.Add(line[1], new Entry(true, cur));
                else
                {
                    var size = long.Parse(line[0]);
                    cur.Children.Add(line[1], new Entry(false, cur) { Size = size });
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

    static void Main_6()
    {
        var input = File.ReadAllLines("day6.txt")[0];

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

    static void Main_5_2()
    {
        var regions = File
            .ReadAllLines("day5.txt")
            .Regions();

        var stacks = regions[0]
            .SkipLast(1)
            .RotateCW()
            .EveryNth(4, startFrom: 1)
            .Select(x => new Stack<char>(x.Where(c => c != ' ')))
            .ToArray();

        var moves = regions[1]
            .ParseAll<(string, long num, string, long from, string, long to)>();

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

    static void Main_5_1()
    {
        var regions = File
            .ReadAllLines("day5.txt")
            .Regions();

        var stacks = regions[0]
            .SkipLast(1)
            .RotateCW()
            .EveryNth(4, startFrom: 1)
            .Select(x => new Stack<char>(x.Where(c => c != ' ')))
            .ToArray();

        var moves = regions[1]
            .ParseAll<(string, long num, string, long from, string, long to)>();

        foreach (var (_, num, _, from, _, to) in moves)
            for (var i = 0; i < num; i++)
                stacks[to - 1].Push(stacks[from - 1].Pop());

        Console.WriteLine(new string(stacks.Select(x => x.First()).ToArray()));
    }

    static void Main_4()
    {
        var lines = File
            .ReadAllLines("day4.txt")
            .Select(x => x.Parse<(R, R)>())
            .ToArray();

        Console.WriteLine($"Part 2: {lines.Count(x => x.Item1.Contains(x.Item2) || x.Item2.Contains(x.Item1))}");
        Console.WriteLine($"Part 2: {lines.Count(x => x.Item1.Overlaps(x.Item2))}");
    }

    static void Main_3_2()
    {
        Console.WriteLine(File
            .ReadAllLines("day3.txt")
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

    static void Main_3_1()
    {
        Console.WriteLine(File
            .ReadAllLines("day3.txt")
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

    static void Main_2_2()
    {
        Console.WriteLine(File
            .ReadAllLines("day2.txt")
            .ParseAll<(char v1, char outcome)>()
            .Select(x => (v1: x.v1 - 'A', outcome: x.outcome - 'X'))
            .Select(x => (x.v1, v2: (x.v1 + x.outcome + 2) % 3))
            .Select(x => x.v2 + 1 + (x.v2 - x.v1 + 4) % 3 * 3)
            .Sum());
    }

    static void Main_2_1()
    {
        Console.WriteLine(File
            .ReadAllLines("day2.txt")
            .ParseAll<(char v1, char v2)>()
            .Select(x => (v1: x.v1 - 'A', v2: x.v2 - 'X'))
            .Select(x => x.v2 + 1 + (x.v2 - x.v1 + 4) % 3 * 3)
            .Sum());
    }

    static void Main_1()
    {
        var regions = File
            .ReadAllLines("day1.txt")
            .Regions()
            .Select(r => r.ParseAll<long>().ToArray())
            .ToArray();

        Console.WriteLine($"Part 1: {regions.Select(x => x.Sum()).Max()}");
        Console.WriteLine($"Part 2: {regions.Select(x => x.Sum()).OrderDescending().Take(3).Sum()}");
    }
}