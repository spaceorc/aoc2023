namespace aoc;

public record R(long Start, long End)
{
    public bool Overlaps(R other) => Start <= other.End && End >= other.Start;
    public bool Contains(R other) => Start <= other.Start && End >= other.End;
}