using SharpGrad.DifEngine;
using SharpGrad.ExprLambda;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;

namespace SharpGrad.Operators
{
    public abstract class UnariOperation<TType> :
        NariOperation<TType>
        where TType : INumber<TType>
    {
        public Value<TType> Operand => Operands[0];

        public sealed override ComputeGradientDelegate[] ChildrensCompute { get; }
        protected abstract Expression ComputeGradient(
            Dictionary<Value<TType>, Expression> variableExpressions,
            Dictionary<Value<TType>, Expression> gradientExpressions,
            List<Expression> expressionList);

        public UnariOperation(string name, Value<TType> child)
            : base(name, child)
        {
            ChildrensCompute = [ComputeGradient];
        }
        internal abstract Expr GetForwardComputation(Expr operand);

        internal sealed override Expression GetForwardComputation(
            Dictionary<Value<TType>, Expression> variableExpressions,
            List<Expression> forwardExpressionList,
            Expression index)
        {
            Expression variable = Expression.Variable(typeof(TType), Name);
            variableExpressions[this] = variable;
            Expression operand = Operand.GetAsOperand(variableExpressions, index);
            forwardExpressionList.Add(Expression.Assign(variable, GetForwardComputation(operand)));
            return variable;
        }

        public override string ToString()
            => $"{Name}({Operand})";
    }
}