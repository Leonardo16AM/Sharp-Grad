using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SharpGrad.DifEngine
{
    public class Value
    {
        public static readonly Value e = new Value(Math.E, "e");

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
        public readonly string Name;
        private readonly BackwardPass Backward;

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
        }


        #region BASIC ARITHMETIC OPERATIONS
        public static Value Add(Value left, Value right)
            => new Value(left.Data + right.Data, AddName, left, right);
        public static Value operator +(Value left, Value right)
            => Add(left, right);

        public static Value Sub(Value left, Value right)
            => new Value(left.Data - right.Data, SubName, left, right);
        public static Value operator -(Value left, Value right)
            => Sub(left, right);
        public static Value operator -(Value @this)
            => new Value(-@this.Data, SubName, @this);


        public static Value Mul(Value left, Value right)
            => new Value(left.Data * right.Data, MulName, left, right);
        public static Value operator *(Value left, Value right)
            => Mul(left, right);

        public static Value Div(Value left, Value right)
            => new Value(left.Data / right.Data, DivName, left, right);
        public static Value operator /(Value left, Value right)
            => Div(left, right);


        public static Value Pow(Value left, Value right)
            => new Value(Math.Pow(left.Data, right.Data), PowName, left, right);
        public static Value operator ^(Value left, Value right)
            => Pow(left, right);
        #endregion

        #region ACTIVATION FUNCTIONS
        public Value ReLU()
            => new Value((Data <= 0) ? 0 : Data, ReLUName, this);

        public Value TanH()
        {
            Value eThis = Value.e ^ this;
            Value eLa = Value.e ^ -this;
            Value c = (eThis - eLa) / (eThis + eLa);
            Value ret = new Value(c.Data, "tanh", c);
            return c;
        }
        #endregion

        #region BACKWARD PASS FUNCTIONS
        protected void BackwardEmpt()
        {
            if(LeftChildren != null)
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
        void DFS(Value value, List<Value> TopOSort, HashSet<Value> Visited)
        {
            Visited.Add(value);
            if (value.LeftChildren != null && !Visited.Contains(value.LeftChildren))
                DFS(value.LeftChildren, TopOSort, Visited);
            if (value.RightChildren != null && !Visited.Contains(value.RightChildren))
                DFS(value.RightChildren, TopOSort, Visited);
            TopOSort.Add(value);
        }

        public void Backpropagate()
        {
            List<Value> TopOSort = new List<Value>();
            HashSet<Value> Visited = new HashSet<Value>();
            DFS(this, TopOSort, Visited);
            for(int i = TopOSort.Count - 1; i >= 0; i--)
            {
                TopOSort[i].Backward();
            }
        }

        public void ResetGrad()
        {
            LeftChildren?.ResetGrad();
            RightChildren?.ResetGrad();
        }
        #endregion
    }
}