using Bingus.Core.Components;
using Bingus.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Bingus.Core.EntityComponentSystem;

public sealed class ECS
{
    private int _defaultTableSize = 1;
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
        var type = new EntityType(typeof(T));
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
        var type = new EntityType(typeof(T1), typeof(T2));
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

    private EntityTable GetTable(EntityType type)
    {
        if (!_tables.TryGetValue(type, out var dict))
            dict = _tables[type] = new EntityTable(type, _defaultTableSize);

        return dict;
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
        if (_entityIndex.ContainsKey(entity))
        {
            var oldTable = _entityIndex[entity];
            var newType = oldTable.EntityType.With(typeof(T));
            var newTable = GetTable(newType);
            newTable.MoveFrom(oldTable, entity, component);
            _entityIndex[entity] = newTable;
        }
        else
        {
            var type = new EntityType(typeof(T));
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