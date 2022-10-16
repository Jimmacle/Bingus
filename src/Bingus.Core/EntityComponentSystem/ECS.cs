using System.Buffers;
using System.Collections;
using Bingus.Core.Components;
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
        var typeArr = ArrayPool<Type>.Shared.Rent(1);
        typeArr[0] = typeof(T);
        var type = EntityType.Create(new Span<Type>(typeArr, 0, 1));
        ArrayPool<Type>.Shared.Return(typeArr);
        
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
        var typeArr = ArrayPool<Type>.Shared.Rent(2);
        typeArr[0] = typeof(T1);
        typeArr[1] = typeof(T2);
        var type = EntityType.Create(new Span<Type>(typeArr, 0, 2));
        ArrayPool<Type>.Shared.Return(typeArr);
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
        //return new FilterEnumerable<T1, T2, T3>(this);
        
        var typeArr = ArrayPool<Type>.Shared.Rent(3);
        typeArr[0] = typeof(T1);
        typeArr[1] = typeof(T2);
        typeArr[2] = typeof(T3);
        var type = EntityType.Create(new Span<Type>(typeArr, 0, 3));
        ArrayPool<Type>.Shared.Return(typeArr);
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
        var typeArr = ArrayPool<Type>.Shared.Rent(1);
        typeArr[0] = typeof(T);
        var typeSpan = new Span<Type>(typeArr, 0, 1);

        if (_entityIndex.ContainsKey(entity))
        {
            var oldTable = _entityIndex[entity];
            var newType = oldTable.EntityType.With(typeSpan);
            var newTable = GetTable(newType);
            newTable.MoveFrom(oldTable, entity, component);
            _entityIndex[entity] = newTable;
        }
        else
        {
            var type = EntityType.Create(typeSpan);
            var table = GetTable(type);
            table.Add(entity, component);
            _entityIndex[entity] = table;
        }
        
        ArrayPool<Type>.Shared.Return(typeArr);
    }

    public EntityBuilder CreateEntity()
    {
        var id = _idProvider.Consume();
        AddComponent(id, id);
        return new EntityBuilder(this, id);
    }

    private readonly struct FilterEnumerable<T1, T2, T3> : IEnumerable<(T1, T2, T3)>
        where T1 : struct, IComponent<T1>
        where T2 : struct, IComponent<T2>
        where T3 : struct, IComponent<T3>
    {
        private readonly ECS _ecs;

        public FilterEnumerable(ECS ecs)
        {
            _ecs = ecs;
        }

        public IEnumerator<(T1, T2, T3)> GetEnumerator()
        {
            return new FilterEnumerator<T1, T2, T3>(_ecs);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    private struct FilterEnumerator<T1, T2, T3> : IEnumerator<(T1, T2, T3)>
        where T1 : struct, IComponent<T1>
        where T2 : struct, IComponent<T2>
        where T3 : struct, IComponent<T3>
    {
        private readonly ECS _ecs;
        private int _tableIndex = 0;
        private int _rowIndex = -1;
        private readonly EntityType _filterType;
    
        public FilterEnumerator(ECS ecs)
        {
            _ecs = ecs;

            var arr = ArrayPool<Type>.Shared.Rent(3);
            var arrSpan = new Span<Type>(arr, 0, 3)
            {
                [0] = typeof(T1),
                [1] = typeof(T2),
                [2] = typeof(T3)
            };
            _filterType = EntityType.Create(arrSpan);
            ArrayPool<Type>.Shared.Return(arr);
        }

        public bool MoveNext()
        {
            if (NoMoreEntities())
                return false;

            if (AtEndOfTable())
            {
                _rowIndex = -1;
                do
                {
                    if (NoMoreTables())
                        return false;
                    
                    _tableIndex++;
                } while (!CurrentTable.EntityType.Is(_filterType));
            }
            
            _rowIndex++;
            Current = (
                CurrentTable.Get<T1>(_rowIndex),
                CurrentTable.Get<T2>(_rowIndex),
                CurrentTable.Get<T3>(_rowIndex));
            return true;
        }

        private bool AtEndOfTable()
        {
            return _rowIndex == CurrentTable.Rows - 1;
        }

        private bool NoMoreEntities()
        {
            return AtEndOfTable() && NoMoreTables();
        }

        private bool NoMoreTables()
        {
            return _tableIndex == _ecs._tableList.Count - 1;
        }

        private EntityTable CurrentTable => _ecs._tableList[_tableIndex];

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public (T1, T2, T3) Current { get; private set; }

        object IEnumerator.Current => Current;

        public void Dispose() { }
    }
}