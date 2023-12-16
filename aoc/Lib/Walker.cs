namespace aoc.Lib;

public record Walker(V Pos, Dir Dir)
{
    public Walker Forward(long k = 1) => this with { Pos = Pos + V.Dir(Dir) * k };
    public Walker TurnCW() => this with { Dir = Dir.RotateCW() };
    public Walker TurnCCW() => this with { Dir = Dir.RotateCCW() };
    public bool Inside<T>(Map<T> map) => map.Inside(Pos);
}