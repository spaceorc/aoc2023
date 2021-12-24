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
        var lines = File
            .ReadAllLines("input.txt");

        Console.WriteLine(0L);
    }

    static void Main_24()
    {
        var lines = File
            .ReadAllLines("day24.txt")
            .Select(x => x.Split())
            .ToArray();

        var res1 = new long[14];
        var res2 = new long[14];
        var cur = new List<(int inp, int val)>();
        var inp = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            if (string.Join(" ", lines[i]) == "div z 1")
            {
                cur.Add((inp++, int.Parse(lines[i + 11][2])));
            }
            else if (string.Join(" ", lines[i]) == "div z 26")
            {
                var dif = int.Parse(lines[i + 1][2]) + cur[^1].val;
                res1[cur[^1].inp] = 9 - Max(dif, 0);
                res1[inp] = 9 + Min(dif, 0);
                res2[cur[^1].inp] = 1 - Min(dif, 0);
                res2[inp] = 1 + Max(dif, 0);
                inp++;
                cur.RemoveAt(cur.Count - 1);
            }
        }

        var eval1 = Evaluate(res1);
        if (eval1.failedInput != -1 || eval1.res != 0)
            throw new Exception("Bad 1");
        
        var eval2 = Evaluate(res2);
        if (eval2.failedInput != -1 || eval2.res != 0)
            throw new Exception("Bad 2");
        
        Console.WriteLine($"part 1: {string.Join("", res1)}");
        Console.WriteLine($"part 2: {string.Join("", res2)}");

        (int failedInput, long res) Evaluate(long[] input)
        {
            var cur = 0;
            var registers = new long[] { 0, 0, 0, 0 }; // w, x, y ,z
            foreach (var line in lines)
            {
                var (op, register, value) = Args(line);
                switch (op)
                {
                    case "inp":
                        registers[register] = input[cur++];
                        break;
                    case "add":
                        registers[register] += value;
                        break;
                    case "mul":
                        registers[register] *= value;
                        break;
                    case "div":
                        if (value == 0)
                            return (cur - 1, long.MinValue);
                        registers[register] /= value;
                        break;
                    case "mod":
                        if (registers[register] < 0 || value <= 0)
                            return (cur - 1, long.MinValue);
                        registers[register] %= value;
                        break;
                    case "eql":
                        registers[register] = registers[register] == value ? 1 : 0;
                        break;
                    default:
                        throw new Exception($"Bad: {op}");
                }
            }

            return (-1, registers[^1]);

            (string op, int register, long b) Args(string[] line)
            {
                var value = line.Length >= 3
                    ? line[2][0] >= 'w' && line[2][0] <= 'z'
                        ? registers[line[2][0] - 'w']
                        : int.Parse(line[2])
                    : 0L;
                return (line[0], line[1][0] - 'w', value);
            }
        }
    }

    static void Main_23_2()
    {
/*
input:
#############
#...........#
###C#A#B#C###
  #D#C#B#A#
  #D#B#A#C#
  #D#D#B#A#
  #########

positions:
#############
#0123456789x#
###a#b#c#d###
  #A#B#C#D#
  #e#f#g#h#
  #E#F#G#H#
  #########
*/
        const string pods = "aAeEbBfFcCgGdDhH";
        var nears = new Dictionary<char, string>
        {
            { '0', "1" },
            { '1', "02" },
            { '2', "13a" },
            { '3', "24" },
            { '4', "35b" },
            { '5', "46" },
            { '6', "57c" },
            { '7', "68" },
            { '8', "79d" },
            { '9', "8x" },
            { 'x', "9" },
            { 'a', "2A" },
            { 'A', "ae" },
            { 'e', "AE" },
            { 'E', "e" },
            { 'b', "4B" },
            { 'B', "bf" },
            { 'f', "BF" },
            { 'F', "f" },
            { 'c', "6C" },
            { 'C', "cg" },
            { 'g', "CG" },
            { 'G', "g" },
            { 'd', "8D" },
            { 'D', "dh" },
            { 'h', "DH" },
            { 'H', "h" },
        };
        var energy = new long[] { 1, 1, 1, 1, 10, 10, 10, 10, 100, 100, 100, 100, 1000, 1000, 1000, 1000 };
        var state = (lastMoved: -1, moved: "0000000000000000", posiitions: "bDgHcCfGadBhAeEF");

        var queue = new PriorityQueue<(int lastMoved, string moved, string positions), long>();
        queue.Enqueue(state, 0);
        var used = new Dictionary<(int lastMoved, string moved, string positions), long>();
        used[state] = 0;
        while (queue.Count > 0)
        {
            var cur = queue.Dequeue();
            var curEnergy = used[cur];
            if (NormAll(cur.positions) == "AAAABBBBCCCCDDDD")
            {
                Console.WriteLine(curEnergy);
                Console.WriteLine($"used={used.Count}");
                break;
            }

            var nextStates = NextStates(cur.lastMoved, cur.moved, cur.positions);
            foreach (var next in nextStates)
            {
                if (!used.TryGetValue(next, out var prevEnergy))
                {
                    used[next] = curEnergy + energy[next.lastMoved];
                    queue.Enqueue(next, used[next]);
                }
                else if (curEnergy + energy[next.lastMoved] < prevEnergy)
                {
                    Console.WriteLine("WTF???");
                    used[next] = curEnergy + energy[next.lastMoved];
                    queue.Enqueue(next, used[next]);
                }
            }
        }

        List<(int lastMoved, string moved, string positions)> NextStates(int lastMoved, string moved,
            string positions)
        {
            var result = new List<(int lastMoved, string moved, string positions)>();
            char pos;
            if (lastMoved == -1)
            {
                for (int p = 0; p < 16; p++)
                {
                    pos = positions[p];
                    foreach (var near in nears[pos])
                    {
                        if (positions.Contains(near))
                            continue;
                        var nextMoved = moved[..p] + '1' + moved[(p + 1)..];
                        var nextPositions = positions[..p] + near + positions[(p + 1)..];
                        result.Add((p, nextMoved, nextPositions));
                    }
                }

                return result;
            }

            pos = positions[lastMoved];
            foreach (var near in nears[pos])
            {
                if (positions.Contains(near))
                    continue;
                if (pos is '2' or '4' or '6' or '8' && near is 'a' or 'b' or 'c' or 'd')
                {
                    if (Norm(pods[lastMoved]) != Norm(near))
                        continue;

                    var whoDeep = positions.IndexOf(Norm(near));
                    if (whoDeep != -1 && Norm(pods[whoDeep]) != Norm(near))
                        continue;

                    whoDeep = positions.IndexOf(Norm2(near));
                    if (whoDeep != -1 && Norm(pods[whoDeep]) != Norm(near))
                        continue;

                    whoDeep = positions.IndexOf(char.ToLower(Norm2(near)));
                    if (whoDeep != -1 && Norm(pods[whoDeep]) != Norm(near))
                        continue;
                }

                var nextPositions = positions[..lastMoved] + near + positions[(lastMoved + 1)..];
                result.Add((lastMoved, moved, nextPositions));
            }

            if (pos is '2' or '4' or '6' or '8')
                return result;

            if (moved[lastMoved] == '2')
            {
                if (Norm(pos) != Norm(pods[lastMoved]))
                    return result;

                if (pos != Norm2(pos))
                {
                    var whoDeep = positions.IndexOf(Norm2(pos));
                    if (whoDeep == -1)
                        return result;

                    if (pos != char.ToLower(Norm2(pos)))
                    {
                        whoDeep = positions.IndexOf(char.ToLower(Norm2(pos)));
                        if (whoDeep == -1)
                            return result;
                        if (pos != Norm(pos))
                        {
                            whoDeep = positions.IndexOf(Norm(pos));
                            if (whoDeep == -1)
                                return result;
                        }
                    }
                }
            }

            if (moved[lastMoved] == '1')
            {
                if (Norm(pos) is 'A' or 'B' or 'C' or 'D' && Norm(pos) != Norm(pods[lastMoved]))
                    return result;
            }


            for (int i = 0; i < 16; i++)
            {
                if (i == lastMoved)
                    continue;
                if (moved[i] == '2')
                    continue;

                if (moved[i] == '1')
                {
                    var whoDeep = positions.IndexOf(Norm2(pods[i]));
                    if (whoDeep != -1 && Norm(pods[whoDeep]) != Norm(pods[i]))
                        continue;
                    whoDeep = positions.IndexOf(char.ToLower(Norm2(pods[i])));
                    if (whoDeep != -1 && Norm(pods[whoDeep]) != Norm(pods[i]))
                        continue;
                    whoDeep = positions.IndexOf(Norm(pods[i]));
                    if (whoDeep != -1 && Norm(pods[whoDeep]) != Norm(pods[i]))
                        continue;
                    whoDeep = positions.IndexOf(char.ToLower(Norm(pods[i])));
                    if (whoDeep != -1)
                        continue;
                }

                pos = positions[i];
                foreach (var near in nears[pos])
                {
                    if (positions.Contains(near))
                        continue;

                    var nextMoved = moved[..i] + (char)(moved[i] + 1) + moved[(i + 1)..];
                    var nextPositions = positions[..i] + near + positions[(i + 1)..];
                    result.Add((i, nextMoved, nextPositions));
                }
            }

            return result;
        }

        static string NormAll(string s)
        {
            return new string(s.Select(Norm).ToArray());
        }

        static char Norm(char c)
        {
            if (char.IsDigit(c))
                return c;
            if (c == 'x')
                return c;
            c = char.ToUpper(c);
            if (c > 'D')
                c = (char)(c - ('E' - 'A'));
            return c;
        }

        static char Norm2(char c)
        {
            if (char.IsDigit(c))
                return c;
            if (c == 'x')
                return c;
            return (char)(Norm(c) + ('E' - 'A'));
        }
    }

    static void Main_23_1()
    {
/*
input:
#############
#...........#
###C#A#B#C###
  #D#D#B#A#
  #########

positions:
#############
#0123456789x#
###a#b#c#d###
  #A#B#C#D#
  #########
 */
        var nears = new Dictionary<char, string>
        {
            { '0', "1" },
            { '1', "02" },
            { '2', "13a" },
            { '3', "24" },
            { '4', "35b" },
            { '5', "46" },
            { '6', "57c" },
            { '7', "68" },
            { '8', "79d" },
            { '9', "8x" },
            { 'x', "9" },
            { 'a', "2A" },
            { 'A', "a" },
            { 'b', "4B" },
            { 'B', "b" },
            { 'c', "6C" },
            { 'C', "c" },
            { 'd', "8D" },
            { 'D', "d" },
        };
        var pods = "aAbBcCdD";
        var energy = new long[] { 1, 1, 10, 10, 100, 100, 1000, 1000 };
        var state = (lastMoved: -1, moved: "00000000", posiitions: "bDcCadAB");
        var queue = new PriorityQueue<(int lastMoved, string moved, string positions), long>();
        queue.Enqueue(state, 0);
        var used = new Dictionary<(int lastMoved, string moved, string positions), long>();
        used[state] = 0;
        while (queue.Count > 0)
        {
            var cur = queue.Dequeue();
            var curEnergy = used[cur];
            if (cur.positions.ToUpper() == "AABBCCDD")
            {
                Console.WriteLine(curEnergy);
                Console.WriteLine($"used={used.Count}");
                return;
            }

            var nextStates = NextStates(cur.lastMoved, cur.moved, cur.positions).ToArray();
            foreach (var next in nextStates)
            {
                if (!used.TryGetValue(next, out var prevEnergy) || curEnergy + energy[next.lastMoved] < prevEnergy)
                {
                    used[next] = curEnergy + energy[next.lastMoved];
                    queue.Enqueue(next, used[next]);
                }
            }
        }

        IEnumerable<(int lastMoved, string moved, string positions)> NextStates(int lastMoved, string moved,
            string positions)
        {
            char pos;
            if (lastMoved == -1)
            {
                for (int p = 0; p < 8; p++)
                {
                    pos = positions[p];
                    foreach (var near in nears[pos])
                    {
                        if (positions.Contains(near))
                            continue;
                        var nextMoved = moved[..p] + '1' + moved[(p + 1)..];
                        var nextPositions = positions[..p] + near + positions[(p + 1)..];
                        yield return (p, nextMoved, nextPositions);
                    }
                }

                yield break;
            }

            pos = positions[lastMoved];
            foreach (var near in nears[pos])
            {
                if (positions.Contains(near))
                    continue;
                if (pos is '2' or '4' or '6' or '8' && near is 'a' or 'b' or 'c' or 'd')
                {
                    if (char.ToLower(pods[lastMoved]) != near)
                        continue;

                    var whoDeep = positions.IndexOf(char.ToUpper(near));
                    if (whoDeep != -1 && char.ToLower(pods[whoDeep]) != near)
                        continue;
                }

                var nextPositions = positions[..lastMoved] + near + positions[(lastMoved + 1)..];
                yield return (lastMoved, moved, nextPositions);
            }

            if (pos is '2' or '4' or '6' or '8')
                yield break;

            if (moved[lastMoved] == '2')
            {
                if (pos != char.ToLower(pods[lastMoved]) && pos != char.ToUpper(pods[lastMoved]))
                    yield break;
            }

            for (int i = 0; i < 8; i++)
            {
                if (i == lastMoved)
                    continue;
                if (moved[i] == '2')
                    continue;

                if (moved[i] == '1')
                {
                    var whoDeep = positions.IndexOf(char.ToUpper(pods[i]));
                    if (whoDeep != -1 && char.ToLower(pods[whoDeep]) != char.ToLower(pods[i]))
                        continue;
                    whoDeep = positions.IndexOf(char.ToLower(pods[i]));
                    if (whoDeep != -1)
                        continue;
                }

                pos = positions[i];
                foreach (var near in nears[pos])
                {
                    if (positions.Contains(near))
                        continue;

                    var nextMoved = moved[..i] + (char)(moved[i] + 1) + moved[(i + 1)..];
                    var nextPositions = positions[..i] + near + positions[(i + 1)..];
                    yield return (i, nextMoved, nextPositions);
                }
            }
        }
    }

    static void Main_22_2()
    {
        var lines = File
            .ReadAllLines("day22.txt")
            .Select(x => x.Split(new[] { " ", "x=", "y=", "z=", ",", ".." }, StringSplitOptions.RemoveEmptyEntries))
            .Select(x => (state: x[0],
                cube: new Cube(long.Parse(x[1]), long.Parse(x[2]), long.Parse(x[3]), long.Parse(x[4]), long.Parse(x[5]),
                    long.Parse(x[6]))))
            .ToArray();

        var cubes = new List<Cube>();
        foreach (var line in lines)
        {
            if (line.state == "on")
            {
                var cubesToAdd = new List<Cube> { line.cube };
                while (cubesToAdd.Count > 0)
                {
                    for (var index = cubesToAdd.Count - 1; index >= 0; index--)
                    {
                        var broken = false;
                        var cubeToAdd = cubesToAdd[index];
                        for (var i = cubes.Count - 1; i >= 0; i--)
                        {
                            var cube = cubes[i];
                            if (!cube.IntersectsWith(cubeToAdd))
                                continue;
                            if (cubeToAdd.Overlaps(cube))
                            {
                                cubes[i] = cubes[^1];
                                cubes.RemoveAt(cubes.Count - 1);
                                continue;
                            }

                            var (intersection, inCube, inCubeToAdd) = Cube.Intersect(cube, cubeToAdd);
                            broken = true;
                            cubes[i] = intersection!;
                            cubes.AddRange(inCube);
                            cubesToAdd[index] = cubesToAdd[^1];
                            cubesToAdd.RemoveAt(cubesToAdd.Count - 1);
                            cubesToAdd.AddRange(inCubeToAdd);
                            break;
                        }

                        if (broken)
                            break;

                        cubes.Add(cubeToAdd);
                        cubesToAdd.RemoveAt(index);
                    }
                }
            }
            else
            {
                for (var i = cubes.Count - 1; i >= 0; i--)
                {
                    var cube = cubes[i];
                    if (!cube.IntersectsWith(line.cube))
                        continue;

                    cubes[i] = cubes[^1];
                    cubes.RemoveAt(cubes.Count - 1);
                    if (line.cube.Overlaps(cube))
                        continue;

                    cubes.AddRange(cube.Subtract(line.cube));
                }
            }
        }

        Console.WriteLine(cubes.Sum(c => c.Size()));
    }

    static void Main_22_1()
    {
        var lines = File
            .ReadAllLines("day22.txt")
            .Select(x => x.Split(new[] { " ", "x=", "y=", "z=", ",", ".." }, StringSplitOptions.RemoveEmptyEntries))
            .Select(x => (state: x[0], minx: long.Parse(x[1]), maxx: long.Parse(x[2]), miny: long.Parse(x[3]),
                maxy: long.Parse(x[4]), minz: long.Parse(x[5]), maxz: long.Parse(x[6])))
            .ToArray();

        var map = new HashSet<V3>();
        foreach (var line in lines)
        {
            var (state, minx, maxx, miny, maxy, minz, maxz) = line;
            if (Abs(line.maxx) > 50)
                continue;

            for (var x = minx; x <= maxx; x++)
            for (var y = miny; y <= maxy; y++)
            for (var z = minz; z <= maxz; z++)
            {
                if (state == "on")
                    map.Add(new V3(x, y, z));
                else
                    map.Remove(new V3(x, y, z));
            }
        }

        Console.WriteLine(map.Count);
    }

    static void Main_21_2()
    {
        var positions = File
            .ReadAllLines("day21.txt")
            .Select(x => x.Split(": ")[1])
            .Select(int.Parse)
            .ToArray();

        var counts = new Dictionary<(int pos, int score, int opos, int oscore, int player), long>
        {
            { (positions[0], 0, positions[1], 0, 0), 1 }
        };

        var queue = new PriorityQueue<(int pos, int score, int opos, int oscore, int player), int>();
        queue.Enqueue(counts.First().Key, 0);

        while (queue.Count > 0)
        {
            var cur = queue.Dequeue();
            var curCount = counts[cur];
            var (pos, score, opos, oscore, player) = cur;

            for (int i = 1; i <= 3; i++)
            for (int j = 1; j <= 3; j++)
            for (int k = 1; k <= 3; k++)
            {
                var nextPos = (pos + i + j + k - 1) % 10 + 1;
                var nextScore = score + nextPos;
                var nextKey = (opos, oscore, nextPos, nextScore, 1 - player);
                if (counts.ContainsKey(nextKey))
                    counts[nextKey] += curCount;
                else
                {
                    counts[nextKey] = curCount;
                    if (nextScore < 21)
                        queue.Enqueue(nextKey, nextScore + oscore);
                }
            }
        }

        var w1 = counts.Where(x => x.Key.oscore >= 21 && x.Key.player == 0).Sum(x => x.Value);
        var w2 = counts.Where(x => x.Key.oscore >= 21 && x.Key.player == 1).Sum(x => x.Value);

        Console.WriteLine(Math.Max(w1, w2));
    }

    static void Main_21_1()
    {
        var positions = File
            .ReadAllLines("day21.txt")
            .Select(x => x.Split(": ")[1])
            .Select(long.Parse)
            .ToArray();

        var dice = 1;
        var player = 0;
        var rolls = 0L;
        var score = new long[2];
        while (true)
        {
            var shift = dice + dice % 100 + 1 + (dice + 1) % 100 + 1;
            dice = (dice + 2) % 100 + 1;
            rolls += 3;

            positions[player] = (positions[player] + shift - 1) % 10 + 1;
            score[player] += positions[player];
            if (score[player] >= 1000)
            {
                Console.WriteLine(rolls * score[1 - player]);
                return;
            }

            player = 1 - player;
        }
    }

    static void Main_20()
    {
        var lines = File
            .ReadAllLines("day20.txt");

        var mask = lines[0];
        var rawMap = lines.Skip(2).ToArray();

        var map = new HashSet<V>();
        var range = new Range(V.Zero, new V(rawMap[0].Length - 1, rawMap.Length - 1));
        foreach (var v in range.All())
        {
            if (rawMap[v.Y][(int)v.X] == '#')
                map.Add(v);
        }

        Console.WriteLine($"part 1: {TransformMany(map, range, 2).Count}");
        Console.WriteLine($"part 2: {TransformMany(map, range, 50).Count}");

        HashSet<V> TransformMany(HashSet<V> map, Range range, int count)
        {
            for (var i = 0; i < count; i++)
                (map, range) = Transform(map, range, outOfRangeBit: i & 1);
            return map;
        }

        (HashSet<V> newMap, Range newRange) Transform(HashSet<V> map, Range range, int outOfRangeBit)
        {
            var newMap = new HashSet<V>();
            var newRange = range.Grow(1);
            foreach (var v in newRange.All())
            {
                var n = 0;
                for (var dy = -1; dy <= 1; dy++)
                for (var dx = -1; dx <= 1; dx++)
                {
                    var nv = v + new V(dx, dy);
                    n <<= 1;
                    if (map.Contains(nv))
                        n |= 1;
                    else if (!nv.InRange(range))
                        n |= outOfRangeBit;
                }

                if (mask[n] == '#')
                    newMap.Add(v);
            }

            return (newMap, newRange);
        }
    }

    static void Main_19()
    {
        var scans = File
            .ReadAllText("day19.txt")
            .Split("\n\n")
            .Select(x => x.Split("\n").Where(l => !string.IsNullOrEmpty(l)).Skip(1).Select(V3.Parse).ToArray())
            .ToList();


        var matches = new Dictionary<int, List<(int to, (V3 shift, int dir))>>();

        for (var i = 0; i < scans.Count; i++)
        {
            var list = new List<(int to, (V3 shift, int dir))>();
            matches[i] = list;
            for (var j = 0; j < scans.Count; j++)
            {
                if (i == j)
                    continue;
                var m = Match(scans[i], scans[j]);
                if (m.Count == 1)
                    list.Add((j, m[0]));
                else if (m.Count != 0)
                    throw new Exception();
            }
        }

        var queue = new Queue<int>();
        queue.Enqueue(0);
        var rots = new Dictionary<int, List<(V3 shift, int dir)>>();
        rots.Add(0, new List<(V3 shift, int dir)>());
        while (queue.Count > 0)
        {
            var cur = queue.Dequeue();
            var curRot = rots[cur];
            foreach (var (to, r) in matches[cur])
            {
                if (rots.ContainsKey(to))
                    continue;

                var next = curRot.ToList();
                next.Add(r);
                rots[to] = next;
                queue.Enqueue(to);
            }
        }

        var beacons = new HashSet<V3>();
        var scanners = new HashSet<V3>();
        foreach (var (scanner, rr) in rots)
        {
            foreach (var v in scans[scanner])
            {
                var r = v;
                for (var i = rr.Count - 1; i >= 0; i--)
                    r = Rotations3.Rotate(r, rr[i].dir) + rr[i].shift;
                beacons.Add(r);
            }

            {
                var r = V3.Zero;
                for (var i = rr.Count - 1; i >= 0; i--)
                    r = Rotations3.Rotate(r, rr[i].dir) + rr[i].shift;
                scanners.Add(r);
            }
        }

        var maxDist = 0L;
        foreach (var v1 in scanners)
        foreach (var v2 in scanners)
        {
            var dist = (v1 - v2).MLen();
            if (dist > maxDist)
                maxDist = dist;
        }

        Console.Out.WriteLine(beacons.Count);
        Console.Out.WriteLine(maxDist);


        List<(V3 shift, int dir)> Match(V3[] a, V3[] b)
        {
            var res = new List<(V3 shift, int dir)>();
            for (var dir = 0; dir < 24; dir++)
                res.AddRange(MatchShift(a, Rotate(b, dir)).Select(v => (v, dir)));

            return res;
        }

        List<V3> MatchShift(V3[] a, V3[] b)
        {
            var result = new List<V3>();
            var aSet = a.ToHashSet();
            var bSet = b.ToHashSet();
            for (var i = 0; i < a.Length; i++)
            for (var j = 0; j < b.Length; j++)
            {
                var shift = a[i] - b[j];
                var common = 0;
                var bad = false;
                for (var k = 0; k < b.Length; k++)
                {
                    var b2 = b[k] + shift;
                    if (aSet.Contains(b2))
                        common++;
                    else if (b2.CLen() <= 1000)
                    {
                        bad = true;
                        break;
                    }
                }

                var common2 = 0;
                for (var k = 0; k < a.Length; k++)
                {
                    var a2 = a[k] - shift;
                    if (bSet.Contains(a2))
                        common2++;
                    else if (a2.CLen() <= 1000)
                    {
                        bad = true;
                        break;
                    }
                }

                if (!bad)
                {
                    if (common != common2)
                        throw new Exception("WTF???");

                    if (common >= 12)
                        result.Add(shift);
                }
            }

            return result.Distinct().ToList();
        }

        V3[] Rotate(V3[] scan, int direction)
        {
            return scan.Select(v => Rotations3.Rotate(v, direction)).ToArray();
        }
    }


    static void Main_18()
    {
        var lines = File
            .ReadAllLines("day18.txt")
            .Select(Snailfish.Number.Read)
            .ToList();

        var res = lines[0].Clone();
        for (var i = 1; i < lines.Count; i++)
        {
            res = Snailfish.Number.Add(res, lines[i].Clone());
            res.Reduce();
        }

        Console.WriteLine($"part 1: {res.Magnitude()}");

        var max = 0L;
        for (var i = 0; i < lines.Count - 1; i++)
        for (var k = i + 1; k < lines.Count; k++)
        {
            var r = Snailfish.Number.Add(lines[i].Clone(), lines[k].Clone());
            r.Reduce();
            var m = r.Magnitude();
            if (m > max)
                max = m;
            r = Snailfish.Number.Add(lines[k].Clone(), lines[i].Clone());
            r.Reduce();
            m = r.Magnitude();
            if (m > max)
                max = m;
        }

        Console.WriteLine($"part 2: {max}");
    }

    static void Main_17()
    {
        var parts = File.ReadAllLines("day17.txt")[0]
            .Split(new[] { "=", "..", "," }, StringSplitOptions.RemoveEmptyEntries);
        var minx = long.Parse(parts[1]);
        var maxx = long.Parse(parts[2]);
        var miny = long.Parse(parts[4]);
        var maxy = long.Parse(parts[5]);

        var highest = 0L;
        var count = 0;
        for (var x = 0; x <= maxx; x++)
        for (var y = miny; y <= 1000; y++)
        {
            var vel = new V(x, y);
            if (Sim(vel, out var h))
            {
                count++;
                if (h > highest)
                {
                    highest = h;
                }
            }
        }

        Console.Out.WriteLine($"part 1: highest={highest}");
        Console.Out.WriteLine($"part 2: count={count}");

        bool Sim(V vel, out long h)
        {
            h = 0;
            var p = V.Zero;
            while (true)
            {
                p += vel;
                vel -= new V(Math.Sign(vel.X), 1);
                if (p.Y > h)
                    h = p.Y;

                if (p.X >= minx && p.X <= maxx && p.Y >= miny && p.Y <= maxy)
                    return true;

                if (p.X > maxx || p.Y < miny)
                    return false;
            }
        }
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

                foreach (var s in src)
                {
                    var num = s.Select(c => match[c - 'a']).ToArray();
                    var n = nums.Single(n => n.SetEquals(num));
                    var ni = Array.IndexOf(nums, n);
                    Console.Write(ni);
                }

                var r = 0;
                foreach (var s in dst)
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