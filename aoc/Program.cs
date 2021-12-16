using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace aoc;

public class Program
{
    static void Main()
    {
        var lines = File.ReadAllLines("input.txt");
        
        Console.Out.WriteLine(0L);
    }

    static void Main_16()
    {
        var source = File.ReadAllLines("day16.txt")[0];
        var packet = Packet.ReadFromHex(source);
        Console.Out.WriteLine(packet.SumVersion());
        Console.Out.WriteLine(packet.Calculate());
    }

    static void Main_15()
    {
        var lines = File.ReadAllLines("day15.txt");
        var map0 = new Map<long>(lines[0].Length, lines.Length);
        foreach (var v in map0.All())
            map0[v] = long.Parse(lines[(int)v.Y][(int)v.X].ToString());

        Console.WriteLine($"part1={Solve(map0)}");
        Console.WriteLine($"part1={Solve(Repeat(map0, 5))}");

        long Solve(Map<long> map)
        {
            var queue = new PriorityQueue<V, long>();
            queue.Enqueue(V.Zero, 0);
            var used = new Dictionary<V, long> { { V.Zero, 0 } };
            while (queue.Count > 0)
            {
                var cur = queue.Dequeue();
                var curLen = used[cur];
                if (cur == map.BottomRight)
                    return curLen;
                foreach (var next in map.Nears(cur))
                {
                    var nextLen = curLen + map[next];
                    if (!used.TryGetValue(next, out var prev) || prev > nextLen)
                    {
                        queue.Enqueue(next, nextLen);
                        used[next] = nextLen;
                    }
                }
            }

            throw new Exception("No path");
        }

        Map<long> Repeat(Map<long> src, int times)
        {
            var map = new Map<long>(src.sizeX * times, src.sizeY * times);
            foreach (var v in map.All())
            {
                var delta = v.X / src.sizeX + v.Y / src.sizeY;
                map[v] = (src[new V(v.X % src.sizeX, v.Y % src.sizeY)] + delta - 1) % 9 + 1;
            }

            return map;
        }
    }

    static void Main_14()
    {
        var lines = File.ReadAllLines("day14.txt");
        var polymer = lines[0];
        var transforms = lines
            .Skip(2)
            .Select(x => x.Split(" -> "))
            .ToDictionary(x => x[0], x => new[] { x[0][0] + x[1], x[1] + x[0][1] });

        var pairs = new DefaultDict<string, long>();
        for (var i = 0; i < polymer.Length - 1; i++)
            pairs[polymer.Substring(i, 2)]++;

        Solve(10);
        Solve(40);

        void Solve(int transformsCount)
        {
            var result = pairs;
            for (var i = 0; i < transformsCount; i++)
                result = Transform(result);

            var counts = new DefaultDict<char, long>();
            foreach (var (pair, count) in result)
            {
                foreach (var c in pair)
                    counts[c] += count;
            }

            counts[polymer[0]]++;
            counts[polymer[^1]]++;

            Console.WriteLine((counts.Values.Max() - counts.Values.Min()) / 2);
        }

        DefaultDict<string, long> Transform(DefaultDict<string, long> source)
        {
            var result = new DefaultDict<string, long>();
            foreach (var (pair, count) in source)
            {
                if (!transforms.TryGetValue(pair, out var next))
                    result[pair] = count;
                else
                {
                    foreach (var n in next)
                        result[n] += count;
                }
            }

            return result;
        }
    }

