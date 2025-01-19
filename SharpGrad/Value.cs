using SharpGrad.Operators;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;

namespace SharpGrad.DifEngine
{
    public abstract class Value<TType> where TType : INumber<TType>
    {
        public static readonly Variable<TType> e = new(TType.CreateSaturating(Math.E), "e");
        public static readonly Variable<TType> Zero = new(TType.Zero, "0");

        public static class Expressions
        {
            public static readonly Expression Zero = Expression.Constant(TType.Zero);
            public static readonly Expression One = Expression.Constant(TType.One);
        }

        public readonly Value<TType>[] Childrens;
        public readonly string Name;
        protected readonly Expression DataExpression;
        public TType Data;
        public TType Grad = TType.Zero;

        public abstract Expression GenerateForwardExpression();

        protected virtual void Backward() { }

        public void Backpropagate()
        {
            Grad = TType.One;
            List<Value<TType>> TopOSort = [];
            HashSet<Value<TType>> Visited = [];
            DFS(TopOSort, Visited);
            for (int i = TopOSort.Count - 1; i >= 0; i--)
            {
                TopOSort[i].Backward();
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

        #region BASIC ARITHMETIC OPERATIONS
        public static AddValue<TType> Add(Value<TType> left, Value<TType> right) => new(left, right);
        public static AddValue<TType> operator +(Value<TType> left, Value<TType> right) => Add(left, right);

        public static SubValue<TType> Sub(Value<TType> left, Value<TType> right) => new(left, right);
        public static SubValue<TType> operator -(Value<TType> left, Value<TType> right) => Sub(left, right);

        public static MulValue<TType> Mul(Value<TType> left, Value<TType> right) => new(left, right);
        public static MulValue<TType> operator *(Value<TType> left, Value<TType> right) => Mul(left, right);

        public static DivValue<TType> Div(Value<TType> left, Value<TType> right) => new(left, right);
        public static DivValue<TType> operator /(Value<TType> left, Value<TType> right) => Div(left, right);
        #endregion

        private static int InstanceCount = 0;

        public Value(TType data, string name, params Value<TType>[] childs)
        {
            Childrens = childs;
            Name = name;
            Data = data;
            DataExpression = Expression.Field(Expression.Constant(this), nameof(Data));
        }

        public static implicit operator Value<TType>(TType d)
            => new Constant<TType>(d, $"v{InstanceCount++}");

        public static explicit operator TType(Value<TType> v)
            => v.Data;

    }
}