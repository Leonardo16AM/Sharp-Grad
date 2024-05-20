using System;
using System.Numerics;

namespace SharpGrad.DifEngine
{
    public class PowValue<TType>(Value<TType> left, Value<TType> right)
        : Value<TType>(TType.CreateSaturating(Math.Pow(double.CreateSaturating(left.Data), double.CreateSaturating(right.Data))), "^", left, right)
        where TType : IBinaryFloatingPointIeee754<TType>
    {
        protected override void Backward()
        {
            LeftChildren.Grad += Grad * RightChildren.Data * TType.CreateSaturating(Math.Pow(double.CreateSaturating(LeftChildren.Data), double.CreateSaturating(RightChildren.Data) - 1.0));
            RightChildren.Grad += Grad * TType.CreateSaturating(Math.Pow(double.CreateSaturating(LeftChildren.Data), double.CreateSaturating(RightChildren.Data)) * Math.Log(double.CreateSaturating(LeftChildren.Data)));
        }
    }
}