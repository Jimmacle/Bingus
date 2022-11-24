using System.Numerics;

namespace Bingus.Numerics;

public interface IVector3<TSelf, TElement> : IVectorBase<TSelf, TElement> where TSelf : IVectorBase<TSelf, TElement> where TElement : INumberBase<TElement>
{
    public TSelf Cross(TSelf other);
}