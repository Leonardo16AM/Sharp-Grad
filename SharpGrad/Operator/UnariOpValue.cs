using SharpGrad.DifEngine;
using System.Numerics;

namespace SharpGrad.Operator
{
    public abstract class UnariOpValue<TType>(string name, Value<TType> child) :
        NariOpValue<TType>(name, child)
        where TType : INumber<TType>
    {
        public Value<TType> Operand => Childrens[0];
    }


}