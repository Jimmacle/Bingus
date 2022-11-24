using System.Reflection;

namespace Bingus.Core.EntityComponentSystem;

public interface IComponent<T> where T : struct
{
    private static string? _id;
    public static string Id => _id ??= typeof(T).GetCustomAttribute<ComponentAttribute>()?.Id 
                                       ?? throw new InvalidOperationException("Component does not define an ID.");
}

[AttributeUsage(AttributeTargets.Struct)]
public class ComponentAttribute : Attribute
{
    public string Id { get; }
    
    public ComponentAttribute(string id)
    {
        Id = id;
    }
}