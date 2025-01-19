using SharpGrad.Operators;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;

namespace SharpGrad.DifEngine
{
    public abstract class ValueBase<TType>(TType data, string name, params ValueBase<TType>[] childs)
        where TType : IBinaryFloatingPointIeee754<TType>
    {
        public TType Grad = TType.Zero;
        public TType Data = data;
        public readonly ValueBase<TType>[] Childrens = childs;
        public readonly string Name = name;

        public abstract Expression GenerateExpression();

        protected virtual void Backward() { }
        public void Backpropagate()
        {
            Grad = TType.One;
            List<ValueBase<TType>> TopOSort = [];
            HashSet<ValueBase<TType>> Visited = [];
            DFS(TopOSort, Visited);
            for (int i = TopOSort.Count - 1; i >= 0; i--)
            {
                TopOSort[i].Backward();
            }
        }

        void DFS(List<ValueBase<TType>> TopOSort, HashSet<ValueBase<TType>> Visited)
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
        public static ValueBase<TType> Add(ValueBase<TType> left, ValueBase<TType> right)
            => new AddValue<TType>(left, right);
        public static ValueBase<TType> operator +(ValueBase<TType> left, ValueBase<TType> right)
            => Add(left, right);

        public static ValueBase<TType> Sub(ValueBase<TType> left, ValueBase<TType> right)
            => new SubValue<TType>(left, right);
        public static ValueBase<TType> operator -(ValueBase<TType> left, ValueBase<TType> right)
            => Sub(left, right);

        public static ValueBase<TType> Mul(ValueBase<TType> left, ValueBase<TType> right)
            => new MulValue<TType>(left, right);
        public static ValueBase<TType> operator *(ValueBase<TType> left, ValueBase<TType> right)
            => Mul(left, right);

        public static ValueBase<TType> Div(ValueBase<TType> left, ValueBase<TType> right)
            => new DivValue<TType>(left, right);
        public static ValueBase<TType> operator /(ValueBase<TType> left, ValueBase<TType> right)
            => Div(left, right);


        public static ValueBase<TType> Pow(ValueBase<TType> left, ValueBase<TType> right)
            => new PowValue<TType>(left, right);
        public ValueBase<TType> Pow(ValueBase<TType> other)
            => new PowValue<TType>(this, other);
        #endregion

    }
}