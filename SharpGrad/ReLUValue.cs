namespace SharpGrad.DifEngine
{
    public class ReLUValue : Value
    {
        public ReLUValue(Value value)
            : base((value.Data <= 0) ? 0 : value.Data, "relu", value)
        {
        }

        protected override void Backward()
        {
            if (Grad > 0)
                LeftChildren.Grad += Grad;
        }
    }
}