using SharpGrad.DifEngine;
using System;
using System.Numerics;

namespace SharpGrad.Operators
{
    // TODO : Find why the commented code is not working.
    public class PowValue<TType>(Value<TType> left, Value<TType> right)
        : BinaryOpValue<TType>(TType.CreateSaturating(Math.Pow(double.CreateSaturating(left.Data), double.CreateSaturating(right.Data))), "^", left, right)
        //: Value<TType>(TType.Pow(left.Data, right.Data), "^", left, right)
        where TType : IBinaryFloatingPointIeee754<TType>, IPowerFunctions<TType>
    {
        protected override void Backward()
        {
            LeftChildren.Grad += Grad * RightChildren.Data * TType.CreateSaturating(Math.Pow(double.CreateSaturating(LeftChildren.Data), double.CreateSaturating(RightChildren.Data) - 1.0));
            RightChildren.Grad += Grad * TType.CreateSaturating(Math.Pow(double.CreateSaturating(LeftChildren.Data), double.CreateSaturating(RightChildren.Data)) * Math.Log(double.CreateSaturating(LeftChildren.Data)));
            //LeftChildren!.Grad += Grad * RightChildren!.Data * TType.Pow(LeftChildren.Data, RightChildren.Data - TType.NegativeOne);
            //RightChildren!.Grad += Grad * TType.Pow(LeftChildren.Data, RightChildren.Data) * TType.Log(LeftChildren.Data);
        }
    }
}