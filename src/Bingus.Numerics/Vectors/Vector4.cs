using System.Numerics;

namespace Bingus.Numerics;

public readonly record struct Vector4<T>(T X, T Y, T Z, T W) : IVectorBase<Vector4<T>, T> where T : INumberBase<T>
{
    public Vector4(Vector3<T> vector, T w) : this(vector.X, vector.Y, vector.Z, w)
    {
        
    }

    public static Vector4<T> One => new(T.One, T.One, T.One, T.One);

    public static Vector4<T> Zero => new(T.Zero, T.Zero, T.Zero, T.Zero);
    
    public static Vector4<T> operator +(Vector4<T> left, Vector4<T> right)
    {
        return new Vector4<T>(
            left.X + right.X,
            left.Y + right.Y,
            left.Z + right.Z,
            left.W + right.W);
    }

    public static Vector4<T> AdditiveIdentity => new(
        T.AdditiveIdentity, 
        T.AdditiveIdentity, 
        T.AdditiveIdentity, 
        T.AdditiveIdentity);

    public static Vector4<T> operator -(Vector4<T> left, Vector4<T> right)
    {
        return new Vector4<T>(
            left.X - right.X,
            left.Y - right.Y,
            left.Z - right.Z,
            left.W - right.W);
    }

    public static Vector4<T> operator *(Vector4<T> left, T right)
    {
        return new Vector4<T>(
            left.X * right,
            left.Y * right,
            left.Z * right,
            left.W * right);
    }

    public static T MultiplicativeIdentity => T.MultiplicativeIdentity;

    public static Vector4<T> operator /(Vector4<T> left, T right)
    {
        return new Vector4<T>(
            left.X / right,
            left.Y / right,
            left.Z / right,
            left.W / right);
    }

    public static Vector4<T> operator -(Vector4<T> value)
    {
        return new Vector4<T>(
            -value.X, 
            -value.Y, 
            -value.Z, 
            -value.W);
    }

    public T Dot(Vector4<T> other)
    {
        return X * other.X + 
               Y * other.Y + 
               Z * other.Z + 
               W * other.W;
    }

    public double Magnitude()
    {
        return double.Sqrt(double.CreateChecked(Dot(this)));
    }
    
    public Vector4<TOther> As<TOther>() where TOther : INumberBase<TOther>
    {
        return new Vector4<TOther>(
            TOther.CreateChecked(X),
            TOther.CreateChecked(Y),
            TOther.CreateChecked(Z),
            TOther.CreateChecked(W));
    }
}
