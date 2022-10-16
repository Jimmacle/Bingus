using Bingus.Core.Components;

namespace Bingus.Core.EntityComponentSystem;

/// <summary>
/// Provides a fluent API to construct an entity.
/// </summary>
public readonly ref struct EntityBuilder
{
    private readonly ECS _ecs;

    public EntityId Id { get; }

    internal EntityBuilder(ECS ecs, EntityId id)
    {
        _ecs = ecs;
        Id = id;
    }

    public EntityBuilder With<T>(in T component) where T : struct, IComponent<T>
    {
        _ecs.AddComponent(Id, component);
        return this;
    }
}