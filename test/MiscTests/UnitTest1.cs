using Bingus.Core.Components;
using Bingus.Core.EntityComponentSystem;
using Bingus.Core.Services;

namespace MiscTests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var entityName = "Bingus";
        var ecs = new ECS(new SequentialEntityIdProvider());
        var id = ecs.CreateEntity().With(new NameComponent(entityName)).Id;
        
        Assert.Equal(entityName, ecs.Get<NameComponent>(id).Name);
    }

    [Fact]
    public void Test2()
    {
        var ecs = new ECS(new SequentialEntityIdProvider());
        var entityIds = new List<EntityId>();

        foreach (var i in Enumerable.Range(0, 100))
        {
            var ent = ecs.CreateEntity().With(new NameComponent($"Entity {i}"));
            entityIds.Add(ent.Id);
        }

        var id = entityIds[69];
        var comp = ecs.Get<NameComponent>(id);
        Assert.Equal("Entity 69", comp.Name);
    }

    [Fact]
    public void Test3()
    {
        var type1 = EntityType.Create(typeof(EntityId), typeof(NameComponent));
        var type2 = EntityType.Create(typeof(EntityId), typeof(NameComponent));
        
        Assert.Equal(type1, type2);
    }

    [Fact]
    public void Test4()
    {
        var super = EntityType.Create(typeof(EntityId));
        var sub = EntityType.Create(typeof(EntityId), typeof(NameComponent));
        
        Assert.True(sub.Is(super));
        Assert.True(!super.Is(sub));
    }

    [Fact]
    public void Test5()
    {
        var ecs = new ECS(new SequentialEntityIdProvider());
        var expectedNames = Enumerable.Range(0, 10).Select(x => x.ToString()).ToArray();

        foreach (var i in Enumerable.Range(0, 5))
        {
            ecs.CreateEntity().With(new NameComponent($"{i}"));
            ecs.CreateEntity().With(new NameComponent($"{i + 5}")).With(new PositionComponent());
        }

        var names = ecs.Filter<NameComponent>().Select(x => x.Name).ToArray();
        
        Array.Sort(expectedNames);
        Array.Sort(names);
        Assert.Equal(expectedNames, names);
    }
    
    [Fact]
    public void Test6()
    {
        var ecs = new ECS(new SequentialEntityIdProvider());
        var expectedNames = Enumerable.Range(5, 5).Select(x => x.ToString()).ToArray();

        foreach (var i in Enumerable.Range(0, 5))
        {
            ecs.CreateEntity().With(new NameComponent($"{i}"));
            ecs.CreateEntity().With(new NameComponent($"{i + 5}")).With(new PositionComponent());
        }

        var names = ecs.Filter<NameComponent, PositionComponent>().Select(x => x.Item1.Name).ToArray();
        
        Array.Sort(expectedNames);
        Array.Sort(names);
        Assert.Equal(expectedNames, names);
    }
}