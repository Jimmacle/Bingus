﻿using BenchmarkDotNet.Attributes;
using Bingus.Core;
using Bingus.Core.Components;
using Bingus.Core.EntityComponentSystem;
using Bingus.Core.Services;

namespace Benchmarks;

[MemoryDiagnoser]
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
    }

    [Benchmark]
    public int Filter() => _ecs.Filter<NameComponent, TransformComponent>().Count();

    [Benchmark]
    public int FilterEmpty() => _ecs.Filter<ColorComponent>().Count();
}