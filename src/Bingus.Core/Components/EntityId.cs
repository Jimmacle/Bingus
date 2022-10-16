using Bingus.Core.EntityComponentSystem;

namespace Bingus.Core.Components;

[ComponentId("id")]
public readonly struct EntityId : IEquatable<EntityId>, IComponent<EntityId>
{
    private readonly ulong _id;

    public EntityId(ulong id)
    {
        _id = id;
    }

    public override string ToString() => _id.ToString();
    
    #region Equality
    public bool Equals(EntityId other) => _id == other._id;
    public override bool Equals(object? obj) => obj is EntityId other && Equals(other);
    public override int GetHashCode() => _id.GetHashCode();
    public static bool operator ==(EntityId left, EntityId right) => left.Equals(right);
    public static bool operator !=(EntityId left, EntityId right) => !left.Equals(right);
    #endregion
}