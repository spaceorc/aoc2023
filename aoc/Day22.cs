using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace aoc;

public static class Day22
{
    public static void Solve(string[] map, string[] pathInput)
    {
        SolvePart1(map, ParsePath(pathInput[0]));
        SolvePart2(map, ParsePath(pathInput[0]));
    }

    private static void SolvePart1(string[] map, (char rotate, int steps)[] path)
    {
        var curPos = new V(0, 0);
        var curDir = 0;
        var range = new Range(V.Zero, new V(map.Max(x => x.Length), map.Length));
        foreach (var (rotate, steps) in path)
        {
            if (rotate == 'L')
                curDir = (curDir + 3) % 4;
            else if (rotate == 'R')
                curDir = (curDir + 1) % 4;
            else
            {
                for (int i = 0; i < steps; i++)
                {
                    var nextPos = curPos;
                    do
                    {
                        nextPos += dirs[curDir];
                        nextPos = new V(nextPos.X.Mod(range.MaxX), nextPos.Y.Mod(range.MaxY));
                    } while (At(map, nextPos) == default);

                    if (At(map, nextPos) == '#')
                        break;

                    curPos = nextPos;
                }
            }
        }
        
        ((curPos.Y + 1) * 1000 + (curPos.X + 1) * 4 + curDir).Out("Part 1: ");
    }

