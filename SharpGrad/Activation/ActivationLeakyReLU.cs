using SharpGrad.DifEngine;
using SharpGrad.Operator;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;

namespace SharpGrad.Activation
{
    public abstract class Activation<TType>(int shape, string name, Value<TType> child) : UnariOpValue<TType>(shape, name, child)
        where TType : INumber<TType>
    {
    }

    public class ActivationLeakyReLU<TType> : Activation<TType>
        where TType : IBinaryFloatingPointIeee754<TType>, IComparable<TType>
    {
        private readonly TType _alpha;

        public static int GetShape(Value<TType> value)
            => value.Shape;

        public ActivationLeakyReLU(Value<TType> value, TType alpha)
            : base(GetShape(value), "leaky_relu", value)
        { }

        internal override Expression GetForwardComputation(Dictionary<Value<TType>, Expression> variableExpressions)
            => Expression.Condition(
                Expression.LessThanOrEqual(Operand.GetAsOperand(variableExpressions), ExpressionZero),
                Expression.Multiply(Expression.Constant(_alpha), Operand.GetAsOperand(variableExpressions)),
                Operand.GetAsOperand(variableExpressions));

        protected override void ComputeGradient(Dictionary<Value<TType>, Expression> variableExpressions, Dictionary<Value<TType>, Expression> gradientExpressions, List<Expression> expressionList)
        {
            Expression grad = gradientExpressions[this];
            Expression relu = variableExpressions[this];
            Expression gr = Expression.Condition(
                Expression.LessThanOrEqual(relu, Expression.Constant(TType.Zero)),
                Expression.Multiply(Expression.Constant(_alpha), grad),
                grad);
            AssignGradientExpession(gradientExpressions, expressionList, Operand, gr);
        }
    }
}
