using System.Numerics;
using Bingus.Core.Components;
using Bingus.Core.EntityComponentSystem;

namespace Bingus.Core.Services;

public class PhysicsSystem : ISystem
{
    private ECS _ecs;
    
    public PhysicsSystem(ECS ecs)
    {
        _ecs = ecs;
    }
    
    public void Tick(TimeSpan dt)
    {
        foreach (var (id, transform, velocity) in _ecs.Filter<EntityId, TransformComponent, VelocityComponent>())
        {
            var translate = Matrix4x4.CreateTranslation(velocity.Velocity * new Vector3((float)dt.TotalSeconds));
            var newTransform = transform.Transform * translate;
            _ecs.Set(id, new TransformComponent { Transform = newTransform });
        }
    }
}