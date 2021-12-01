using System;
using System.IO;
using System.Linq;

namespace aoc
{
    public class Program
    {
        static void Main()
        {
            var nums = File.ReadAllLines("input.txt").Select(long.Parse).ToArray();
            var res = 0L;
            
            //
            
            Console.Out.WriteLine(res);
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