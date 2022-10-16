namespace Bingus.Core.EntityComponentSystem.Internal;

internal sealed class TypeNameComparer : IComparer<Type>
{
    public int Compare(Type? x, Type? y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (ReferenceEquals(null, y)) return 1;
        if (ReferenceEquals(null, x)) return -1;
        return string.Compare(x.AssemblyQualifiedName, y.AssemblyQualifiedName, StringComparison.Ordinal);
    }
}