using SharpGrad.Operators;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;

namespace SharpGrad.DifEngine
{
    public abstract class Value<TType> where TType : INumber<TType>
    {
        private static int InstanceCount = 0;
        public static readonly Variable<TType> e = new(TType.CreateSaturating(Math.E), "e");
        public static readonly Variable<TType> Zero = new(TType.Zero, "0");

        public Value(string name, params Value<TType>[] childs)
        {
            Childrens = childs;
            Name = name;
            data = TType.Zero;
            DataExpression = Expression.Field(Expression.Constant(this), nameof(data));
        }

        public static class Expressions
        {
            public static readonly Expression Zero = Expression.Constant(TType.Zero);
            public static readonly Expression One = Expression.Constant(TType.One);
        }

        public readonly Value<TType>[] Childrens;
        public readonly string Name;
        protected readonly Expression DataExpression;
        protected TType data;
        public virtual TType Data => data;

        public TType Grad = TType.Zero;

        public abstract Expression GenerateForwardExpression(Dictionary<Value<TType>, Expression> variableExpressions);

        private Func<TType>? forwardLambda;
        public Func<TType> ForwardLambda
        {
            get
            {
                if (forwardLambda is null)
                {
                    // Get forward expression
                    Expression forwardExpression = GenerateForwardExpression([]);
                    // Compile Expression ussing Lambda function
                    forwardLambda = Expression.Lambda<Func<TType>>(forwardExpression).Compile();
                }
                return forwardLambda;
            }
        }

        protected virtual void Backward(TType accCount) { }

        public void Backpropagate(int accCount = 1)
        {
            Grad = TType.One;
            List<Value<TType>> TopOSort = [];
            HashSet<Value<TType>> Visited = [];
            DFS(TopOSort, Visited);
            TType accC = TType.CreateSaturating(accCount);
            for (int i = TopOSort.Count - 1; i >= 0; i--)
            {
                TopOSort[i].Backward(accC);
            }
        }

        void DFS(List<Value<TType>> TopOSort, HashSet<Value<TType>> Visited)
        {
            Visited.Add(this);
            foreach (var child in Childrens)
            {
                if (!Visited.Contains(child))
                    child.DFS(TopOSort, Visited);
            }
            TopOSort.Add(this);
        }

        public override string ToString()
        {
            return Name;
        }

        #region BASIC ARITHMETIC OPERATIONS
        public static AddValue<TType> Add(Value<TType> left, Value<TType> right) => new(left, right);
        public static AddValue<TType> operator +(Value<TType> left, Value<TType> right) => Add(left, right);

        public static SubValue<TType> Sub(Value<TType> left, Value<TType> right) => new(left, right);
        public static SubValue<TType> operator -(Value<TType> left, Value<TType> right) => Sub(left, right);

        public static MulValue<TType> Mul(Value<TType> left, Value<TType> right) => new(left, right);
        public static MulValue<TType> operator *(Value<TType> left, Value<TType> right) => Mul(left, right);

        public static DivValue<TType> Div(Value<TType> left, Value<TType> right) => new(left, right);

        public void ResetGradient()
        {
            Grad = TType.Zero;
            if (Childrens.Length > 0)
            {
                foreach (var child in Childrens)
                {
                    child.ResetGradient();
                }
            }
        }

        public static DivValue<TType> operator /(Value<TType> left, Value<TType> right) => Div(left, right);
        #endregion

        public static implicit operator Value<TType>(TType d)
            => new Constant<TType>(d, $"v{InstanceCount++}");

        public static explicit operator TType(Value<TType> v)
            => v.data;

    }
}