using System.Buffers;
using System.Numerics;
using BenchmarkDotNet.Attributes;
using Bingus.Core;
using Bingus.Core.Components;
using Bingus.Core.EntityComponentSystem;
using Bingus.Core.Services;
using Bingus.Numerics.Matrices;

namespace Benchmarks;

[MemoryDiagnoser]
public class Misc
{
    private static Matrix4<double> m1 = new(
        5, 2, 6, 1,
        0, 6, 2, 0,
        3, 8, 1, 4,
        1, 8, 5, 6);
    
    private static Matrix4<double> m2 = new(
        7, 5, 8, 0,
        1, 8, 2, 6,
        9, 4, 3, 8,
        5, 3, 7, 9);

    //[Benchmark]
    public Matrix4<float> MMulGenericFloat()
    {
        var m1 = new Matrix4<float>(
            5, 2, 6, 1,
            0, 6, 2, 0,
            3, 8, 1, 4,
            1, 8, 5, 6);
        
        var m2 = new Matrix4<float>(
            7, 5, 8, 0,
            1, 8, 2, 6,
            9, 4, 3, 8,
            5, 3, 7, 9);

        return m1 * m2;
    }
    
    [Benchmark]
    public Matrix4<double> MMulGenericDouble()
    {
        return m1 * m2;
    }
    
    //[Benchmark]
    public Matrix4x4 MMulNumericsFloat()
    {
        var m1 = new Matrix4x4(
            5, 2, 6, 1,
            0, 6, 2, 0,
            3, 8, 1, 4,
            1, 8, 5, 6);
        
        var m2 = new Matrix4x4(
            7, 5, 8, 0,
            1, 8, 2, 6,
            9, 4, 3, 8,
            5, 3, 7, 9);

        return m1 * m2;
    }
}

/*[MemoryDiagnoser]
public class ECSBench
{
    private ECS _ecs;
    
    public ECSBench()
    {
        var ecs = new ECS(new SequentialEntityIdProvider());

        foreach (var i in Enumerable.Range(0, 10000))
        {
            ecs.CreateEntity()
                .With(new NameComponent($"{i}"))
                .With(new TransformComponent());
        }

        _ecs = ecs;
        
        // Warm up array pool
        ArrayPool<Type>.Shared.Return(ArrayPool<Type>.Shared.Rent(16));
    }

    [Benchmark]
    public int Filter() => _ecs.Filter<NameComponent, TransformComponent>().Count();

    [Benchmark]
    public int FilterFaster() => _ecs.Filter<EntityId, NameComponent, TransformComponent>().Count();

    [Benchmark]
    public int FilterEmpty() => _ecs.Filter<ColorComponent>().Count();
}*/