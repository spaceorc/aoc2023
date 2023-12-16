namespace aoc.Lib;

public record Walker(V Pos, Dir Dir)
{
    public Walker Forward(long k = 1) => this with { Pos = Pos + V.Dir(Dir) * k };
    public Walker TurnCW(long k = 1) => this with { Dir = Dir.RotateCW(k) };
    public Walker TurnCCW(long k = 1) => this with { Dir = Dir.RotateCCW(k) };
    public bool Inside<T>(Map<T> map) => map.Inside(Pos);
}