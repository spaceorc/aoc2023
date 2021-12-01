using System;
using System.IO;
using System.Linq;

namespace aoc
{
    public class Program
    {
        static void Main()
        {
        }
        
        static void Main_1_2()
        {
            var ns = File.ReadAllLines("day1.txt").Select(long.Parse).ToArray();
            var x = ns[0] + ns[1] + ns[2];
            var r = 0;
            for (int i = 3; i < ns.Length; i++)
            {
                var x2 = x + ns[i] - ns[i - 3];
                if (x2 > x)
                    r++;
                x = x2;
            }

            Console.Out.WriteLine(r);
        }

        static void Main_1_1()
        {
            var ns = File.ReadAllLines("day1.txt").Select(long.Parse).ToArray();
            var x = 0;
            for (int i = 1; i < ns.Length; i++)
            {
                if (ns[i] > ns[i - 1])
                    x++;
            }

            Console.Out.WriteLine(x);
        }
    }
}