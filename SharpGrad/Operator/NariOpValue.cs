using SharpGrad.DifEngine;
using SharpGrad.Operators;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;

namespace SharpGrad.Operator
{
    public abstract class NariOpValue<TType> : Value<TType>
    where TType : INumber<TType>
    {
        public delegate void ComputeGradientDelegate(
            Dictionary<Value<TType>, Expression> VariableExpressions,
            Dictionary<Value<TType>, Expression> GradientExpressions,
            List<Expression> expressionList);

        public abstract ComputeGradientDelegate[] ChildrensCompute { get; }

        public override TType[] Data
        {
            get
            {
                //if (!isComputed)
                //{
                //    ForwardLambda();
                //    isComputed = true;
                //}
                return data;
            }
        }

        public static IReadOnlyList<Dimension> GetShape(Value<TType>[] childs)
        {
            HashSet<Dimension> shape = [];
            foreach (var child in childs)
            {
                foreach (var dim in child.Shape)
                {
                    shape.Add(dim);
                }
            }
            return [.. shape];
        }

        public NariOpValue(string name, params Value<TType>[] childs)
            : base(GetShape(childs), name, childs)
        {
            if (childs.Length < 1)
                throw new ArgumentException($"Operator {name} must have at least one child.");
        }

        internal abstract Expression GetForwardComputation(Dictionary<Value<TType>, Expression> variableExpressions, Expression index);

        public override bool GetAsOperand(Dictionary<Value<TType>, Expression> variableExpressions, List<Expression> forwardExpressionList, Expression index, out Expression? operand)
        {
            if (!variableExpressions.TryGetValue(this, out operand))
            {
                for (int i = 0; i < Operands.Length; i++)
                {
                    Operands[i].BuildForward(variableExpressions, forwardExpressionList, index);
                }
                operand = Expression.Variable(typeof(TType), Name);
                variableExpressions[this] = operand;
                forwardExpressionList.Add(Expression.Assign(operand, GetForwardComputation(variableExpressions, index)));
            }
            return true;
        }

        private List<Value<TType>>? topOSort = null;
        private readonly Dictionary<Value<TType>, Expression> variableExpressions = [];
        private readonly Dictionary<Value<TType>, Expression> gradientExpressions = [];

        public static List<Expression> BuildForwardExpressionList(Dictionary<Value<TType>, Expression> variableExpressions, Expression index, List<Value<TType>> topOSort)
        {
            List<Expression> forwardExpressionList = [];

            for (int i = 0; i < topOSort.Count; i++)
            {
                Value<TType> e = topOSort[i];
                Debug.Assert(!variableExpressions.ContainsKey(e));
                if (e is Constant<TType> c)
                {
                    variableExpressions[c] = c.GetExpression(index);
                }
                else if (e is Variable<TType> v)
                {
                    Expression expression = Expression.Variable(typeof(TType), v.Name);
                    variableExpressions[v] = expression;
                    Expression field = Expression.Field(Expression.Constant(v), nameof(data));
                    Expression element = Expression.ArrayAccess(field, v.IsScalar
                        ? Expression.Constant(0)
                        : index);
                    Expression assign = Expression.Assign(expression, element);
                    forwardExpressionList.Add(assign);
                }
                else if (topOSort[i] is NariOpValue<TType> n)
                {
                    Expression expression = Expression.Variable(typeof(TType), n.Name);
                    variableExpressions[n] = expression;
                    Expression forwardComputation = n.GetForwardComputation(variableExpressions, index);
                    Expression assign = Expression.Assign(expression, forwardComputation);
                    forwardExpressionList.Add(assign);
                }
                else
                {
                    throw new InvalidOperationException("Unknown type");
                }
            }

            return forwardExpressionList;
        }

        public static List<ParameterExpression> SaveParameters(Dictionary<Value<TType>, Expression> variableExpressions, List<Expression> forwardExpressionList, Expression index, bool all = true)
        {
            List<ParameterExpression> parameters = [];

            foreach (var e in variableExpressions)
            {
                if (e.Value is ParameterExpression parameter)
                {
                    if (all || e.Key.IsOutput)
                    {
                        Expression field = Expression.Field(Expression.Constant(e.Key), nameof(data));
                        Expression arrayAccess = Expression.ArrayAccess(field, (e.Key.IsScalar) ? Expression.Constant(0) : index);
                        forwardExpressionList.Add(Expression.Assign(arrayAccess, parameter));
                        parameters.Add(parameter);
                    }
                }
            }

            return parameters;
        }

        private Action? forwardLambda;
        public Action ForwardLambda
        {
            get
            {
                if (forwardLambda is null)
                {
                    if (topOSort is null)
                    {
                        topOSort = [];
                        DFS(topOSort, []);
                    }
                    variableExpressions.Clear();

                    ParameterExpression index = Expression.Parameter(typeof(int), "index");
                    List<Expression> forwardExpressionList = BuildForwardExpressionList(variableExpressions, index, topOSort);

                    // Backup all parameters to data
                    List<ParameterExpression> parameters = SaveParameters(variableExpressions, forwardExpressionList, index);
                    parameters.Add(index);

                    // Build loop expression until index equals data.Length
                    LabelTarget breakLabel = Expression.Label("ForwardLoopBreak");
                    Expression loopBody = Expression.Block(forwardExpressionList.Append(Expression.PostIncrementAssign(index)));
                    Expression loop = Expression.Loop(
                        Expression.IfThenElse(
                            Expression.LessThan(index, Expression.Constant(data.Length)),
                            loopBody,
                            Expression.Break(breakLabel)
                        ),
                        breakLabel
                    );

                    // Build block and compile Expression
                    loop = Expression.Block(
                        parameters,
                        Expression.Assign(index, Expression.Constant(0)),
                        loop);
                    forwardLambda = Expression.Lambda<Action>(loop).Compile();
                }
                return forwardLambda;
            }
        }


        public static List<Expression> BuildBackwardExpressionList(
            Dictionary<Value<TType>, Expression> variableExpressions,
            Dictionary<Value<TType>, Expression> gradientExpressions,
            ParameterExpression index,
            List<Value<TType>> topOSort)
        {
            List<Expression> backwardExpressionList = [];
            for (int i = topOSort.Count - 1; i >= 0; i--)
            {
                Value<TType> e = topOSort[i];
                if (e is Constant<TType> c)
                {
                    continue;
                }
                else if (e is Variable<TType> v)
                {
                    // This is the last use of the variable.
                    // Save the gradient to Grad field.
                    Expression gradField = Expression.Field(Expression.Constant(v), nameof(Grad));
                    Expression arrayAccess = Expression.ArrayAccess(gradField, (v.Shape.Size() == 1) ? Expression.Constant(0) : index);
                    backwardExpressionList.Add(Expression.Assign(arrayAccess, gradientExpressions[v]));
                }
                else if (topOSort[i] is NariOpValue<TType> n)
                {
                    for (int j = 0; j < n.ChildrensCompute.Length; j++)
                    {
                        n.ChildrensCompute[j](variableExpressions, gradientExpressions, backwardExpressionList);
                    }
                }
                else
                {
                    throw new InvalidOperationException("Unknown type");
                }
            }
            return backwardExpressionList;
        }

        private Action? backwardLambda;
        public Action BackwardLambda
        {
            get
            {
                if (backwardLambda is null)
                {
                    Array.Fill(Grad, TType.Zero);
                    gradientExpressions.Clear();
                    gradientExpressions.Add(this, Expression.Constant(TType.One));

                    ParameterExpression index = Expression.Parameter(typeof(int), "index");
                    Expression init = Expression.Assign(index, Expression.Constant(0));
                    List<Expression> backwardExpressionList = [init];

                    if (topOSort is null)
                    {
                        topOSort = [];
                        DFS(topOSort, []);
                        backwardExpressionList.AddRange(BuildForwardExpressionList(variableExpressions, index, topOSort));
                        SaveParameters(variableExpressions, backwardExpressionList, index, false);
                    }
                    else
                    {
                        // Load data
                        foreach (var e in variableExpressions)
                        {
                            if (e.Value is ParameterExpression parameter)
                            {
                                Expression field = Expression.Field(Expression.Constant(e.Key), nameof(data));
                                Expression arrayAccess = Expression.ArrayAccess(field, (e.Key.Shape.Size() == 1) ? Expression.Constant(0) : index);
                                backwardExpressionList.Add(Expression.Assign(parameter, arrayAccess));
                            }
                        }
                    }

                    backwardExpressionList.AddRange(BuildBackwardExpressionList(variableExpressions, gradientExpressions, index, topOSort));

                    // Build block and compile Expression
                    List<ParameterExpression> parameters = [index];
                    parameters.AddRange(variableExpressions.Values.OfType<ParameterExpression>());
                    parameters.AddRange(gradientExpressions.Values.OfType<ParameterExpression>());

                    LabelTarget breakLabel = Expression.Label("LoopBreak");
                    Expression loop = Expression.Loop(
                        Expression.IfThenElse(
                            Expression.LessThan(index, Expression.Constant(data.Length)),
                            Expression.Block(backwardExpressionList.Append(Expression.PostIncrementAssign(index))),
                            Expression.Break(breakLabel)
                        ),
                        breakLabel
                    );

                    Expression backwardExpression = Expression.Block(parameters, loop);
                    backwardLambda = Expression.Lambda<Action>(backwardExpression).Compile();
                }
                return backwardLambda;
            }
        }
    }
}