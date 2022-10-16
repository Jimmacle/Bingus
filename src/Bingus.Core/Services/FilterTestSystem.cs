using Bingus.Core.Components;
using Bingus.Core.EntityComponentSystem;

namespace Bingus.Core.Services;

public class FilterTestSystem : ISystem
{
    private ECS _ecs;
    
    public FilterTestSystem(ECS ecs)
    {
        _ecs = ecs;
    }
    
    public void Tick(TimeSpan dt)
    {
        Console.WriteLine(_ecs.Filter<EntityId, TransformComponent, VelocityComponent>().Count());
    }
}