using SharpGrad.DifEngine;
using SharpGrad.ExprLambda;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;

namespace SharpGrad.Operators
{
    public class PowValue<TType> :
            BinaryOpValue<TType>
            where TType : INumber<TType>, IPowerFunctions<TType>, ILogarithmicFunctions<TType>
    {
        public PowValue(Value<TType> left, Value<TType> right)
            : base("^", left, right)
        { }

        protected override Expr GetForwardComputation(Expr left, Expr right)
            => Expression.Call(typeof(TType).GetMethod("Pow", new[] { typeof(TType), typeof(TType) })!, left, right);

        protected override void ComputeLeftGradient(Dictionary<Value<TType>, Expression> variableExpressions, Dictionary<Value<TType>, Expression> gradientExpressions, List<Expression> expressionList)
        {
            Expression grad = gradientExpressions[this];
            Expression left = variableExpressions[LeftOperand];
            Expression right = variableExpressions[RightOperand];
            Expression r1 = Expression.Subtract(right, Expression.Constant(TType.One));
            Expression lr1 = Expression.Call(typeof(TType).GetMethod("Pow", [typeof(TType), typeof(TType)])!, left, r1);
            Expression gr = Expression.Multiply(grad, Expression.Multiply(right, lr1));
            AssignGradientExpession(gradientExpressions, expressionList, LeftOperand, gr);
        }

        protected override void ComputeRightGradient(Dictionary<Value<TType>, Expression> variableExpressions, Dictionary<Value<TType>, Expression> gradientExpressions, List<Expression> expressionList)
        {
            Expression grad = gradientExpressions[this];
            Expression left = variableExpressions[LeftOperand];
            Expression right = variableExpressions[RightOperand];
            Expression logl = Expression.Call(typeof(TType).GetMethod("Log", [typeof(TType)])!, left);
            Expression lr = Expression.Call(typeof(TType).GetMethod("Pow", [typeof(TType), typeof(TType)])!, left, right);
            Expression gr = Expression.Multiply(grad, Expression.Multiply(lr, logl));
            AssignGradientExpession(gradientExpressions, expressionList, RightOperand, gr);
        }
    }
}