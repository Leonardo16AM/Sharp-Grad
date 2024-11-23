using System.Numerics;

namespace SharpGrad.DifEngine
{
    public class TanhValue<TType> : Value<TType>
        where TType : IBinaryFloatingPointIeee754<TType>
    {
        public TanhValue(Value<TType> value)
            : base(TType.Tanh(value.Data), "tanh", value)
        {
        }

        protected override void Backward()
        {
            var tanh = Data;
            LeftChildren.Grad += Grad * (TType.One - tanh * tanh);
        }
    }
}
