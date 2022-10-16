using System.Reflection;

namespace Bingus.Core.EntityComponentSystem;

public sealed class EntityType : IEquatable<EntityType>
{
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

    public EntityType(params Type[] componentIds)
    {
        _components = componentIds;
        Array.Sort(_components, NameComparer);
        _hashCode = _components.Aggregate(0, HashCode.Combine);
    }

    public EntityType With(params Type[] with)
    {
        var arr = new Type[_components.Length + with.Length];
        Array.Copy(_components, arr, _components.Length);
        Array.Copy(with, 0, arr, _components.Length, with.Length);
        return new EntityType(arr);
    }

    public EntityType Without(params Type[] without)
    {
        var arr = new Type[_components.Length - without.Length];
        var dstIndex = 0;
        foreach (var type in _components)
        {
            if (without.Contains(type))
                continue;

            arr[dstIndex] = type;
            dstIndex++;
        }

        return new EntityType(arr);
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
        return supertype._components.All(x => _components.Contains(x));
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
        return _name ??= "[" + string.Join(", ", _components.Select(x => x.GetCustomAttribute<ComponentIdAttribute>().Id)) + "]";
    }
}