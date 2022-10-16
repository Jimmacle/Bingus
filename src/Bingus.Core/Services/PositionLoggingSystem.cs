using Bingus.Core.Components;
using Bingus.Core.EntityComponentSystem;

namespace Bingus.Core.Services;

public class PositionLoggingSystem : ISystem
{
    private ECS _ecs;
    
    public PositionLoggingSystem(ECS ecs)
    {
        _ecs = ecs;
    }
    
    public void Tick(TimeSpan dt)
    {
        foreach (var (id, transform) in _ecs.Filter<EntityId, TransformComponent>())
        {
            Console.WriteLine($"Entity {id} transform:\n{transform.Transform}");
        }
    }
}