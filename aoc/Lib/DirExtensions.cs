namespace aoc.Lib;

public static class DirExtensions
{
    public static Dir RotateCW(this Dir dir) => (Dir)(((int)dir + 1) % 4);
    public static Dir RotateCCW(this Dir dir) => (Dir)(((int)dir + 3) % 4);
}