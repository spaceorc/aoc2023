using System;
using IDisposable = System.IDisposable;

namespace aoc.Lib;

public record DisposableAction(Action Action) : IDisposable
{
    public void Dispose() => Action();
}
