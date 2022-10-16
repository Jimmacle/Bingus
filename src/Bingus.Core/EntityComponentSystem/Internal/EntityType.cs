using System.Buffers;
using System.Reflection;

namespace Bingus.Core.EntityComponentSystem.Internal;

internal sealed class EntityType : IEquatable<EntityType>
{
    private static readonly Dictionary<int, EntityType> Cache = new();
    private static readonly TypeNameComparer NameComparer = new();

    private readonly int _hashCode;

    public bool Equals(EntityType? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return GetHashCode().Equals(other.GetHashCode());
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((EntityType)obj);
    }

    public override int GetHashCode()
    {
        return _hashCode;
    }

    public static bool operator ==(EntityType? left, EntityType? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(EntityType? left, EntityType? right)
    {
        return !Equals(left, right);
    }

    private readonly Type[] _components;
    private string? _name;

    public ReadOnlySpan<Type> Components => _components;

    private EntityType(ReadOnlySpan<Type> componentIds)
    {
        _components = componentIds.ToArray();
        Array.Sort(_components, NameComparer);
        _hashCode = TypeHash(_components);
    }

    public static EntityType Create(ReadOnlySpan<Type> componentTypes)
    {
        var hash = TypeHash(componentTypes);
        if (Cache.TryGetValue(hash, out var value))
            return value;

        return Cache[hash] = new EntityType(componentTypes);
    }

    public EntityType With(ReadOnlySpan<Type> with)
    {
        var newLength = _components.Length + with.Length;
        using var arr = ArrayPool<Type>.Shared.RentDisposable(newLength);
        Array.Copy(_components, arr.Array, _components.Length);
        for (var i = 0; i < with.Length; i++)
            arr[_components.Length + i] = with[i];
        
        var eType = Create(arr);
        return eType;
    }

    public EntityType Without(Type[] without)
    {
        using var arr = ArrayPool<Type>.Shared.RentDisposable(_components.Length - without.Length);
        var dstIndex = 0;
        foreach (var type in _components)
        {
            if (without.Contains(type))
                continue;

            arr[dstIndex] = type;
            dstIndex++;
        }
        
        var eType = Create(arr);
        return eType;
    }

    public bool Match(EntityType from, Type with)
    {
        if (from._components.Length != _components.Length - 1)
            return false;

        if (!_components.Contains(with))
            return false;

        foreach (var type in _components)
            if (type != with && !from._components.Contains(type))
                return false;

        return true;
    }

    public bool Is(EntityType supertype)
    {
        foreach (var superComp in supertype._components)
        {
            if (!_components.Contains(superComp))
                return false;
        }

        return true;
    }

    public bool IsSupertypeOf(EntityType subtype)
    {
        return subtype.Is(this);
    }

    public bool Has<T>()
    {
        return _components.Contains(typeof(T));
    }
    
    public override string ToString()
    {
        var componentIds = _components.Select(x =>
            x.GetCustomAttribute<ComponentIdAttribute>()?.Id ??
            throw new InvalidOperationException("Component does not have an ID."));
        
        return _name ??= "[" + string.Join(", ", componentIds) + "]";
    }

    private static int TypeHash(ReadOnlySpan<Type> componentIds)
    {
        var hashCode = 0;
        foreach (var t in componentIds)
            hashCode = HashCode.Combine(hashCode, t);

        return hashCode;
    }
}