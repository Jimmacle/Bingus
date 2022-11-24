using System.Diagnostics.CodeAnalysis;

namespace Bingus.Numerics;

internal static class Exceptions
{
    public static Exception UnsupportedElementType()
    {
        return new NotSupportedException("Unsupported element type.");
    }
}
