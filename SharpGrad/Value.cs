using SharpGrad.Operator;
using SharpGrad.Operators;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;

namespace SharpGrad.DifEngine
{
    public abstract class Value<TType> where TType : INumber<TType>
    {
        public static readonly Expression ExpressionZero = Expression.Constant(TType.Zero);
        public static readonly Expression ExpressionOne = Expression.Constant(TType.One);

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

        public readonly Value<TType>[] Childrens;
        public readonly string Name;
        protected readonly Expression DataExpression;
        protected TType data;
        public virtual TType Data => data;

        public TType Grad = TType.Zero;

        protected void DFS(List<Value<TType>> TopOSort, HashSet<Value<TType>> Visited)
        {
            if (Visited.Add(this))
            {
                foreach (var child in Childrens)
                {
                    child.DFS(TopOSort, Visited);
                }
                TopOSort.Add(this);
            }
        }

        public abstract bool GetAsOperand(Dictionary<Value<TType>, Expression> variableExpressions, List<Expression> forwardExpressionList, out Expression? operand);
        public void BuildForward(Dictionary<Value<TType>, Expression> variableExpressions, List<Expression> forwardExpressionList)
            => _ = GetAsOperand(variableExpressions, forwardExpressionList, out var _);
        public Expression GetAsOperand(Dictionary<Value<TType>, Expression> variableExpressions)
        {
            List<Expression> forwardExpressionList = [];
            if (GetAsOperand(variableExpressions, forwardExpressionList, out var operand)
                && forwardExpressionList.Count == 0)
            {
                return operand!;
            }
            else
            {
                throw new InvalidOperationException($"Expression list should be empty. Found {forwardExpressionList.Count} expressions.");
            }
        }
        protected void AssignGradientExpession(Dictionary<Value<TType>, Expression> gradientExpressions, List<Expression> forwardExpressionList, Value<TType> LeftOperand, Expression gradientExpression)
        {
            if (!gradientExpressions.TryGetValue(LeftOperand, out Expression? leftGrad))
            {
                leftGrad = Expression.Variable(typeof(TType), $"{LeftOperand.Name}_grad");
                gradientExpressions[LeftOperand] = leftGrad;
                forwardExpressionList.Add(Expression.Assign(leftGrad, gradientExpression));
            }
            else
            {
                forwardExpressionList.Add(Expression.AddAssign(leftGrad, gradientExpression));
            }
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