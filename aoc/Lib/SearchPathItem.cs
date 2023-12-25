using System.Collections.Generic;
using System.Linq;

namespace aoc.Lib;

public record SearchPathItem<TState>(TState State, long Distance, SearchPathItem<TState>? Prev)
{
    public IEnumerable<SearchPathItem<TState>> PathBackX()
    {
        for (var c = this; c != null; c = c.Prev)
            yield return c;
    }

    public IEnumerable<TState> PathBack()
    {
        for (var c = this; c != null; c = c.Prev)
            yield return c.State;
    }

    public IEnumerable<TState> Path() => PathBack().Reverse();
}
