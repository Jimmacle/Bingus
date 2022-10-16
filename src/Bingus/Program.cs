// See https://aka.ms/new-console-template for more information

using System.Drawing;
using Bingus.Core;
using Bingus.Core.Components;
using Bingus.Core.EntityComponentSystem;
using Bingus.Core.Services;
using Microsoft.Extensions.DependencyInjection;

var builder = new EngineBuilder();
builder.Services.AddSingleton<IEntityIdProvider, RandomEntityIdProvider>();
builder.Services.AddScoped<ECS>();
builder.Services.AddScoped<IGameLoop>(_ => new FixedGameLoop(TimeSpan.FromSeconds(1 / 60d)));
builder.Services.AddScoped<ISystem, TickLoggingSystem>();
var engine = builder.Build();

var scene = engine.CreateScene();
var ecs = scene.ECS;
var rand = new Random(0);

for (var i = 0; i < 5; i++)
{
    ecs.CreateEntity()
        .With(new NameComponent($"EntityWithPositionAndColor {i}"))
        .With(new PositionComponent { X = rand.Next(), Y = rand.Next() })
        .With(new ColorComponent { Color = Color.Red });
    
    ecs.CreateEntity()
        .With(new NameComponent($"EntityWithPosition {i}"))
        .With(new PositionComponent { X = rand.Next(), Y = rand.Next() });

    ecs.CreateEntity()
        .With(new NameComponent($"Entity {i}"));
}

var entityNames = ecs.Filter<NameComponent>().Select(x => x.Name);
Console.WriteLine($"Entity names: {string.Join(", ", entityNames)}\n");

foreach (var ent in ecs.Filter<NameComponent, PositionComponent>())
{
    var (name, position) = ent;
    Console.WriteLine($"Entity with name {name.Name} is at position {position.X}, {position.Y}");
}

var entityColors = ecs.Filter<ColorComponent>().Select(x => x.Color);
Console.WriteLine($"\nEntity colors: {string.Join(", ", entityColors)}");

var entityIds = ecs.Filter<EntityId>();
Console.WriteLine($"\nEntity IDs: {string.Join(", ", entityIds)}");

while (Console.ReadKey().Key != ConsoleKey.Q)
{
    scene.Run();
    Console.ReadKey();
    await scene.StopAsync();
}