    static void Main_13()
    {
        var lines = File.ReadAllLines("day13.txt");

        var dots = lines
            .TakeWhile(line => !string.IsNullOrEmpty(line))
            .Select(V.Parse)
            .ToHashSet();

        var instructions = lines
            .SkipWhile(line => !string.IsNullOrEmpty(line))
            .SkipWhile(string.IsNullOrEmpty)
            .Select(line => line.Split(new[] { " ", "=" }, StringSplitOptions.RemoveEmptyEntries))
            .Select(line => (dir: line[2], coord: int.Parse(line[3])))
            .ToList();

        foreach (var (dir, coord) in instructions)
        {
            dots = dir == "y"
                ? dots.Select(v => v.Y < coord ? v : new V(v.X, coord * 2 - v.Y)).ToHashSet()
                : dots.Select(v => v.X < coord ? v : new V(coord * 2 - v.X, v.Y)).ToHashSet();
            Console.WriteLine(dots.Count);
        }

        var minY = dots.Select(x => x.Y).Min();
        var maxY = dots.Select(x => x.Y).Max();
        var minX = dots.Select(x => x.X).Min();
        var maxX = dots.Select(x => x.X).Max();
        for (var y = minY; y <= maxY; y++)
        {
            for (var x = minX; x <= maxX; x++)
                Console.Write(dots.Contains(new V(x, y)) ? "X" : " ");
            Console.WriteLine();
        }
    }

    static void Main_12()
    {
        var lines = File
            .ReadAllLines("day12.txt")
            .Select(x => x.Split("-"))
            .ToArray();

        var edges = lines
            .Concat(lines.Select(x => x.Reverse().ToArray()))
            .Where(x => x[1] != "start")
            .ToLookup(x => x[0], x => x[1]);

        Console.WriteLine($"part1={Solve("start", new HashSet<string>(), true)}");
        Console.WriteLine($"part2={Solve("start", new HashSet<string>(), false)}");

        int Solve(string cur, ISet<string> used, bool reused)
        {
            var res = 0;
            foreach (var next in edges[cur])
            {
                if (next == "end")
                    res++;
                else if (char.IsUpper(next[0]))
                    res += Solve(next, used, reused);
                else if (used.Add(next))
                {
                    res += Solve(next, used, reused);
                    used.Remove(next);
                }
                else if (!reused)
                    res += Solve(next, used, true);
            }

            return res;
        }
    }

    static void Main_11()
    {
        var lines = File.ReadAllLines("day11.txt");
        var map = new Map<int>(lines[0].Length, lines.Length);
        foreach (var v in map.All())
            map[v] = int.Parse(lines[(int)v.Y][(int)v.X].ToString());

        var total = 0L;
        for (var i = 0;; i++)
        {
            var f = Sim();
            total += f;
            if (i == 99)
                Console.WriteLine($"part1={total}");
            if (f == map.totalCount)
            {
                Console.WriteLine($"part2={i + 1}");
                break;
            }
        }

        long Sim()
        {
            var queue = new Queue<V>();
            foreach (var v in map.All())
                queue.Enqueue(v);

            while (queue.Count > 0)
            {
                var v = queue.Dequeue();
                if (map[v] == -1)
                    continue;
                map[v]++;
                if (map[v] > 9)
                {
                    map[v] = -1;
                    foreach (var nv in map.Nears8(v))
                    {
                        if (map[nv] <= 9)
                            queue.Enqueue(nv);
                    }
                }
            }

            var res = 0;
            foreach (var v in map.All())
                if (map[v] == -1)
                {
                    res++;
                    map[v] = 0;
                }

            return res;
        }
    }

    static void Main_10()
    {
        var lines = File.ReadAllLines("day10.txt");

        var closeToOpenMatch = new Dictionary<char, char>
        {
            { '>', '<' },
            { ')', '(' },
            { '}', '{' },
            { ']', '[' },
        };

        var res1 = lines.Select(SolveOne1).Where(r => r != 0).Sum();
        Console.WriteLine($"res1 = {res1}");

        var res2 = lines.Select(SolveOne2).Where(r => r != 0).ToList();
        res2.Sort();
        Console.WriteLine($"res2 = {res2[res2.Count / 2]}");

        long SolveOne1(string s)
        {
            var score = new Dictionary<char, long>
            {
                { ')', 3 },
                { ']', 57 },
                { '}', 1197 },
                { '>', 25137 },
            };

            var stack = new Stack<char>();
            foreach (var c in s)
            {
                if (!closeToOpenMatch.TryGetValue(c, out var expectedOpen))
                    stack.Push(c);
                else
                {
                    var open = stack.Pop();
                    if (expectedOpen != open)
                        return score[c];
                }
            }

            return 0;
        }

        long SolveOne2(string s)
        {
            var score = new Dictionary<char, long>
            {
                { '(', 1 },
                { '[', 2 },
                { '{', 3 },
                { '<', 4 },
            };
            var stack = new Stack<char>();
            foreach (var c in s)
            {
                if (!closeToOpenMatch.TryGetValue(c, out var expectedOpen))
                    stack.Push(c);
                else
                {
                    var open = stack.Pop();
                    if (expectedOpen != open)
                        return 0;
                }
            }

            return stack.Aggregate(0L, (current, c) => current * 5 + score[c]);
        }
    }

