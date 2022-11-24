using Bingus.Numerics;
using Bingus.Core.EntityComponentSystem;

namespace Bingus.Core.Components;

[Component("velocity")]
public struct VelocityComponent : IComponent<VelocityComponent>
{
    public Vector3<double> Velocity { get; init; }
}