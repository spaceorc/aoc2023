using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace aoc;

public static class Day24AlternativeSolution
{
    public static void RunDynProg()
    {
        var lines = File.ReadAllLines("day24.txt").Select(Parse).ToArray();

        var res = Solve(0, new long[4], new Dictionary<(int, long, long, long, long), string?>());
        Console.WriteLine(res);

        string? Solve(int lineIndex, long[] regs, Dictionary<(int, long, long, long, long), string?> results)
        {
            var key = (lineIndex, regs[0], regs[1], regs[2], regs[3]);
            if (results.TryGetValue(key, out var res))
                return res;

            regs = regs.ToArray();
            for (var i = lineIndex; i < lines.Length; i++)
            {
                var (op, reg, otherReg, val) = lines[i];
                var arg = otherReg == -1 ? val : regs[otherReg];
                switch (op)
                {
                    case "inp":
                        // part 2: for (var n = 1; n <= 9; n++)
                        for (var n = 9; n >= 1; n--)
                        {
                            regs[reg] = n;
                            var number = Solve(i + 1, regs, results);
                            if (number != null)
                            {
                                results[key] = $"{n}{number}";
                                return $"{n}{number}";
                            }
                        }

                        results[key] = null;
                        return null;
                    case "add":
                        regs[reg] += arg;
                        break;
                    case "mul":
                        regs[reg] *= arg;
                        break;
                    case "div":
                        if (arg == 0)
                        {
                            results[key] = null;
                            return null;
                        }

                        regs[reg] /= arg;
                        break;
                    case "mod":
                        if (regs[reg] < 0 || arg <= 0)
                        {
                            results[key] = null;
                            return null;
                        }

                        regs[reg] %= arg;
                        break;
                    case "eql":
                        regs[reg] = regs[reg] == arg ? 1 : 0;
                        break;
                    default:
                        throw new Exception($"Bad op: {op}");
                }
            }

            if (regs[^1] == 0)
            {
                results[key] = "";
                return "";
            }

            results[key] = null;
            return null;
        }
    }

    public static void RunNaive()
    {
        var lines = File.ReadAllLines("day24.txt").Select(Parse).ToArray();
        
        // part 2: for (var number = 11111111111111; number <= 99999999999999; number++)
        for (var number = 99999999999999; number >= 0; number--)
        {
            if (number % 10000 == 0)
                Console.WriteLine(number);

            if (Solve(number))
            {
                Console.WriteLine(number);
                return;
            }
        }

        bool Solve(long number)
        {
            var digits = number.ToString().Select(x => x - '0').ToArray();
            var num = 0;
            var regs = new long[4];
            foreach (var (op, reg, otherReg, val) in lines)
            {
                var arg = otherReg == -1 ? val : regs[otherReg];
                switch (op)
                {
                    case "inp":
                        regs[reg] = digits[num++];
                        break;
                    case "add":
                        regs[reg] += arg;
                        break;
                    case "mul":
                        regs[reg] *= arg;
                        break;
                    case "div":
                        if (arg == 0)
                            return false;
                        regs[reg] /= arg;
                        break;
                    case "mod":
                        if (regs[reg] < 0 || arg <= 0)
                            return false;
                        regs[reg] %= arg;
                        break;
                    case "eql":
                        regs[reg] = regs[reg] == arg ? 1 : 0;
                        break;
                    default:
                        throw new Exception($"Bad op: {op}");
                }
            }

            return regs[^1] == 0;
        }
    }

    static (string op, int reg, int otherReg, long val) Parse(string line)
    {
        var split = line.Split();
        var otherReg = -1;
        var val = 0L;
        if (split.Length >= 3)
        {
            if (char.IsLetter(split[2][0]))
                otherReg = split[2][0] - 'w';
            else
                val = long.Parse(split[2]);
        }

        return (split[0], split[1][0] - 'w', otherReg, val);
    }
}