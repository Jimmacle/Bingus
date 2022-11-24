using Bingus.Core.EntityComponentSystem;

namespace Bingus.Core.Components;

[Component("name")]
public readonly struct NameComponent : IComponent<NameComponent>
{
    public string Name { get; }

    public NameComponent(string name)
    {
        Name = name;
    }
}