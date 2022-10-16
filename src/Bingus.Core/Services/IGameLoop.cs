namespace Bingus.Core.Services;

public delegate void GameTickDel(TimeSpan dt, CancellationToken cancel);

public interface IGameLoop
{
    TimeSpan TickTime { get; }
    void Run(GameTickDel tickAction);
    Task StopAsync();
}