using Bingus.Core.Components;
using Bingus.Core.EntityComponentSystem;

namespace Bingus.Core.Services;

public sealed class RandomEntityIdProvider : IEntityIdProvider
{
    private readonly Random _random = new();
    private readonly HashSet<EntityId> _consumedIds = new();

    public EntityId Consume()
    {
        EntityId id;
        do
        {
            id = new EntityId((ulong)_random.NextInt64());
        } while (_consumedIds.Contains(id));

        _consumedIds.Add(id);
        return id;
    }

    public void Release(EntityId id)
    {
        _consumedIds.Remove(id);
    }
}