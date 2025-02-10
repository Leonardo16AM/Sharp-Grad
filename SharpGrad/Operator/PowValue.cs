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
            Expr grad = gradientExpressions[this];
            Expr left = variableExpressions[LeftOperand];
            Expr right = variableExpressions[RightOperand];
            Expr pow = Expression.Call(typeof(TType).GetMethod("Pow", [typeof(TType), typeof(TType)])!, left, right - Expr.Constant(TType.One));
            Expr gr = grad * right * pow;
            AssignGradientExpession(gradientExpressions, expressionList, LeftOperand, gr);
        }

        protected override void ComputeRightGradient(Dictionary<Value<TType>, Expression> variableExpressions, Dictionary<Value<TType>, Expression> gradientExpressions, List<Expression> expressionList)
        {
            Expr grad = gradientExpressions[this];
            Expr left = variableExpressions[LeftOperand];
            Expr right = variableExpressions[RightOperand];
            Expr logl = Expression.Call(typeof(TType).GetMethod("Log", [typeof(TType)])!, left);
            Expr lr = Expression.Call(typeof(TType).GetMethod("Pow", [typeof(TType), typeof(TType)])!, left, right);
            Expr gr = grad * lr * logl;
            AssignGradientExpession(gradientExpressions, expressionList, RightOperand, gr);
        }
    }
}