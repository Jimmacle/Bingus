using System.Numerics;

namespace Bingus.Numerics;

public interface IVectorBase<TSelf, TElement> : 
    IAdditionOperators<TSelf, TSelf, TSelf>,
    IAdditiveIdentity<TSelf, TSelf>,
    ISubtractionOperators<TSelf, TSelf, TSelf>,
    IMultiplyOperators<TSelf, TElement, TSelf>,
    IMultiplicativeIdentity<TSelf, TElement>,
    IDivisionOperators<TSelf, TElement, TSelf>,
    IUnaryNegationOperators<TSelf, TSelf>
    where TSelf : IVectorBase<TSelf, TElement>
    where TElement : INumberBase<TElement>
{
    static abstract TSelf One { get; }
    static abstract TSelf Zero { get; }
    
    TElement Dot(TSelf other);

    double Magnitude();
}