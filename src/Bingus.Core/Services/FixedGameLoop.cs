using System.Diagnostics;

namespace Bingus.Core.Services;

public class FixedGameLoop : IGameLoop
{
    private readonly TimeSpan _dt;
    private CancellationTokenSource _stop = new();
    private readonly Stopwatch _sw = new();
    private readonly SemaphoreSlim _stopped = new(0);

    public FixedGameLoop(TimeSpan dt)
    {
        _dt = dt;
    }

    public TimeSpan TickTime { get; private set; }

    public void Run(GameTickDel tickAction)
    {
        if (!_stop.TryReset())
            _stop = new CancellationTokenSource();
        
        while (!_stop.IsCancellationRequested)
        {
            _sw.Restart();
            tickAction(_dt, _stop.Token);

            while (_sw.Elapsed < _dt)
            {
                // TODO don't busy wait
            }

            TickTime = _sw.Elapsed;
        }

        _stopped.Release();
    }

    public async Task StopAsync()
    {
        _stop.Cancel();
        await _stopped.WaitAsync();
    }
}