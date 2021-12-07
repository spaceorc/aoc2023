using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace aoc
{
    public class Program
    {
        static void Main()
        {
            var ns = File
                .ReadAllLines("input.txt")
                .Select(long.Parse)
                .ToList();

            
            Console.WriteLine(0L);
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
                return ns.Sum(x => (1L + Math.Abs(x - o))*Math.Abs(x - o)/2L);
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
                .Select(l => l.Split(new []{" -> ", ","}, StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray())
                .Select(p => (new V(p[0], p[1]), new V(p[2], p[3])))
                .ToArray();

            var used = new Dictionary<V, int>();
            foreach (var (a, b) in lines)
            {
                // part 1
                // if (a.X != b.X && a.Y != b.Y)
                //     continue;
                foreach (var p in Helpers.MakeLine(a, b))
                    used[p] = used.GetOrDefault(p) + 1;
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
}