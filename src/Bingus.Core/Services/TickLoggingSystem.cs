namespace Bingus.Core.Services;

public class TickLoggingSystem : ISystem
{
    private IGameLoop _loop;
    
    public TickLoggingSystem(IGameLoop loop)
    {
        _loop = loop;
    }
    
    public void Tick(TimeSpan dt)
    {
        Console.WriteLine($"E: {dt} A: {_loop.TickTime}");
    }
}