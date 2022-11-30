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
        var nums = File
            .ReadAllLines("day1.txt")
            .Select(x =>
            {
                return long.Parse(x);
            })
            .ToArray();
        
        var res = 0L;
        

        Console.Out.WriteLine(res);
    }
}