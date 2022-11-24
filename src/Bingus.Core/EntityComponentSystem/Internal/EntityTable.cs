using System.Runtime.CompilerServices;
using Bingus.Core.Components;

namespace Bingus.Core.EntityComponentSystem.Internal;

/// <summary>
/// Stores components for entities with the same archetype (set of components). This allows for fast iteration of
/// similar entities.
/// </summary>
internal sealed class EntityTable
{
    private int _capacity;
    private readonly Array[] _columns;
    private readonly Dictionary<EntityId, int> _entityIndex = new();
    private readonly Dictionary<Type, int> _componentIndex = new();
    public EntityType EntityType { get; }

    public int Rows { get; private set; }

    public EntityTable(EntityType entityType, int initialCapacity)
    {
        EntityType = entityType;
        _columns = new Array[entityType.Components.Length];
        _capacity = initialCapacity;
        
        for (var i = 0; i < entityType.Components.Length; i++)
        {
            _columns[i] = Array.CreateInstance(entityType.Components[i], initialCapacity);
            _componentIndex[entityType.Components[i]] = i;
        }
    }

    public void Add<T>(EntityId id, T component) where T : struct, IComponent<T>
    {
        if (_columns.Length > 1)
            throw new InvalidOperationException(
                "Cannot add a new entity to a table with more than 1 type of component.");
        
        EnsureCapacity(Rows + 1);
        ColumnInternal<T>()[^1] = component;
        _entityIndex[id] = Rows;
        Rows++;
    }

    public void MoveFrom<T>(EntityTable source, EntityId id, in T component) where T : struct, IComponent<T>
    {
        if (!EntityType.Match(source.EntityType, typeof(T)))
            throw new InvalidOperationException(
                "Entity does not have the correct components to be moved to this table.");
        
        EnsureCapacity(Rows + 1);

        var srcIndex = source._entityIndex[id];
        var dstIndex = Rows;
        
        foreach (var type in source.EntityType.Components)
        {
            var srcArray = source._columns[source._componentIndex[type]];
            var dstArray = _columns[_componentIndex[type]];
            Array.Copy(srcArray, srcIndex, dstArray, dstIndex, 1);
        }

        var column = ColumnInternal<T>();
        column[Rows] = component;
        
        source.Remove(id);
        _entityIndex[id] = Rows;
        Rows++;
    }

    /// <summary>
    /// Removes the entity from the table and moves the last entity in the table to its place to
    /// preserve memory contiguity.
    /// </summary>
    /// <param name="id">The <see cref="EntityId"/> of the entity to remove.</param>
    public void Remove(EntityId id)
    {
        var entIndex = _entityIndex[id];
        var lastIndex = Rows - 1;
        var lastEnt = Column<EntityId>()[^1];
        
        foreach (var col in _columns)
            Array.Copy(col, lastIndex, col, entIndex, 1);

        _entityIndex.Remove(id);
        if (_entityIndex.Count > 0)
            _entityIndex[lastEnt] = entIndex;
        Rows--;
    }

    private void EnsureCapacity(int capacity)
    {
        if (_capacity >= capacity)
            return;

        var newCapacity = _capacity * 2;
        
        for (var i = 0; i < _columns.Length; i++)
        {
            var column = _columns[i];
            var componentType = column.GetType().GetElementType()!;
            var newArray = Array.CreateInstance(componentType, newCapacity);
            column.CopyTo(newArray, 0);
            _columns[i] = newArray;
        }

        _capacity = newCapacity;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<T> Column<T>() where T : struct, IComponent<T>
    {
        return ColumnInternal<T>()[..Rows];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Span<T> ColumnInternal<T>() where T : struct, IComponent<T>
    {
        var index = _componentIndex[typeof(T)];
        return (T[])_columns[index];
    }

    public void Set<T>(EntityId entity, in T component) where T : struct, IComponent<T>
    {
        var column = ColumnInternal<T>();
        var index = _entityIndex[entity];
        column[index] = component;
    }
    
    public T Get<T>(EntityId entity) where T : struct, IComponent<T>
    {
        var column = Column<T>();
        var index = _entityIndex[entity];
        return column[index];
    }

    public T Get<T>(int index) where T : struct, IComponent<T>
    {
        var column = Column<T>();
        return column[index];
    }
}