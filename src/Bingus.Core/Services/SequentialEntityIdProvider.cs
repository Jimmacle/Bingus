using Bingus.Core.Components;
using Bingus.Core.EntityComponentSystem;

namespace Bingus.Core.Services;

public interface IEntityIdProvider
{
    EntityId Consume();
    void Release(EntityId id);
}

public sealed class SequentialEntityIdProvider : IEntityIdProvider
{
    private ulong _next;

    public EntityId Consume() => new(Interlocked.Increment(ref _next));

    public void Release(EntityId id)
    {
        // Not needed.
    }
}