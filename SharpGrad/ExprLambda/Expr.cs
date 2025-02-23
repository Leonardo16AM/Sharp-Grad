using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SharpGrad.ExprLambda
{
    public class Expr(Expression expression)
    {
        public readonly Expression Expression = expression;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Expr Add(Expr left)
            => left;
        public static Expr operator +(Expr left)
            => Add(left);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Expr Add(Expr left, Expr right)
            => new BinaryExpr(ExpressionType.Add, left, right);
        public static Expr operator +(Expr left, Expr right)
            => Add(left, right);
        public static Expr operator +(Expr left, Expression right)
            => Add(left, right);
        public static Expr operator +(Expression left, Expr right)
            => Add(left, right);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Expr Negate(Expr left)
            => Expression.Negate(left);
        public static Expr operator -(Expr left)
            => Expression.Negate(left);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Expr Subtract(Expr left, Expr right)
            => new BinaryExpr(ExpressionType.Subtract, left, right);
        public static Expr operator -(Expr left, Expr right)
            => Subtract(left, right);
        public static Expr operator -(Expr left, Expression right)
            => Subtract(left, right);
        public static Expr operator -(Expression left, Expr right)
            => Subtract(left, right);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Expr Multiply(Expr left, Expr right)
            => new BinaryExpr(ExpressionType.Multiply, left, right);
        public static Expr operator *(Expr left, Expr right)
            => Multiply(left, right);
        public static Expr operator *(Expr left, Expression right)
            => Multiply(left, right);
        public static Expr operator *(Expression left, Expr right)
            => Multiply(left, right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Expr Divide(Expr left, Expr right)
            => new BinaryExpr(ExpressionType.Divide, left, right);
        public static Expr operator /(Expr left, Expr right)
            => Divide(left, right);
        public static Expr operator /(Expr left, Expression right)
            => Divide(left, right);
        public static Expr operator /(Expression left, Expr right)
            => Divide(left, right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Expr Modulo(Expr left, Expr right)
            => new BinaryExpr(ExpressionType.Modulo, left, right);
        public static Expr operator %(Expr left, Expr right)
            => Modulo(left, right);
        public static Expr operator %(Expr left, Expression right)
            => Modulo(left, right);
        public static Expr operator %(Expression left, Expr right)
            => Modulo(left, right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Expr Xor(Expr left, Expr right)
            => new BinaryExpr(ExpressionType.ExclusiveOr, left, right);
        public static Expr operator ^(Expr left, Expr right)
            => Xor(left, right);
        public static Expr operator ^(Expr left, Expression right)
            => Xor(left, right);
        public static Expr operator ^(Expression left, Expr right)
            => Xor(left, right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Expr Or(Expr left, Expr right)
            => new BinaryExpr(ExpressionType.OrElse, left, right);
        public static Expr operator |(Expr left, Expr right)
            => Or(left, right);
        public static Expr operator |(Expr left, Expression right)
            => Or(left, right);
        public static Expr operator |(Expression left, Expr right)
            => Or(left, right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Expr And(Expr left, Expr right)
            => new BinaryExpr(ExpressionType.AndAlso, left, right);
        public static Expr operator &(Expr left, Expr right)
            => And(left, right);
        public static Expr operator &(Expr left, Expression right)
            => And(left, right);
        public static Expr operator &(Expression left, Expr right)
            => And(left, right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Expr Not(Expr left)
            => Expression.Not(left);
        public static Expr operator !(Expr left)
            => Expression.Not(left);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Expr Equal(Expr left, Expr right)
            => new BinaryExpr(ExpressionType.Equal, left, right);
        public static Expr operator ==(Expr left, Expr right)
            => Equal(left, right);
        public static Expr operator ==(Expr left, Expression right)
            => Equal(left, right);
        public static Expr operator ==(Expression left, Expr right)
            => Equal(left, right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Expr NotEqual(Expr left, Expr right)
            => new BinaryExpr(ExpressionType.NotEqual, left, right);
        public static Expr operator !=(Expr left, Expr right)
            => NotEqual(left, right);
        public static Expr operator !=(Expr left, Expression right)
            => NotEqual(left, right);
        public static Expr operator !=(Expression left, Expr right)
            => NotEqual(left, right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Expr GreaterThan(Expr left, Expr right)
            => new BinaryExpr(ExpressionType.GreaterThan, left, right);
        public static Expr operator >(Expr left, Expr right)
            => GreaterThan(left, right);
        public static Expr operator >(Expr left, Expression right)
            => GreaterThan(left, right);
        public static Expr operator >(Expression left, Expr right)
            => GreaterThan(left, right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Expr GreaterThanOrEqual(Expr left, Expr right)
            => new BinaryExpr(ExpressionType.GreaterThanOrEqual, left, right);
        public static Expr operator >=(Expr left, Expr right)
            => GreaterThanOrEqual(left, right);
        public static Expr operator >=(Expr left, Expression right)
            => GreaterThanOrEqual(left, right);
        public static Expr operator >=(Expression left, Expr right)
            => GreaterThanOrEqual(left, right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Expr LessThan(Expr left, Expr right)
            => new BinaryExpr(ExpressionType.LessThan, left, right);
        public static Expr operator <(Expr left, Expr right)
            => LessThan(left, right);
        public static Expr operator <(Expr left, Expression right)
            => LessThan(left, right);
        public static Expr operator <(Expression left, Expr right)
            => LessThan(left, right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Expr LessThanOrEqual(Expr left, Expr right)
            => new BinaryExpr(ExpressionType.LessThanOrEqual, left, right);

        public static Expr Condition(Expr test, Expr ifTrue, Expr ifFalse)
            => Expression.Condition(test, ifTrue, ifFalse);

        public static MethodCallExpression Call(MethodInfo method, Expression arg0)
            => Expression.Call(method, arg0);

        public static Expr Constant(object? value)
            => Expression.Constant(value);

        public static Expr operator <=(Expr left, Expr right)
            => LessThanOrEqual(left, right);
        public static Expr operator <=(Expr left, Expression right)
            => LessThanOrEqual(left, right);
        public static Expr operator <=(Expression left, Expr right)
            => LessThanOrEqual(left, right);



        public static implicit operator Expr(Expression expression)
            => new(expression);
        public static implicit operator Expression(Expr expr)
            => expr.Expression;
    }

    internal class BinaryExpr(ExpressionType nodeType, Expr left, Expr right) : Expr(Expression.MakeBinary(nodeType, left, right))
    { }
}
