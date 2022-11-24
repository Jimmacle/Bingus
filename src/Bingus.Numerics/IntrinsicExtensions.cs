using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;

namespace Bingus.Numerics;

public static class IntrinsicExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector256<T> ShuffleOne<T>(this Vector256<T> vector, int index) where T : struct
    {
        return vector switch
        {
            Vector256<double> v => Vector256.Shuffle(v, Vector256.Create((long)index)).As<double, T>(),
            Vector256<long> v => Vector256.Shuffle(v, Vector256.Create((long)index)).As<long, T>(),
            Vector256<ulong> v => Vector256.Shuffle(v, Vector256.Create((ulong)index)).As<ulong, T>(),
            _ => throw Exceptions.UnsupportedElementType()
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector128<T> ShuffleOne<T>(this Vector128<T> vector, int index) where T : struct
    {
        return vector switch
        {
            Vector128<float> v => Vector128.Shuffle(v, Vector128.Create(index)).As<float, T>(),
            Vector128<int> v => Vector128.Shuffle(v, Vector128.Create(index)).As<int, T>(),
            Vector128<uint> v => Vector128.Shuffle(v, Vector128.Create((uint)index)).As<uint, T>(),
            _ => throw Exceptions.UnsupportedElementType()
        };
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector64<T> ShuffleOne<T>(this Vector64<T> vector, int index) where T : struct
    {
        return vector switch
        {
            Vector64<short> v => Vector64.Shuffle(v, Vector64.Create((short)index)).As<short, T>(),
            Vector64<ushort> v => Vector64.Shuffle(v, Vector64.Create((ushort)index)).As<ushort, T>(),
            _ => throw Exceptions.UnsupportedElementType()
        };
    }
}
