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
            var lines = File
                .ReadAllLines("input.txt");

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
            for (int i = 2; i < lines.Length; i += 6)
            {
                var b = new List<long[]>();
                for (int j = 0; j < 5; j++)
                {
                    b.Add(lines[i + j].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(long.Parse)
                        .ToArray());
                }

                foreach (var line in b)
                {
                    foreach (var num in line)
                    {
                        n2b[num].Add(boards.Count);
                    }
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
                    for (int r = 0; r < 5; r++)
                    {
                        for (int c = 0; c < 5; c++)
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
            for (int j = 0; j < lines[0].Length; j++)
            {
                var ones = 0;
                var zeros = 0;
                for (int i = 0; i < lines.Count; i++)
                {
                    {
                        if (lines[i][j] == '1')
                            ones++;
                        else
                            zeros++;
                    }
                }

                for (int i = lines.Count - 1; i >= 0; i--)
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
            for (int j = 0; j < lines[0].Length; j++)
            {
                var ones = 0;
                var zeros = 0;
                for (int i = 0; i < lines.Count; i++)
                {
                    {
                        if (lines[i][j] == '1')
                            ones++;
                        else
                            zeros++;
                    }
                }

                for (int i = lines.Count - 1; i >= 0; i--)
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
            for (int i = 0; i < lines.Length; i++)
            {
                for (int j = 0; j < lines[i].Length; j++)
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