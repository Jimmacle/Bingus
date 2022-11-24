using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;

namespace Bingus.Numerics.Matrices;

[StructLayout(LayoutKind.Sequential)]
public readonly record struct Matrix4<T>(
    T M11,
    T M12,
    T M13,
    T M14,
    T M21,
    T M22,
    T M23,
    T M24,
    T M31,
    T M32,
    T M33,
    T M34,
    T M41,
    T M42,
    T M43,
    T M44) where T : unmanaged, INumberBase<T>
{
    public readonly T M11 = M11;
    public readonly T M12 = M12;
    public readonly T M13 = M13;
    public readonly T M14 = M14;
    public readonly T M21 = M21;
    public readonly T M22 = M22;
    public readonly T M23 = M23;
    public readonly T M24 = M24;
    public readonly T M31 = M31;
    public readonly T M32 = M32;
    public readonly T M33 = M33;
    public readonly T M34 = M34;
    public readonly T M41 = M41;
    public readonly T M42 = M42;
    public readonly T M43 = M43;
    public readonly T M44 = M44;

    public static Matrix4<T> Identity = new Matrix4<T>(
        T.One,
        T.Zero,
        T.Zero,
        T.Zero,
        T.Zero,
        T.One,
        T.Zero,
        T.Zero,
        T.Zero,
        T.Zero,
        T.One,
        T.Zero,
        T.Zero,
        T.Zero,
        T.Zero,
        T.One);

    public Vector3<T> Translation => new(M41, M42, M43);

    public static unsafe Matrix4<T> operator +(Matrix4<T> left, Matrix4<T> right)
    {
        Unsafe.SkipInit(out Matrix4<T> result);

        switch (sizeof(T))
        {
            case 8:
                (Vector256.Load(&left.M21) + Vector256.Load(&right.M21)).Store(&result.M21);
                (Vector256.Load(&left.M41) + Vector256.Load(&right.M41)).Store(&result.M41);
                goto case 4;
            case 4:
                (Vector256.Load(&left.M31) + Vector256.Load(&right.M31)).Store(&result.M31);
                goto case 2;
            case 2:
                (Vector256.Load(&left.M11) + Vector256.Load(&right.M11)).Store(&result.M11);
                break;
            default:
                throw Exceptions.UnsupportedElementType();
        }

        return result;
    }
    
    public static unsafe Matrix4<T> operator *(Matrix4<T> left, Matrix4<T> right)
    {
        Unsafe.SkipInit(out Matrix4<T> result);

        switch (sizeof(T))
        {
            case 8:
            {
                for (var i = 0; i < 16; i += 4)
                {
                    var row = Vector256.Load(&left.M11 + i);
                    (row.ShuffleOne(0) * Vector256.Load(&right.M11) + 
                     row.ShuffleOne(1) * Vector256.Load(&right.M21) + 
                     row.ShuffleOne(2) * Vector256.Load(&right.M31) + 
                     row.ShuffleOne(3) * Vector256.Load(&right.M41))
                        .Store(&result.M11 + i);
                }
                break;
            }
            case 4:
            {
                for (var i = 0; i < 16; i += 4)
                {
                    var row = Vector128.Load(&left.M11 + i);
                    (row.ShuffleOne(0) * Vector128.Load(&right.M11) + 
                     row.ShuffleOne(1) * Vector128.Load(&right.M21) + 
                     row.ShuffleOne(2) * Vector128.Load(&right.M31) + 
                     row.ShuffleOne(3) * Vector128.Load(&right.M41))
                        .Store(&result.M11 + i);
                }
                break;
            }
            case 2:
            {
                for (var i = 0; i < 16; i += 4)
                {
                    var row = Vector64.Load(&left.M11 + i);
                    (row.ShuffleOne(0) * Vector64.Load(&right.M11) + 
                     row.ShuffleOne(1) * Vector64.Load(&right.M21) + 
                     row.ShuffleOne(2) * Vector64.Load(&right.M31) + 
                     row.ShuffleOne(3) * Vector64.Load(&right.M41))
                        .Store(&result.M11 + i);
                }
                break;
            }
            default:
                throw Exceptions.UnsupportedElementType();
        }

        return result;
    }

    public Matrix4<T> Transpose()
    {
        return new Matrix4<T>(
            M11, M21, M31, M41,
            M12, M22, M32, M42,
            M13, M23, M33, M43,
            M14, M24, M34, M44);
    }
}