    static void Main_9_2()
    {
        var lines = File.ReadAllLines("day9.txt");

        var map = new Map<int>(lines[0].Length, lines.Length);
        foreach (var v in map.All())
            map[v] = int.Parse(lines[(int)v.Y][(int)v.X].ToString());

        var used = new Map<bool>(map.sizeX, map.sizeY);

        var basins = new List<long>();
        foreach (var v in map.All())
        {
            if (used[v] || map[v] == 9)
                continue;

            var s = 1L;
            var queue = new Queue<V>();
            queue.Enqueue(v);
            used[v] = true;
            while (queue.Count > 0)
            {
                var cur = queue.Dequeue();
                foreach (var n in map.Nears(cur))
                {
                    if (map[n] != 9 && !used[n])
                    {
                        used[n] = true;
                        queue.Enqueue(n);
                        s++;
                    }
                }
            }

            basins.Add(s);
        }

        basins.Sort();


        Console.WriteLine(basins[^1] * basins[^2] * basins[^3]);
    }

    static void Main_9_1()
    {
        var lines = File.ReadAllLines("day9.txt");

        var map = new Map<int>(lines[0].Length, lines.Length);
        foreach (var v in map.All())
            map[v] = int.Parse(lines[(int)v.Y][(int)v.X].ToString());


        var r = map.All().Where(v => map.Nears(v).All(n => map[n] > map[v])).Sum(v => map[v] + 1);
        Console.WriteLine(r);
    }

    static void Main_8_2()
    {
        var lines = File
            .ReadAllLines("day8.txt")
            .Select(x => x.Split(" | "))
            .Select(x => (x[0].Split(), x[1].Split()))
            .ToList();

        var nums = new[]
        {
            "abcefg".Select(c => c - 'a').ToHashSet(),
            "cf".Select(c => c - 'a').ToHashSet(),
            "acdeg".Select(c => c - 'a').ToHashSet(),
            "acdfg".Select(c => c - 'a').ToHashSet(),
            "bcdf".Select(c => c - 'a').ToHashSet(),
            "abdfg".Select(c => c - 'a').ToHashSet(),
            "abdefg".Select(c => c - 'a').ToHashSet(),
            "acf".Select(c => c - 'a').ToHashSet(),
            "abcdefg".Select(c => c - 'a').ToHashSet(),
            "abcdfg".Select(c => c - 'a').ToHashSet(),
        };

        var res = 0;
        foreach (var (src, dst) in lines)
            res += SolveOne(src, dst);

        Console.WriteLine(res);

        int SolveOne(string[] src, string[] dst)
        {
            foreach (var match in Enumerable.Range(0, 7).ToArray().Permutations())
            {
                var good = src.Concat(dst)
                    .Select(s => s.Select(c => match[c - 'a']).ToArray())
                    .All(num => nums.Any(n => n.SetEquals(num)));

                if (!good)
                    continue;

                foreach (string s in src)
                {
                    var num = s.Select(c => match[c - 'a']).ToArray();
                    var n = nums.Single(n => n.SetEquals(num));
                    var ni = Array.IndexOf(nums, n);
                    Console.Write(ni);
                }

                var r = 0;
                foreach (string s in dst)
                {
                    var num = s.Select(c => match[c - 'a']).ToArray();
                    var n = nums.Single(n => n.SetEquals(num));
                    var ni = Array.IndexOf(nums, n);
                    r = r * 10 + ni;
                }

                Console.WriteLine($" | {r}");
                return r;
            }

            throw new Exception("WTF");
        }
    }

