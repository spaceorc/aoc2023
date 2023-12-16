namespace aoc.Lib;

public static class DirExtensions
{
    public static Dir RotateCW(this Dir dir, long k = 1) => (Dir)((int)dir + k).Mod(4);
    public static Dir RotateCCW(this Dir dir, long k = 1) => (Dir)((int)dir - k).Mod(4);
}