using System;
using System.Collections.Generic;

namespace aoc.Lib;

public static class Search
{
    public static IEnumerable<SearchPathItem<TState>> Bfs<TState>(
        IEnumerable<TState> startFrom,
        Func<TState, IEnumerable<TState>> getNextStates,
        Predicate<SearchPathItem<TState>>? filter = null
    )
        where TState : notnull
    {
        var queue = new Queue<TState>();
        var used = new Dictionary<TState, SearchPathItem<TState>>();
        foreach (var state in startFrom)
        {
            queue.Enqueue(state);
            used.Add(state, new SearchPathItem<TState>(state, 0, null));
        }

        while (queue.Count > 0)
        {
            var cur = queue.Dequeue();
            var curItem = used[cur];
            
            yield return curItem;
            
            foreach (var next in getNextStates(cur))
            {
                if (used.ContainsKey(next))
                    continue;
                
                var nextItem = new SearchPathItem<TState>(next, curItem.Distance + 1, curItem);
                if (filter?.Invoke(nextItem) == false)
                    continue;
                
                used.Add(next, nextItem);
                queue.Enqueue(next);
            }
        }
    }
    
    public static IEnumerable<SearchPathItem<TState>> Dijkstra<TState>(
        IEnumerable<TState> startFrom,
        Func<TState, IEnumerable<(TState state, long distance)>> getNextStates,
        Predicate<SearchPathItem<TState>>? filter = null
    )
        where TState : notnull
    {
        var queue = new PriorityQueue<TState, long>();
        var used = new Dictionary<TState, SearchPathItem<TState>>();
        foreach (var state in startFrom)
        {
            queue.Enqueue(state, 0);
            used.Add(state, new SearchPathItem<TState>(state, 0, null));
        }

        while (queue.TryDequeue(out var cur, out var curDistance))
        {
            var curItem = used[cur];
            if (curItem.Distance != curDistance)
                continue;
            
            yield return curItem;
            
            foreach (var (nextState, distance) in getNextStates(cur))
            {
                if (used.TryGetValue(nextState, out var prevItem) && prevItem.Distance <= curItem.Distance + distance)
                    continue;
                
                var nextItem = new SearchPathItem<TState>(nextState, curItem.Distance + distance, curItem);
                if (filter?.Invoke(nextItem) == false)
                    continue;
                
                used[nextState] = nextItem;
                queue.Enqueue(nextState, nextItem.Distance);
            }
        }
    }
}