using System.Reflection;

namespace Bingus.Core.EntityComponentSystem;

public interface IComponent<T> where T : struct
{
    private static string? _id;
    public static string Id => _id ??= typeof(T).GetCustomAttribute<ComponentIdAttribute>()?.Id 
                                       ?? throw new InvalidOperationException("Component does not define an ID.");
}

[AttributeUsage(AttributeTargets.Struct)]
public class ComponentIdAttribute : Attribute
{
    public string Id { get; }
    
    public ComponentIdAttribute(string id)
    {
        Id = id;
    }
}