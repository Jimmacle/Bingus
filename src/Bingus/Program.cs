using System.Diagnostics;
using System.Numerics;
using System.Reflection;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using Bingus.Core;
using Bingus.Core.Components;
using Bingus.Core.EntityComponentSystem;
using Bingus.Core.Services;
using Bingus.Numerics;
using Bingus.Numerics.Matrices;
using CodegenAnalysis;
using Microsoft.Extensions.DependencyInjection;

char.IsLetter()

var potato = 24;
var salad = 19;
var bun = 0;
Console.WriteLine($"{potato} {salad} {bun}");

var schnitzel = bun - salad + potato;
var burger = bun;
var roast = (potato - 4 * schnitzel) / 2;

Console.WriteLine($"{schnitzel} {burger} {roast}");

var potato2 = 4 * schnitzel + 2 * roast;
var salad2 = 3 * schnitzel + 2 * roast;
var bun2 = burger;
Console.WriteLine($"{potato2} {salad2} {bun2}");

/*if (Stopwatch.IsHighResolution)
{
    Console.WriteLine("Operations timed using the system's high-resolution performance counter.");
}
else
{
    Console.WriteLine("Operations timed using the DateTime class.");
}
        
long frequency = Stopwatch.Frequency;
Console.WriteLine("  Timer frequency in ticks per second = {0}",
    frequency);
long nanosecPerTick = (1000L*1000L*1000L) / frequency;
Console.WriteLine("  Timer is accurate within {0} nanoseconds",
    nanosecPerTick);
return;

var mulFloat = typeof(Matrix4<float>).GetMethod("op_Multiply", BindingFlags.Public | BindingFlags.Static);
var mulDouble = typeof(Matrix4<double>).GetMethod("op_Multiply", BindingFlags.Public | BindingFlags.Static);

var mf = CodegenInfo.Obtain(CompilationTier.Tier1, mulFloat, null, Matrix4<float>.Identity, Matrix4<float>.Identity);
var md = CodegenInfo.Obtain(CompilationTier.Tier1, mulDouble, null, Matrix4<double>.Identity, Matrix4<double>.Identity);

Console.WriteLine($"""
=== FLOAT ===
{mf}
=============

===DOUBLE ===
{md}
=============
""");
return;

new FixedGameLoop(TimeSpan.FromSeconds(1/60d)).Run((_, _) => { });
return;

var builder = new GameBuilder();
builder.Services.AddSingleton<IEntityIdProvider, RandomEntityIdProvider>();
builder.Services.AddScoped<ECS>();
builder.Services.AddScoped<IGameLoop>(_ => new FixedGameLoop(TimeSpan.FromSeconds(1/60d)));
//builder.Services.AddScoped<ISystem, TickLoggingSystem>();
//builder.Services.AddScoped<ISystem, PhysicsSystem>();
//builder.Services.AddScoped<ISystem, PositionLoggingSystem>();
builder.Services.AddScoped<ISystem, FilterTestSystem>();
var engine = builder.Build();
var scene = engine.CreateScene();

foreach (var i in Enumerable.Range(0, 10000))
{
    scene.ECS.CreateEntity()
        .With(new TransformComponent())
        .With(new VelocityComponent { Velocity = new Vector3<double>(1, 0, 0) });
}

while (Console.ReadKey().Key != ConsoleKey.Q)
{
    scene.Run();
    Console.ReadKey();
    await scene.StopAsync();
}*/