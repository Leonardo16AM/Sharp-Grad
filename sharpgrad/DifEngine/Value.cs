namespace SharpGrad.DifEngine
{
    public class Value
    {
        public const string ReLUName = "ReLU";
        public const string AddName = "+";
        public const string SubName = "-";
        public const string MulName = "*";
        public const string DivName = "/";
        public const string PowName = "^";

        public const string TanhName = "tanh";


        public delegate void BackwardPass();

        public double Grad;
        public double Data;
        public readonly Value? LeftChildren;
        public readonly Value? RightChildren;
        public readonly List<Value> TopOSort;
        public readonly string Name;
        private readonly BackwardPass Backward;
        private readonly HashSet<Value> Visited;

        public Value(double data, string name, Value? leftChild = null, Value? rightChild = null)
        {
            Data = data;
            Grad = 0.0;
            LeftChildren = leftChild;
            RightChildren = rightChild;
            Name = name;
            Backward = name switch
            {
                AddName => BackwardAdd,
                SubName => BackwardSubs,
                MulName => BackwardMul,
                PowName => BackwardPow,
                ReLUName => BackwardReLU,
                DivName => BackwardDiv,
                //TanhName => BackwardEmpt,
                _ => BackwardEmpt,
            };
            TopOSort = new();
            Visited = new();
        }


        #region BASIC ARITHMETIC OPERATIONS
        public static Value Add(Value left, Value right)
            => new(left.Data + right.Data, AddName, left, right);
        public static Value operator +(Value left, Value right)
            => Add(left, right);

        public static Value Sub(Value left, Value right)
            => new(left.Data - right.Data, SubName, left, right);
        public static Value operator -(Value left, Value right)
            => Sub(left, right);


        public static Value Mul(Value left, Value right)
            => new(left.Data * right.Data, MulName, left, right);
        public static Value operator *(Value left, Value right)
            => Mul(left, right);

        public static Value Div(Value left, Value right)
            => new(left.Data / right.Data, DivName, left, right);
        public static Value operator /(Value left, Value right)
            => Div(left, right);


        public static Value Pow(Value left, Value right)
            => new(Math.Pow(left.Data, right.Data), PowName, left, right);
        public static Value operator ^(Value left, Value right)
            => Pow(left, right);
        #endregion

        #region ACTIVATION FUNCTIONS
        public Value ReLU()
        {
            Value c = new((Data <= 0) ? 0 : Data, ReLUName, this);
            return c;
        }

        public Value TanH()
        {
            Value e = new(Math.E, "e");
            Value la = (new Value(0.0, "zero")) - this;
            Value c = (((e ^ this) - (e ^ la)) / ((e ^ this) + (e ^ la)));
            Value ret = new(c.Data, "tanh", c);
            return c;
        }
        #endregion

        #region BACKWARD PASS FUNCTIONS
        protected void BackwardEmpt()
        {
            if(LeftChildren is not null)
                LeftChildren.Grad += Grad;
        }

        protected void BackwardAdd()
        {
            LeftChildren.Grad += Grad;
            RightChildren.Grad += Grad;
        }

        protected void BackwardSubs()
        {
            LeftChildren.Grad += Grad;
            RightChildren.Grad -= Grad;
        }

        protected void BackwardMul()
        {
            LeftChildren.Grad += Grad * RightChildren.Data;
            RightChildren.Grad += Grad * LeftChildren.Data;
        }

        protected void BackwardDiv()
        {
            LeftChildren.Grad += Grad / RightChildren.Data;
            RightChildren.Grad += Grad * LeftChildren.Data / (RightChildren.Data * RightChildren.Data);
        }

        protected void BackwardPow()
        {
            LeftChildren.Grad += Grad * RightChildren.Data * Math.Pow(LeftChildren.Data, RightChildren.Data - 1.0);
            RightChildren.Grad += Grad * Math.Pow(LeftChildren.Data, RightChildren.Data) * Math.Log(LeftChildren.Data);
        }

        protected void BackwardReLU()
        {
            if (Grad > 0)
                LeftChildren.Grad += Grad;
        }
        #endregion

        #region BACKPROPAGATION
        void DFS(Value value)
        {
            Visited.Add(value);
            if (value.LeftChildren is not null && !Visited.Contains(value.LeftChildren))
                DFS(value.LeftChildren);
            if (value.RightChildren is not null && !Visited.Contains(value.RightChildren))
                DFS(value.RightChildren);
            TopOSort.Add(value);
        }

        public void Backpropagate()
        {
            Visited.Clear();
            DFS(this);
            TopOSort.Reverse();
            foreach (Value value in TopOSort)
            {
                value.Backward();
                // Console.WriteLine(u.name+" -> "+u.grad);
            }
        }

        public void ResetGrad()
        {
            Visited.Clear();
            LeftChildren?.ResetGrad();
            RightChildren?.ResetGrad();
        }
        #endregion
    }
}