    private static void SolvePart2(string[] map, (char rotate, int steps)[] path)
    {
        var faceSize = map.Min(x => x.Trim().Length);
        var facesMap = ParseFacesMap();
        var facesStarts = facesMap.Select(f => f * faceSize).ToArray();
        var faceRange = new Range(V.Zero, new V(faceSize - 1, faceSize - 1));
        var faceTransitions = BuildFaceTransitions();
        
        var curFace = 0;
        var curPos = new V(0, 0);
        var curDir = 0;
        
        foreach (var (rotate, steps) in path)
        {
            if (rotate == 'L')
                curDir = (curDir + 3) % 4;
            else if (rotate == 'R')
                curDir = (curDir + 1) % 4;
            else
            {
                for (int i = 0; i < steps; i++)
                {
                    var nextFace = curFace;
                    var nextDir = curDir;
                    var nextPos = curPos;

                    nextPos += dirs[curDir];
                    if (!nextPos.InRange(faceRange))
                        (nextFace, nextDir, nextPos) = MakeTransition(curFace, curDir, curPos);

                    if (IsWall(nextFace, nextPos))
                        break;

                    curFace = nextFace;
                    curDir = nextDir;
                    curPos = nextPos;
                }
            }
        }

        var realPos = ToMapPosition(curFace, curPos);
        ((realPos.Y + 1) * 1000 + (realPos.X + 1) * 4 + curDir).Out("Part 2: ");

        Dictionary<(int face, int dir), (int face, int dir)> BuildFaceTransitions()
        {
            var faceAndDirectionToNextFace = new Dictionary<(int face, int dir), int>();
            for (int face = 0; face < facesMap.Length; face++)
            {
                for (int dir = 0; dir < 4; dir++)
                {
                    var n = facesMap[face] + dirs[dir];
                    var nf = Array.IndexOf(facesMap, n);
                    if (nf != -1)
                        faceAndDirectionToNextFace.Add((face, dir), nf);
                }
            }

            var cubeCoords = new Dictionary<int, (V3 coord, int[] rotationDirs)>
            {
                [0] = (new V3(0, 0, 1), Array.Empty<int>())
            };
            var cubeRotations = new[]
            {
                RotateRight,
                RotateDown,
                RotateLeft,
                RotateUp,
            };

            while (cubeCoords.Count < facesMap.Length)
            {
                for (int face = 0; face < facesMap.Length; face++)
                {
                    if (!cubeCoords.TryGetValue(face, out var cubeCoord))
                        continue;

                    for (int dir = 0; dir < 4; dir++)
                    {
                        if (faceAndDirectionToNextFace.TryGetValue((face, dir), out var nextFace) &&
                            !cubeCoords.ContainsKey(nextFace))
                        {
                            var rotationDirs = cubeCoord.rotationDirs.Append((dir + 2) % 4).ToArray();
                            var coord = new V3(0, 0, 1);
                            foreach (var rotationDir in rotationDirs.Reverse())
                                coord = cubeRotations[(rotationDir + 2) % 4](coord);
                            cubeCoords[nextFace] = (coord, rotationDirs);
                        }
                    }
                }
            }

            for (int face = 0; face < facesMap.Length; face++)
            {
                var rotationDirs = cubeCoords[face].rotationDirs;
                var rotatedCoordToFace = cubeCoords.Select(x =>
                {
                    var coord = x.Value.coord;
                    foreach (var rotationDir in rotationDirs)
                        coord = cubeRotations[rotationDir](coord);
                    return (face: x.Key, coord);
                }).ToDictionary(x => x.coord, x => x.face);

                for (int dir = 0; dir < 4; dir++)
                {
                    if (faceAndDirectionToNextFace.ContainsKey((face, dir)))
                        continue;

                    var nextCoord = cubeRotations[dir](new V3(0, 0, 1));
                    var nextFace = rotatedCoordToFace[nextCoord];
                    faceAndDirectionToNextFace.Add((face, dir), nextFace);
                }
            }

            var dirByFacePair = faceAndDirectionToNextFace
                .ToDictionary(
                    x => (x.Key.face, x.Value),
                    x => x.Key.dir);

            return faceAndDirectionToNextFace
                .ToDictionary(
                    x => (x.Key.face, x.Key.dir),
                    x => (x.Value, (dirByFacePair[(x.Value, x.Key.face)] + 2) % 4));

            V3 RotateUp(V3 v)
            {
                return new V3(v.X, v.Z, -v.Y);
            }

            V3 RotateDown(V3 v)
            {
                return new V3(v.X, -v.Z, v.Y);
            }

            V3 RotateRight(V3 v)
            {
                return new V3(v.Z, v.Y, -v.X);
            }

            V3 RotateLeft(V3 v)
            {
                return new V3(-v.Z, v.Y, v.X);
            }
        }
        
        (int toFace, int toDir, V to) MakeTransition(int fromFace, int dir, V from)
        {
            var (toFace, toDir) = faceTransitions[(fromFace, dir)];
            var to = (from + dirs[dir]).Mod(faceSize);
            for (var dd = dir; dd != toDir; dd = (dd + 1) % 4)
                to = RotateCW(to);
            return (toFace, toDir, to);
        }

        V[] ParseFacesMap()
        {
            var result = new List<V>();
            for (int y = 0; y < 6; y++)
            for (int x = 0; x < 6; x++)
            {
                var v = new V(x, y);
                var pos = v * faceSize;
                if (At(map, pos) != default)
                    result.Add(v);
            }

            return result.ToArray();
        }

        V ToMapPosition(int face, V positionInsideFace)
        {
            return facesStarts[face] + positionInsideFace;
        }

        bool IsWall(int face, V v) => At(map, ToMapPosition(face, v)) == '#';

        V RotateCW(V v)
        {
            var shifted = v * 2 - new V(faceSize - 1, faceSize - 1);
            var rotated = new V(-shifted.Y, shifted.X);
            var shiftedBack = (rotated + new V(faceSize - 1, faceSize - 1)) / 2;
            return shiftedBack;
        }
    }

    private static V[] dirs = { new(1, 0), new(0, 1), new(-1, 0), new(0, -1) };

    private static char At(string[] map, V pos)
    {
        if (pos.Y < 0 || pos.Y >= map.Length)
            return default;
        if (pos.X < 0 || pos.X >= map[pos.Y].Length)
            return default;
        var ch = map[pos.Y][(int)pos.X];
        return ch is '#' or '.' ? ch : default;
    }

    private static (char rotate, int steps)[] ParsePath(string path)
    {
        return new Regex(@"\d+|L|R").Matches(path)
            .Select(x => x.Value)
            .Select(x => x switch
            {
                "L" or "R" => (x[0], 0),
                _ => (default, int.Parse(x))
            })
            .ToArray();
    }
}