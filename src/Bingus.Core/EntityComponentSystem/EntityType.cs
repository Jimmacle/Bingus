using System.Buffers;
using System.Reflection;

namespace Bingus.Core.EntityComponentSystem;

internal sealed class EntityType : IEquatable<EntityType>
{
    private static Dictionary<int, EntityType> _cache = new();
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
        if (_cache.TryGetValue(hash, out var value))
            return value;

        return _cache[hash] = new EntityType(componentTypes);
    }

    public EntityType With(ReadOnlySpan<Type> with)
    {
        var newLength = _components.Length + with.Length;
        var arr = ArrayPool<Type>.Shared.Rent(newLength);
        Array.Copy(_components, arr, _components.Length);
        for (var i = 0; i < with.Length; i++)
            arr[_components.Length + i] = with[i];
        
        var eType = Create(new Span<Type>(arr, 0, newLength));
        ArrayPool<Type>.Shared.Return(arr);
        return eType;
    }

    public EntityType Without(Type[] without)
    {
        var arr = ArrayPool<Type>.Shared.Rent(_components.Length - without.Length);
        var dstIndex = 0;
        foreach (var type in _components)
        {
            if (without.Contains(type))
                continue;

            arr[dstIndex] = type;
            dstIndex++;
        }
        
        var eType = Create(arr);
        ArrayPool<Type>.Shared.Return(arr);
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
        return _name ??= "[" + string.Join(", ", _components.Select(x => x.GetCustomAttribute<ComponentIdAttribute>().Id)) + "]";
    }

    private static int TypeHash(ReadOnlySpan<Type> componentIds)
    {
        var hashCode = 0;
        foreach (var t in componentIds)
            hashCode = HashCode.Combine(hashCode, t);

        return hashCode;
    }
}