    static void Main_8_1()
    {
        var lines = File
            .ReadAllLines("day8.txt")
            .Select(x => x.Split(" | "))
            .Select(x => (x[0].Split(), x[1].Split()))
            .ToList();


        var res = lines.Sum(line => line.Item2.Count(x => new[] { 2, 3, 4, 7 }.Contains(x.Length)));
        Console.WriteLine(res);
    }

    static void Main_7()
    {
        var ns = File
            .ReadAllText("day7.txt")
            .Trim()
            .Split(",")
            .Select(int.Parse)
            .ToList();


        var res = Enumerable.Range(ns.Min(), ns.Max() - ns.Min() + 1).Select(Calc2).ToArray();
        Console.WriteLine(res.Min());

        int Calc1(int o)
        {
            return ns.Sum(x => Math.Abs(x - o));
        }

        long Calc2(int o)
        {
            return ns.Sum(x => (1L + Math.Abs(x - o)) * Math.Abs(x - o) / 2L);
        }
    }

    static void Main_6()
    {
        var ns = File.ReadAllText("day6.txt").Trim().Split(",").Select(long.Parse).ToList();

        const int days = 256; // 80 for part 1
        var counts = new long[9];
        foreach (var n in ns)
            counts[n]++;

        for (var day = 0; day < days; day++)
        {
            var zeros = counts[0];
            for (var i = 0; i < counts.Length - 1; i++)
                counts[i] = counts[i + 1];
            counts[6] += zeros;
            counts[8] = zeros;
        }

        Console.WriteLine(counts.Sum());
    }

