using System.Numerics;
using Bingus.Core.EntityComponentSystem;

namespace Bingus.Core.Components;

[ComponentId("velocity")]
public struct VelocityComponent : IComponent<VelocityComponent>
{
    public Vector3 Velocity { get; init; }
}