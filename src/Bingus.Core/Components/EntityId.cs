using Bingus.Core.EntityComponentSystem;

namespace Bingus.Core.Components;

[Component("id")]
public readonly record struct EntityId : IComponent<EntityId>
{
    private readonly ulong _id;

    public EntityId(ulong id)
    {
        _id = id;
    }

    public override string ToString() => _id.ToString();
}