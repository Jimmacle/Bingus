using System.Diagnostics;

namespace Bingus.Core.Services;

public delegate void GameTickDel(TimeSpan dt, CancellationToken cancel);

public interface IGameLoop
{
    TimeSpan TickTime { get; }
    void Run(GameTickDel tickAction);
    void Stop();
}

public class FixedGameLoop : IGameLoop
{
    private readonly TimeSpan _dt;
    private readonly CancellationTokenSource _stop = new();
    private readonly SemaphoreSlim _tickSemaphore = new(1);
    private readonly Stopwatch _sw = new();

    public FixedGameLoop(TimeSpan dt)
    {
        _dt = dt;
    }

    public TimeSpan TickTime { get; private set; }

    public void Run(GameTickDel tickAction)
    {
        while (!_stop.IsCancellationRequested)
        {
            _sw.Restart();
            tickAction(_dt, _stop.Token);
            _tickSemaphore.Wait();

            while (_sw.Elapsed < _dt)
            {
                // TODO don't busy wait
            }

            TickTime = _sw.Elapsed;
        }
    }

    public void Stop()
    {
        _stop.Cancel();
    }
}