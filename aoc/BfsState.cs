using System.Collections.Generic;
using System.Linq;

namespace aoc;

public record BfsState(V Pos, Dictionary<V, (V? Prev, int Distance)> PathMap)
{
    public int Distance => PathMap[Pos].Distance;
    
    public IEnumerable<V> PathBack()
    {
        for (V? c = Pos; c != null; c = PathMap[c.Value].Prev)
            yield return c.Value;
    }

    public IEnumerable<V> Path() => PathBack().Reverse();
}