using SharpGrad.DifEngine;
using SharpGrad.Operator;
using System.Collections.Generic;
using System.Numerics;

namespace SharpGrad.Activation
{
    public abstract class Activation<TType>(string name, Value<TType> child) : UnariOpValue<TType>(name, child)
        where TType : INumber<TType>
    {
    }
}
