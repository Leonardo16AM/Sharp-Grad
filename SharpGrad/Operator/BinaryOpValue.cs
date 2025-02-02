using SharpGrad.DifEngine;
using SharpGrad.Operator;
using System.Collections.Generic;
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

        public BinaryOpValue(int shape, string name, Value<TType> left, Value<TType> right) : base(shape, name, left, right)
        {
            ChildrensCompute = [ComputeLeftGradient, ComputeRightGradient];
        }

        protected abstract Expression GetForwardComputation(Expression left, Expression right);
        internal override Expression GetForwardComputation(Dictionary<Value<TType>, Expression> variableExpressions, Expression index)
        {
            Expression left = LeftOperand.GetAsOperand(variableExpressions, index);
            Expression right = RightOperand.GetAsOperand(variableExpressions, index);
            return GetForwardComputation(left, right);
        }

        public override string ToString()
            => $"({LeftOperand} {Name} {RightOperand})";
    }
}