    static void Main_5()
    {
        var lines = File
            .ReadAllLines("day5.txt")
            .Select(l =>
                l.Split(new[] { " -> ", "," }, StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray())
            .Select(p => (new V(p[0], p[1]), new V(p[2], p[3])))
            .ToArray();

        var used = new Dictionary<V, int>();
        foreach (var (a, b) in lines)
        {
            // part 1
            // if (a.X != b.X && a.Y != b.Y)
            //     continue;
            foreach (var p in Helpers.MakeLine(a, b))
                used[p] = used.GetValueOrDefault(p) + 1;
        }

        Console.Out.WriteLine(used.Count(x => x.Value > 1));
    }

    static void Main_4()
    {
        var lines = File.ReadAllLines("day4.txt");

        var numbers = lines[0].Split(',').Select(long.Parse).ToArray();

        var boards = new List<List<long[]>>();
        var bsum = new List<long>();
        var n2b = numbers.ToDictionary(x => x, _ => new List<int>());
        var usedRows = new Dictionary<int, int[]>();
        var usedCols = new Dictionary<int, int[]>();
        for (var i = 2; i < lines.Length; i += 6)
        {
            var b = new List<long[]>();
            for (var j = 0; j < 5; j++)
                b.Add(lines[i + j].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(long.Parse)
                    .ToArray());

            foreach (var line in b)
            {
                foreach (var num in line)
                    n2b[num].Add(boards.Count);
            }

            usedRows.Add(boards.Count, new int[5]);
            usedCols.Add(boards.Count, new int[5]);
            boards.Add(b);
            bsum.Add(b.SelectMany(xx => xx).Sum());
        }


        var usedb = new HashSet<int>();
        for (var ni = 0; ni < numbers.Length; ni++)
        {
            var number = numbers[ni];
            var bs = n2b[number];
            foreach (var bi in bs)
            {
                for (var r = 0; r < 5; r++)
                {
                    for (var c = 0; c < 5; c++)
                    {
                        var b = boards[bi];
                        if (b[r][c] == number)
                        {
                            usedRows[bi][r]++;
                            usedCols[bi][c]++;
                            bsum[bi] -= number;
                            if (usedRows[bi][r] == 5 || usedCols[bi][c] == 5)
                            {
                                if (usedb.Add(bi))
                                    Console.Out.WriteLine($"{bi} -> {number} = {bsum[bi] * number}");
                            }
                        }
                    }
                }
            }
        }
    }


    static void Main_3_2()
    {
        var original = File
            .ReadAllLines("day3.txt")
            .ToArray();


        var lines = original.ToList();
        var ox = "";
        for (var j = 0; j < lines[0].Length; j++)
        {
            var ones = 0;
            var zeros = 0;
            for (var i = 0; i < lines.Count; i++)
            {
                {
                    if (lines[i][j] == '1')
                        ones++;
                    else
                        zeros++;
                }
            }

            for (var i = lines.Count - 1; i >= 0; i--)
            {
                if (ones < zeros && lines[i][j] == '1'
                    || zeros <= ones && lines[i][j] == '0')
                {
                    lines[i] = lines[^1];
                    lines.RemoveAt(lines.Count - 1);
                }
            }

            if (lines.Count == 1)
            {
                ox = lines[0];
                break;
            }
        }

        lines = original.ToList();
        var co2 = "";
        for (var j = 0; j < lines[0].Length; j++)
        {
            var ones = 0;
            var zeros = 0;
            for (var i = 0; i < lines.Count; i++)
            {
                {
                    if (lines[i][j] == '1')
                        ones++;
                    else
                        zeros++;
                }
            }

            for (var i = lines.Count - 1; i >= 0; i--)
            {
                if (ones >= zeros && lines[i][j] == '1'
                    || zeros > ones && lines[i][j] == '0')
                {
                    lines[i] = lines[^1];
                    lines.RemoveAt(lines.Count - 1);
                }
            }

            if (lines.Count == 1)
            {
                co2 = lines[0];
                break;
            }
        }

        var oxv = Convert.ToUInt64(ox, 2);
        var co2v = Convert.ToUInt64(co2, 2);


        Console.Out.WriteLine(oxv * co2v);
    }

    static void Main_3_1()
    {
        var lines = File
            .ReadAllLines("day3.txt")
            .ToArray();


        var counts = new long[lines[0].Length];
        for (var i = 0; i < lines.Length; i++)
        {
            for (var j = 0; j < lines[i].Length; j++)
            {
                if (lines[i][j] == '1')
                    counts[j]++;
            }
        }

        var gammaRate = new string(counts.Select(x => x > lines.Length / 2 ? '1' : '0').ToArray());
        var epsilonRate = new string(counts.Select(x => x > lines.Length / 2 ? '0' : '1').ToArray());
        var g = Convert.ToUInt64(gammaRate, 2);
        var e = Convert.ToUInt64(epsilonRate, 2);


        Console.Out.WriteLine(g * e);
    }

    static void Main_2_2()
    {
        var lines = File
            .ReadAllLines("day2.txt")
            .Select(x => x.Split())
            .Select(x => (dir: x[0], val: long.Parse(x[1])))
            .ToArray();

        var p = new V3(0, 0, 0);

        foreach (var (dir, val) in lines)
        {
            p += dir switch
            {
                "up" => new V3(0, 0, -val),
                "down" => new V3(0, 0, val),
                "forward" => new V3(val, p.Z * val, 0),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        Console.Out.WriteLine(p.X * p.Y);
    }

    static void Main_2_1()
    {
        var lines = File
            .ReadAllLines("day2.txt")
            .Select(x => x.Split())
            .Select(x => (dir: x[0], val: long.Parse(x[1])))
            .ToArray();

        var p = new V();

        foreach (var (dir, val) in lines)
        {
            p += dir switch
            {
                "up" => new V(0, -val),
                "down" => new V(0, val),
                "forward" => new V(val, 0),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        Console.Out.WriteLine(p.X * p.Y);
    }

    static void Main_1_2()
    {
        var nums = File.ReadAllLines("day1.txt").Select(long.Parse).ToArray();
        var x = nums[0] + nums[1] + nums[2];
        var res = 0L;
        for (var i = 3; i < nums.Length; i++)
        {
            var x2 = x + nums[i] - nums[i - 3];
            if (x2 > x)
                res++;
            x = x2;
        }

        Console.Out.WriteLine(res);
    }

    static void Main_1_1()
    {
        var nums = File.ReadAllLines("day1.txt").Select(long.Parse).ToArray();
        var res = 0L;
        for (var i = 1; i < nums.Length; i++)
        {
            if (nums[i] > nums[i - 1])
                res++;
        }

        Console.Out.WriteLine(res);
    }
}