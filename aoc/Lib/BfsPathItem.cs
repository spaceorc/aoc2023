using System.Collections.Generic;
using System.Linq;

namespace aoc.Lib;

public record BfsPathItem<TState>(TState State, int Distance, BfsPathItem<TState>? Prev)
{
    public IEnumerable<TState> PathBack()
    {
        for (var c = this; c != null; c = c.Prev)
            yield return c.State;
    }

    public IEnumerable<TState> Path() => PathBack().Reverse();
}
