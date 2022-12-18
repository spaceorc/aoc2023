using System.Collections.Generic;
using System.Linq;

namespace aoc;

public record BfsState<TState>(TState State, int Distance, BfsState<TState>? Prev)
{
    public IEnumerable<TState> PathBack()
    {
        for (BfsState<TState>? c = this; c != null; c = c.Prev)
            yield return c.State;
    }

    public IEnumerable<TState> Path() => PathBack().Reverse();
}