using System.Collections.Generic;
using System.Linq;

namespace aoc.Lib;

public static class StringListHelpers
{
    public static string[] Columns(this IEnumerable<string> lines)
    {
        var result = new List<string>();
        var linesArr = lines as string[] ?? lines.ToArray();
        for (var i = 0; i < linesArr[0].Length; i++)
            result.Add(new string(linesArr.Select(x => x[i]).ToArray()));

        return result.ToArray();
    }

    public static string[] RotateCW(this IEnumerable<string> lines)
    {
        return lines.Reverse().Columns();
    }

    public static string[] RotateCCW(this IEnumerable<string> lines, int count = 1)
    {
        return lines.RotateCW(3 * count);
    }

    public static string[] RotateCW(this IEnumerable<string> lines, int count)
    {
        var result = lines;
        for (var i = 0; i < count % 4; i++)
            result = result.RotateCW();

        return result.ToArray();
    }

    public static List<string[]> Regions(this IEnumerable<string> lines)
    {
        var result = new List<string[]>();
        var cur = new List<string>();
        foreach (var line in lines)
        {
            if (line != "")
                cur.Add(line);
            else if (cur.Count > 0)
            {
                result.Add(cur.ToArray());
                cur.Clear();
            }
        }

        if (cur.Count > 0)
            result.Add(cur.ToArray());

        return result;
    }
}
