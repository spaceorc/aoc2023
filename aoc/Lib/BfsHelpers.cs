using System;
using System.Collections.Generic;

namespace aoc.Lib;

public static class BfsHelpers
{
    public static IEnumerable<BfsPathItem<TState>> Bfs<TState>(
        IEnumerable<TState> startFrom,
        Func<TState, IEnumerable<TState>> getNextStates,
        int maxDistance = int.MaxValue
    )
        where TState : notnull
    {
        var queue = new Queue<TState>();
        var used = new Dictionary<TState, BfsPathItem<TState>>();
        foreach (var state in startFrom)
        {
            queue.Enqueue(state);
            used.Add(state, new BfsPathItem<TState>(state, 0, null));
        }

        while (queue.Count > 0)
        {
            var cur = queue.Dequeue();
            var curBfsState = used[cur];
            yield return curBfsState;

            if (curBfsState.Distance >= maxDistance)
                continue;

            foreach (var next in getNextStates(cur))
            {
                if (used.ContainsKey(next))
                    continue;
                used.Add(next, new BfsPathItem<TState>(next, curBfsState.Distance + 1, curBfsState));
                queue.Enqueue(next);
            }
        }
    }
}