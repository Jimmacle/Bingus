using Bingus.Core.EntityComponentSystem;

namespace Bingus.Core.Components;

[ComponentId("id")]
public readonly struct EntityId : IEquatable<EntityId>, IComponent<EntityId>
{
    public bool Equals(EntityId other)
    {
        return _id == other._id;
    }

    public override bool Equals(object? obj)
    {
        return obj is EntityId other && Equals(other);
    }

    public override int GetHashCode()
    {
        return _id.GetHashCode();
    }

    public static bool operator ==(EntityId left, EntityId right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(EntityId left, EntityId right)
    {
        return !left.Equals(right);
    }

    private readonly ulong _id;

    public EntityId(ulong id)
    {
        _id = id;
    }

    public override string ToString()
    {
        return _id.ToString();
    }
}