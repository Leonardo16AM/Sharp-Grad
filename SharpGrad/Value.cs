using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;

namespace SharpGrad.DifEngine
{
    //TODO: Use class inheritance instead of switch-case
    public class Value<TType>(TType data, string name, Value<TType>? leftChild = null, Value<TType>? rightChild = null)
        where TType : IBinaryFloatingPointIeee754<TType>
    {
        private static int InstanceCount = 0;

        public static readonly Value<TType> e = new(TType.CreateSaturating(Math.E), "e");
        public static readonly Value<TType> Zero = new(TType.CreateSaturating(0.0), "zero");

        public delegate void BackwardPass();

        public TType Grad = TType.CreateSaturating(0.0);
        public TType Data = data;
        public readonly Value<TType>? LeftChildren = leftChild;
        public readonly Value<TType>? RightChildren = rightChild;
        public readonly string Name = name;


        #region BASIC ARITHMETIC OPERATIONS
        public static Value<TType> Add(Value<TType> left, Value<TType> right)
            => new AddValue<TType>(left, right);
        public static Value<TType> operator +(Value<TType> left, Value<TType> right)
            => Add(left, right);

        public static Value<TType> Sub(Value<TType> left, Value<TType> right)
            => new SubValue<TType>(left, right);
        public static Value<TType> operator -(Value<TType> left, Value<TType> right)
            => Sub(left, right);
        public static Value<TType> Sub(Value<TType> @this)
            => new SubValue<TType>(Zero, @this);
        public static Value<TType> operator -(Value<TType> @this)
            => Sub(@this);


        public static Value<TType> Mul(Value<TType> left, Value<TType> right)
            => new MulValue<TType>(left, right);
        public static Value<TType> operator *(Value<TType> left, Value<TType> right)
            => Mul(left, right);

        public static Value<TType> Div(Value<TType> left, Value<TType> right)
            => new DivValue<TType>(left, right);
        public static Value<TType> operator /(Value<TType> left, Value<TType> right)
            => Div(left, right);


        public static Value<TType> Pow(Value<TType> left, Value<TType> right)
            => new PowValue<TType>(left, right);
        public Value<TType> Pow(Value<TType> other)
            => new PowValue<TType>(this, other);
        #endregion

        #region ACTIVATION FUNCTIONS

        public Value<TType> ReLU()
            => new ReLUValue<TType>(this);

        public Value<TType> Tanh()
            => new TanhValue<TType>(this);

        public Value<TType> Sigmoid()
            => new SigmoidValue<TType>(this);
        
        public Value<TType> LeakyReLU(TType alpha)
            => new LeakyReLUValue<TType>(this,alpha);


        #endregion

        #region BACKPROPAGATION
        protected virtual void Backward()
        {
            if(LeftChildren != null)
                LeftChildren.Grad += Grad;
        }

        void DFS(List<Value<TType>> TopOSort, HashSet<Value<TType>> Visited)
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
            Grad = TType.CreateSaturating(1.0);
            List<Value<TType>> TopOSort = [];
            HashSet<Value<TType>> Visited = [];
            DFS(TopOSort, Visited);
            for(int i = TopOSort.Count - 1; i >= 0; i--)
            {
                TopOSort[i].Backward();
            }
        }
        #endregion

        public static implicit operator Value<TType>(TType d)
            => new(d, $"value_{++InstanceCount}");
        public static explicit operator TType(Value<TType> v)
            => v.Data;
    }
}