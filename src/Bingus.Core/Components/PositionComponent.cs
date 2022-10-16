using Bingus.Core.EntityComponentSystem;

namespace Bingus.Core.Components;

[ComponentId("position")]
public readonly struct PositionComponent : IComponent<PositionComponent>
{
    public int X { get; init; }
    public int Y { get; init; }
}