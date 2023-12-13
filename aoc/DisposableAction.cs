using IDisposable = System.IDisposable;

namespace aoc;

public record DisposableAction(System.Action Action) : IDisposable
{
    public void Dispose() => Action();
}
