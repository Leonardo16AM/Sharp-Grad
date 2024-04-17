using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SharpGrad.DifEngine
{
    //TODO: Use class inheritance instead of switch-case
    public class Value
    {
        private static int InstanceCount = 0;

        public static readonly Value e = new Value(Math.E, "e");
        public static readonly Value Zero = new Value(0.0, "zero");

        public delegate void BackwardPass();

        public double Grad;
        public double Data;
        public readonly Value? LeftChildren;
        public readonly Value? RightChildren;
        public readonly string Name;

        public Value(double data, string name, Value? leftChild = null, Value? rightChild = null)
        {
            Data = data;
            Grad = 0.0;
            LeftChildren = leftChild;
            RightChildren = rightChild;
            Name = name;
        }


        #region BASIC ARITHMETIC OPERATIONS
        public static Value Add(Value left, Value right)
            => new AddValue(left, right);
        public static Value operator +(Value left, Value right)
            => Add(left, right);

        public static Value Sub(Value left, Value right)
            => new SubValue(left, right);
        public static Value operator -(Value left, Value right)
            => Sub(left, right);
        public static Value Sub(Value @this)
            => new SubValue(Zero, @this);
        public static Value operator -(Value @this)
            => Sub(@this);


        public static Value Mul(Value left, Value right)
            => new MulValue(left, right);
        public static Value operator *(Value left, Value right)
            => Mul(left, right);

        public static Value Div(Value left, Value right)
            => new DivValue(left, right);
        public static Value operator /(Value left, Value right)
            => Div(left, right);


        public static Value Pow(Value left, Value right)
            => new PowValue(left, right);
        public static Value operator ^(Value left, Value right)
            => Pow(left, right);
        #endregion

        #region ACTIVATION FUNCTIONS
        public Value ReLU()
            => new ReLUValue(this);

        public Value TanH()
        {
            Value eThis = Value.e ^ this;
            Value eLa = Value.e ^ -this;
            Value c = (eThis - eLa) / (eThis + eLa);
            Value ret = new Value(c.Data, "tanh", c);
            return c;
        }
        #endregion

        protected virtual void Backward()
        {
            if(LeftChildren != null)
                LeftChildren.Grad += Grad;
        }

        #region BACKPROPAGATION
        void DFS(List<Value> TopOSort, HashSet<Value> Visited)
        {
            Visited.Add(this);
            if (LeftChildren != null && !Visited.Contains(LeftChildren))
                LeftChildren.DFS(TopOSort, Visited);
            if (RightChildren != null && !Visited.Contains(RightChildren))
                RightChildren.DFS(TopOSort, Visited);
            TopOSort.Add(this);
        }

        public void Backpropagate()
        {
            Grad = 1.0;
            List<Value> TopOSort = new List<Value>();
            HashSet<Value> Visited = new HashSet<Value>();
            DFS(TopOSort, Visited);
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

        public static implicit operator Value(double d)
            => new Value(d, $"value_{++InstanceCount}");
        public static explicit operator double(Value v)
            => v.Data;
    }
}