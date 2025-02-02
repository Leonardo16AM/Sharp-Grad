using SharpGrad.DifEngine;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;

namespace SharpGrad.Operators
{
    public class PowValue<TType> :
        BinaryOpValue<TType>
        where TType : INumber<TType>, IPowerFunctions<TType>, ILogarithmicFunctions<TType>
    {
        public static int GetShape(Value<TType> left, Value<TType> right)
        {
            if (left.Shape != right.Shape)
            {
                throw new System.ArgumentException("Shapes of left and right operands must be equal.");
            }
            return left.Shape;
        }

        public PowValue(Value<TType> left, Value<TType> right)
            : base(GetShape(left, right), "^", left, right)
        { }

        protected override Expression GetForwardComputation(Expression left, Expression right)
            => Expression.Call(typeof(TType).GetMethod("Pow")!, left, right);

        protected override void ComputeLeftGradient(Dictionary<Value<TType>, Expression> variableExpressions, Dictionary<Value<TType>, Expression> gradientExpressions, List<Expression> expressionList)
        {
            Expression grad = gradientExpressions[this];
            Expression left = variableExpressions[LeftOperand];
            Expression right = variableExpressions[RightOperand];
            Expression r1 = Expression.Subtract(right, Expression.Constant(TType.One));
            Expression lr1 = Expression.Call(typeof(TType).GetMethod("Pow")!, left, r1);
            Expression gr = Expression.Multiply(grad, Expression.Multiply(right, lr1));
            AssignGradientExpession(gradientExpressions, expressionList, LeftOperand, gr);
        }

        protected override void ComputeRightGradient(Dictionary<Value<TType>, Expression> variableExpressions, Dictionary<Value<TType>, Expression> gradientExpressions, List<Expression> expressionList)
        {
            Expression grad = gradientExpressions[this];
            Expression left = variableExpressions[LeftOperand];
            Expression right = variableExpressions[RightOperand];
            Expression logl = Expression.Call(typeof(TType).GetMethod("Log")!, left);
            Expression lr = Expression.Call(typeof(TType).GetMethod("Pow")!, left, right);
            Expression gr = Expression.Multiply(grad, Expression.Multiply(lr, logl));
            AssignGradientExpession(gradientExpressions, expressionList, RightOperand, gr);
        }
    }
}