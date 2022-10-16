using System.Buffers;
using System.Collections;
using Bingus.Core.Components;
using Bingus.Core.EntityComponentSystem.Internal;
using Bingus.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Bingus.Core.EntityComponentSystem;

public sealed class ECS
{
    private int _defaultTableSize = 1;
    private readonly List<EntityTable> _tableList = new();
    private readonly Dictionary<EntityType, EntityTable> _tables = new();
    private readonly Dictionary<EntityId, EntityTable> _entityIndex = new();
    private readonly IEntityIdProvider _idProvider;

    [ActivatorUtilitiesConstructor]
    public ECS(IEntityIdProvider idProvider)
    {
        _idProvider = idProvider;
    }

    public IEnumerable<T> Filter<T>() where T : struct, IComponent<T>
    {
        using var types = ArrayPool<Type>.Shared.RentDisposable(1);
        types[0] = typeof(T);
        var type = EntityType.Create(types);

        foreach (var table in _tables)
        {
            if (!table.Key.Is(type))
                continue;

            for (var i = 0; i < table.Value.Rows; i++)
                yield return table.Value.Get<T>(i);
        }
    }
    
    public IEnumerable<(T1, T2)> Filter<T1, T2>() where T1 : struct, IComponent<T1> where T2 : struct, IComponent<T2>
    {
        using var types = ArrayPool<Type>.Shared.RentDisposable(2);
        types[0] = typeof(T1);
        types[1] = typeof(T2);
        var type = EntityType.Create(types);
        foreach (var table in _tables)
        {
            if (!table.Key.Is(type))
                continue;

            for (var i = 0; i < table.Value.Rows; i++)
            {
                var t1 = table.Value.Get<T1>(i);
                var t2 = table.Value.Get<T2>(i);
                yield return (t1, t2);
            }
        }
    }
    
    public IEnumerable<(T1, T2, T3)> Filter<T1, T2, T3>() where T1 : struct, IComponent<T1> where T2 : struct, IComponent<T2> where T3 : struct, IComponent<T3>
    {
        using var types = ArrayPool<Type>.Shared.RentDisposable(3);
        types[0] = typeof(T1);
        types[1] = typeof(T2);
        types[2] = typeof(T3);
        var type = EntityType.Create(types);
        foreach (var table in _tables)
        {
            if (!table.Key.Is(type))
                continue;

            for (var i = 0; i < table.Value.Rows; i++)
            {
                var t1 = table.Value.Get<T1>(i);
                var t2 = table.Value.Get<T2>(i);
                var t3 = table.Value.Get<T3>(i);
                
                yield return (t1, t2, t3);
            }
        }
    }

    private EntityTable GetTable(EntityType type)
    {
        if (!_tables.TryGetValue(type, out var table))
        {
            table = _tables[type] = new EntityTable(type, _defaultTableSize);
            _tableList.Add(table);
        }

        return table;
    }

    public T Get<T>(EntityId entity) where T : struct, IComponent<T>
    {
        var table = _entityIndex[entity];
        return table.Get<T>(entity);
    }

    public void Set<T>(EntityId entity, in T component) where T : struct, IComponent<T>
    {
        var table = _entityIndex[entity];
        table.Set(entity, component);
    }

    public void AddComponent<T>(EntityId entity, in T component) where T : struct, IComponent<T>
    {
        using var types = ArrayPool<Type>.Shared.RentDisposable(1);
        types[0] = typeof(T);

        if (_entityIndex.ContainsKey(entity))
        {
            var oldTable = _entityIndex[entity];
            var newType = oldTable.EntityType.With(types);
            var newTable = GetTable(newType);
            newTable.MoveFrom(oldTable, entity, component);
            _entityIndex[entity] = newTable;
        }
        else
        {
            var type = EntityType.Create(types);
            var table = GetTable(type);
            table.Add(entity, component);
            _entityIndex[entity] = table;
        }
    }

    public EntityBuilder CreateEntity()
    {
        var id = _idProvider.Consume();
        AddComponent(id, id);
        return new EntityBuilder(this, id);
    }
}