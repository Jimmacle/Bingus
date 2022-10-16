// See https://aka.ms/new-console-template for more information

using System.Drawing;
using System.Numerics;
using Bingus.Core;
using Bingus.Core.Components;
using Bingus.Core.EntityComponentSystem;
using Bingus.Core.Services;
using Microsoft.Extensions.DependencyInjection;

var builder = new EngineBuilder();
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
        .With(new VelocityComponent { Velocity = new Vector3(1, 0, 0) });
}

while (Console.ReadKey().Key != ConsoleKey.Q)
{
    scene.Run();
    Console.ReadKey();
    await scene.StopAsync();
}