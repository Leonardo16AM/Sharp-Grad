using System;
using System.Linq.Expressions;
using System.Reflection;

namespace SharpGrad;
public static class MethodToExpressionTree
{
    public static Expression<Func<float>> MonExpression = () => 1;

    public static Expression<Func<T, TResult>> GetExpressionTree<T, TResult>(MethodInfo methodInfo)
    {
        if (methodInfo == null)
            throw new ArgumentNullException(nameof(methodInfo));

        var parameters = methodInfo.GetParameters();
        if (parameters.Length != 0)
            throw new ArgumentException("La méthode ne doit avoir aucun paramètre.", nameof(methodInfo));

        //var parameterType = parameters[0].ParameterType;
        //if (parameterType != typeof(T))
        //    throw new ArgumentException($"Le type de paramètre de la méthode doit être {typeof(T)}.", nameof(methodInfo));

        var returnType = methodInfo.ReturnType;
        if (returnType != typeof(TResult))
            throw new ArgumentException($"Le type de retour de la méthode doit être {typeof(TResult)}.", nameof(methodInfo));

        var parameter = Expression.Parameter(typeof(T), "x");
        var methodCall = Expression.Call(methodInfo, parameter);
        var lambda = Expression.Lambda<Func<T, TResult>>(methodCall, parameter);

        return lambda;
    }
}