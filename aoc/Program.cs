using System.Linq;

namespace aoc
{
    public class Program
    {
        private static void Main()
        {
            Runner.RunFile("day2.txt", Solve_2);
        }

        private static void Solve_2(
            [StructuredTemplate("Game {Id}: {Sets:[;]}")]
            (long id, (long n, string color)[][] sets)[] input
        )
        {
            var games = input
                .Select(x => (
                    x.id,
                    sets: x.sets
                        .Select(s => s.ToLookup(i => i.color, i => i.n))
                        .Select(l => (
                                R: l["red"].SingleOrDefault(),
                                G: l["green"].SingleOrDefault(),
                                B: l["blue"].SingleOrDefault()
                            )
                        )))
                .ToList();
            
            long Solve_Part1()
            {
                return games
                    .Where(x => x.sets.All(s => s is { R: <= 12, G: <= 13, B: <= 14 }))
                    .Select(x => x.id)
                    .Sum();
            }

            long Solve_Part2()
            {
                return games
                    .Select(x => x.sets.Max(s => s.R) * x.sets.Max(s => s.G) * x.sets.Max(s => s.B))
                    .Sum();
            }

            Solve_Part1().Out("Part 1: ");
            Solve_Part2().Out("Part 2: ");
        }
        
        private static void Solve_1(string[] input)
        {
            long Solve(bool isPart2)
            {
                var replacements1 = Enumerable
                    .Range(1, 9)
                    .Select(digit => (str: digit.ToString(), digit));

                var replacements2 = new[] { "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" }
                    .Select((str, i) => (str, digit: i + 1));

                var replacements = isPart2 ? replacements1.Concat(replacements2) : replacements1;
                return input
                    .Select(line => (
                            first: replacements.Select(x => (x.digit, p: line.IndexOf(x.str))).Where(x => x.p != -1)
                                .MinBy(r => r.p).digit,
                            last: replacements.Select(x => (x.digit, p: line.LastIndexOf(x.str))).Where(x => x.p != -1)
                                .MaxBy(r => r.p).digit
                        )
                    )
                    .Select(x => x.first * 10 + x.last)
                    .Sum();
            }

            Solve(false).Out("Part 1: ");
            Solve(true).Out("Part 2: ");
        }
    }
}