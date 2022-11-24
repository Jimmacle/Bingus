using System.Numerics;
using JetBrains.Annotations;

namespace Bingus.Numerics;

public readonly record struct Vector3<T>(T X, T Y, T Z) : IVector3<Vector3<T>, T> where T : INumberBase<T>
{
    [UsedImplicitly]
    private readonly T _paddingForSimd = T.Zero;
    
    public static Vector3<T> operator +(Vector3<T> left, Vector3<T> right)
    {
        return new Vector3<T>(
            left.X + right.X, 
            left.Y + right.Y, 
            left.Z + right.Z);
    }

    public static Vector3<T> AdditiveIdentity => new(T.Zero, T.Zero, T.Zero);

    public static Vector3<T> operator -(Vector3<T> left, Vector3<T> right)
    {
        return new Vector3<T>(
            left.X - right.X, 
            left.Y - right.Y, 
            left.Z - right.Z);
    }

    public static Vector3<T> operator *(Vector3<T> left, T right)
    {
        return new Vector3<T>(
            left.X * right, 
            left.Y * right, 
            left.Z * right);
    }

    public static T MultiplicativeIdentity => T.One;

    public Vector3<T> Cross(Vector3<T> other)
    {
        return new Vector3<T>
        {
            X = Y * other.Z - Z * other.Y,
            Y = Z * other.X - X * other.Z,
            Z = X * other.Y - Y * other.X
        };
    }

    public static Vector3<T> operator -(Vector3<T> value)
    {
        return new Vector3<T>(
            -value.X, 
            -value.Y, 
            -value.Z);
    }

    public static Vector3<T> operator /(Vector3<T> left, T right)
    {
        return new Vector3<T>(
            left.X / right, 
            left.Y / right, 
            left.Z / right);
    }

    public static Vector3<T> One => new(T.One, T.One, T.One);

    public static Vector3<T> Zero => new(T.Zero, T.Zero, T.Zero);

    public T Dot(Vector3<T> other)
    {
        return X * other.X + 
               Y * other.Y + 
               Z * other.Z;
    }

    public double Magnitude()
    {
        return double.Sqrt(double.CreateChecked(Dot(this)));
    }

    public Vector3<TOther> As<TOther>() where TOther : INumberBase<TOther>
    {
        return new Vector3<TOther>(
            TOther.CreateChecked(X),
            TOther.CreateChecked(Y),
            TOther.CreateChecked(Z));
    }
}