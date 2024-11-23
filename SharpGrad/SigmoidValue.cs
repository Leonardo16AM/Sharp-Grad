using System.Numerics;

namespace SharpGrad.DifEngine
{
    public class SigmoidValue<TType> : Value<TType>
        where TType : IBinaryFloatingPointIeee754<TType>
    {
        public SigmoidValue(Value<TType> value)
            : base(TType.One / (TType.One + TType.Exp(-value.Data)), "sigmoid", value)
        {
        }

        protected override void Backward()
        {
            var sigmoid = Data;
            LeftChildren.Grad += Grad * sigmoid * (TType.One - sigmoid);
        }
    }
}
