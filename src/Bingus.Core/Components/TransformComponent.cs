using System.Numerics;
using Bingus.Core.EntityComponentSystem;

namespace Bingus.Core.Components;

[Component("transform")]
public readonly struct TransformComponent : IComponent<TransformComponent>
{
    public Matrix4x4 Transform { get; init; }

    public TransformComponent()
    {
        Transform = Matrix4x4.Identity;
    }
}