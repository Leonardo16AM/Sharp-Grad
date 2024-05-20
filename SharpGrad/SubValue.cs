using System.Numerics;

namespace SharpGrad.DifEngine
{
    public class SubValue<TType>(Value<TType> left, Value<TType> right) : Value<TType>(left.Data - right.Data, "-", left, right)
        where TType : IBinaryFloatingPointIeee754<TType>
    {
        protected override void Backward()
        {
            LeftChildren.Grad += Grad;
            RightChildren.Grad -= Grad;
        }
    }
}