using System.Diagnostics;
using System.Threading;

namespace Bingus.Core.Services;

public class FixedGameLoop : IGameLoop, IDisposable
{
    private readonly TimeSpan _dt;
    private CancellationTokenSource _stop = new();
    private readonly Stopwatch _sw = new();
    private readonly SemaphoreSlim _stopped = new(0);
    private readonly AutoResetEvent _tick = new(false);

    public FixedGameLoop(TimeSpan dt)
    {
        _dt = dt;
    }

    public TimeSpan TickTime { get; private set; }

    public void Run(GameTickDel tickAction)
    {
        // Throttles the spin wait to avoid burning up CPU for no reason.
        using var timer = new HighResolutionTimer(1);
        timer.Tick += () => _tick.Set();
        timer.Start();

        if (!_stop.TryReset())
            _stop = new CancellationTokenSource();
        
        while (!_stop.IsCancellationRequested)
        {
            _sw.Restart();
            tickAction(_dt, _stop.Token);
            
            while (_sw.Elapsed < _dt)
            {
                var remainingTime = _dt - _sw.Elapsed;
                if (remainingTime > TimeSpan.FromMilliseconds(1))
                    _tick.WaitOne();
            }
            
            Console.WriteLine(_sw.Elapsed.TotalMilliseconds);
            TickTime = _sw.Elapsed;
        }

        _stopped.Release();
    }

    public async Task StopAsync()
    {
        _stop.Cancel();
        await _stopped.WaitAsync();
    }

    public void Dispose()
    {
        _stop.Dispose();
        _stopped.Dispose();
        _tick.Dispose();
    }
}