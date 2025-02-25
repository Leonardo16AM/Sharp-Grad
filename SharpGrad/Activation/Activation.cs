using SharpGrad.DifEngine;
using SharpGrad.Operators;
using System.Numerics;

namespace SharpGrad.Activation
{
    public abstract class Activation<TType>(string name, Value<TType> child) : UnariOperation<TType>(name, child)
        where TType : INumber<TType>
    {
    }
}
