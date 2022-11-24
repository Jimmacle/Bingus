using System.Buffers;

namespace Bingus.Core;

public static class ArrayPoolExtensions
{
    public static RentedArray<T> RentDisposable<T>(this ArrayPool<T> pool, int length)
    {
#pragma warning disable Bingus0001
        return new RentedArray<T>(pool, pool.Rent(length), length);
    }
}

/// <summary>
/// Encapsulates an array rented from an <see cref="ArrayPool{T}"/>.
/// </summary>
/// <typeparam name="T">The type of elements in the array.</typeparam>
public readonly struct RentedArray<T> : IDisposable
{
    private readonly ArrayPool<T> _source;
    private readonly T[] _array;

    public int Length { get; }

    internal RentedArray(ArrayPool<T> source, T[] array, int length)
    {
        _source = source;
        _array = array;
        Length = length;
    }

    public T this[int index]
    {
        get => AsSpan()[index];
        set => AsSpan()[index] = value;
    }

    public Span<T> AsSpan() => new(_array, 0, Length);

    public void Dispose()
    {
        _source.Return(_array);
    }

    public static implicit operator Span<T>(RentedArray<T> arr) => arr.AsSpan();

    public static implicit operator ReadOnlySpan<T>(RentedArray<T> arr) => arr.AsSpan();
}