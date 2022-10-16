using System.Drawing;
using Bingus.Core.EntityComponentSystem;

namespace Bingus.Core.Components;

[ComponentId("color")]
public readonly struct ColorComponent : IComponent<ColorComponent>
{
    public Color Color { get; init; }
}