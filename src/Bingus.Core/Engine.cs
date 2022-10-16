using Bingus.Core.EntityComponentSystem;
using Bingus.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Bingus.Core;

/// <summary>
/// Note: Scoped services are scoped by scene.
/// </summary>
public sealed class EngineBuilder
{
    public IServiceCollection Services { get; } = new ServiceCollection();

    public Engine Build()
    {
        return ActivatorUtilities.CreateInstance<Engine>(Services.BuildServiceProvider());
    }
}

public sealed class Engine
{
    public IServiceProvider Services { get; }

    public List<Scene> Scenes { get; } = new();
    
    [ActivatorUtilitiesConstructor]
    public Engine(IServiceProvider services)
    {
        Services = services;
    }

    public Scene CreateScene()
    {
        var scene = ActivatorUtilities.CreateInstance<Scene>(Services.CreateScope().ServiceProvider);
        Scenes.Add(scene);
        return scene;
    }
}

public class Scene
{
    public ECS ECS { get; }

    private readonly IEnumerable<ISystem> _systems;
    private readonly IGameLoop _loop;

    public Scene(ECS ecs, IGameLoop loop, IEnumerable<ISystem> systems)
    {
        ECS = ecs;
        _loop = loop;
        _systems = systems;
    }

    public void Run() => new Thread(() => _loop.Run(TickAction)).Start();
    public Task StopAsync() => _loop.StopAsync();

    private void TickAction(TimeSpan dt, CancellationToken cancel)
    {
        foreach (var system in _systems)
        {
            if (cancel.IsCancellationRequested)
                return;
            
            system.Tick(dt);   
        }
    }
}