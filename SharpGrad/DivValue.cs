namespace SharpGrad.DifEngine
{
    public class DivValue : Value
    {
        public DivValue(Value left, Value right)
            : base(left.Data / right.Data, "/", left, right)
        {
        }

        // TODO: Is this a good way to backpropagate division?
        protected override void Backward()
        {
            LeftChildren.Grad += Grad / RightChildren.Data;
            RightChildren.Grad += Grad * LeftChildren.Data / (RightChildren.Data * RightChildren.Data);
        }
    }
}