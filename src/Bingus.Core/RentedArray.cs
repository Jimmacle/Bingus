using System.Buffers;

namespace Bingus.Core;

public static class ArrayPoolExtensions
{
    public static RentedArray<T> RentDisposable<T>(this ArrayPool<T> pool, int length)
    {
        return new RentedArray<T>(pool, pool.Rent(length), length);
    }
}

public readonly struct RentedArray<T> : IDisposable
{
    private readonly ArrayPool<T> _source;
    private readonly T[] _array;
    private readonly int _length;

    internal RentedArray(ArrayPool<T> source, T[] array, int length)
    {
        _source = source;
        _array = array;
        _length = length;
    }

    public T this[int index]
    {
        get => Span[index];
        set => Span[index] = value;
    }

    public Span<T> Span => new(_array, 0, _length);

    public T[] Array => _array;

    public void Dispose()
    {
        _source.Return(_array);
    }

    public static implicit operator Span<T>(RentedArray<T> arr) => arr.Span;

    public static implicit operator ReadOnlySpan<T>(RentedArray<T> arr) => new(arr._array, 0, arr._length);
}