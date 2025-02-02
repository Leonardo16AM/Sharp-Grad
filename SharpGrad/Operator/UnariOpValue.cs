using SharpGrad.DifEngine;
using SharpGrad.ExprLambda;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;

namespace SharpGrad.Operator
{
    public abstract class UnariOpValue<TType> :
        NariOpValue<TType>
        where TType : INumber<TType>
    {
        public Value<TType> Operand => Operands[0];

        public sealed override ComputeGradientDelegate[] ChildrensCompute { get; }
        protected abstract void ComputeGradient(
            Dictionary<Value<TType>, Expression> variableExpressions,
            Dictionary<Value<TType>, Expression> gradientExpressions,
            List<Expression> expressionList);

        public UnariOpValue(int shape, string name, Value<TType> child) : base(shape, name, child)
        {
            ChildrensCompute = [ComputeGradient];
        }
        internal abstract Expr GetForwardComputation(Expr operand);

        internal sealed override Expression GetForwardComputation(Dictionary<Value<TType>, Expression> variableExpressions, Expression index)
        {
            Expression operand = Operand.GetAsOperand(variableExpressions, index);
            return GetForwardComputation(operand);
        }

        public override string ToString()
            => $"{Name}({Operand})";
    }
}