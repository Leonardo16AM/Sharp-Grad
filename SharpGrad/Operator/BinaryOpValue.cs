using SharpGrad.DifEngine;
using SharpGrad.ExprLambda;
using SharpGrad.Operator;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;

namespace SharpGrad.Operators
{
    public abstract class BinaryOpValue<TType> :
        NariOpValue<TType>
        where TType : INumber<TType>
    {
        public Value<TType> LeftOperand => Operands[0];
        protected abstract void ComputeLeftGradient(
            Dictionary<Value<TType>, Expression> variableExpressions,
            Dictionary<Value<TType>, Expression> gradientExpressions,
            List<Expression> expressionList);

        public Value<TType> RightOperand => Operands[1];
        protected abstract void ComputeRightGradient(
            Dictionary<Value<TType>, Expression> variableExpressions,
            Dictionary<Value<TType>, Expression> gradientExpressions,
            List<Expression> expressionList);

        public sealed override ComputeGradientDelegate[] ChildrensCompute { get; }

        public static IReadOnlyList<Dimension> GetShape(Value<TType> left, Value<TType> right)
            => left.Shape
                .Union(right.Shape)
                .Distinct()
                .ToList();

        public BinaryOpValue(string name, Value<TType> left, Value<TType> right)
            : base(GetShape(left, right), name, left, right)
        {
            ChildrensCompute = [ComputeLeftGradient, ComputeRightGradient];
        }

        protected abstract Expr GetForwardComputation(Expr left, Expr right);
        internal sealed override Expression GetForwardComputation(Dictionary<Value<TType>, Expression> variableExpressions, Expression index)
        {
            Expression left = LeftOperand.GetAsOperand(variableExpressions, index);
            Expression right = RightOperand.GetAsOperand(variableExpressions, index);
            return GetForwardComputation(left, right);
        }

        public override string ToString()
            => $"({LeftOperand} {Name} {RightOperand})";
    }
}