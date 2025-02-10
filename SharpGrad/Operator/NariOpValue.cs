using SharpGrad.DifEngine;
using SharpGrad.Operators;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public static Dimension[] GetShape(Value<TType>[] childs)
        {
            HashSet<Dimension> shape = new(childs.SelectMany(c => c.Shape));
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

        public static List<Expression> BuildForwardExpressionList(Dictionary<Value<TType>, Expression> variableExpressions, List<Expression> forwardExpressionList, Expression index, List<Value<TType>> topOSort)
        {
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
                    Expression variable = Expression.Variable(typeof(TType), v.Name);
                    variableExpressions[v] = variable;
                    Expression element = Expression.MakeIndex(Expression.Constant(v), typeof(Value<TType>).GetProperty("Item"), [index]);
                    Expression assign = Expression.Assign(variable, element);
                    forwardExpressionList.Add(assign);
                }
                else if (topOSort[i] is NariOpValue<TType> n)
                {
                    Expression variable = Expression.Variable(typeof(TType), n.Name);
                    variableExpressions[n] = variable;
                    Expression forwardComputation = n.GetForwardComputation(variableExpressions, index);
                    Expression assign = Expression.Assign(variable, forwardComputation);
                    forwardExpressionList.Add(assign);
                }
                else
                {
                    throw new InvalidOperationException("Unknown type");
                }
            }

            return forwardExpressionList;
        }
        public static List<Expression> BuildForwardExpressionList(Dictionary<Value<TType>, Expression> variableExpressions, Expression index, List<Value<TType>> topOSort)
            => BuildForwardExpressionList(variableExpressions, [], index, topOSort);


        public static List<ParameterExpression> SaveParameters(Dictionary<Value<TType>, Expression> variableExpressions, List<Expression> forwardExpressionList, Expression index, bool all = true)
        {
            List<ParameterExpression> parameters = [];

            foreach (var e in variableExpressions)
            {
                if (e.Value is ParameterExpression parameter)
                {
                    if (all || e.Key.IsOutput)
                    {
                        Expression arrayAccess = Expression.MakeIndex(Expression.Constant(e.Key), typeof(Value<TType>).GetProperty("Item"), [index]);
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

                    // Create Dimdexer and assign it
                    ParameterExpression dimdexer = Expression.Parameter(typeof(Dimdexer), nameof(dimdexer));
                    Expression getShape = Expression.PropertyOrField(Expression.Constant(this), nameof(Shape));
                    Expression newDimdexer = Expression.New(typeof(Dimdexer).GetConstructor([typeof(Dimension[])])!, getShape);
                    Expression assignDimdexer = Expression.Assign(dimdexer, newDimdexer);

                    // Current index and assign it
                    ParameterExpression current = Expression.Variable(typeof(Dimdices), nameof(current));
                    Expression currentExpression = Expression.PropertyOrField(dimdexer, nameof(Dimdexer.Current));
                    Expression assignCurrent = Expression.Assign(current, currentExpression);

                    // Get forward expression list
                    List<Expression> forwardExpressionList = [assignCurrent];
                    BuildForwardExpressionList(variableExpressions, forwardExpressionList, current, topOSort);

                    // Backup all parameters to data
                    List<ParameterExpression> parameters = SaveParameters(variableExpressions, forwardExpressionList, current);
                    parameters.Add(current);
                    parameters.Add(dimdexer);

                    // Condition MoveNext
                    MethodInfo methodMoveNext = typeof(Dimdexer).GetMethod(nameof(Dimdexer.MoveNext))!;

                    // Build loop expression until 'dimdexer.MoveNext()' is false
                    LabelTarget breakLabel = Expression.Label(nameof(breakLabel));
                    Expression loop = Expression.Loop(
                        Expression.IfThenElse(
                            Expression.Call(dimdexer, methodMoveNext),
                            Expression.Block(forwardExpressionList),
                            Expression.Break(breakLabel)
                        ),
                        breakLabel
                    );

                    // Build block and compile Expression
                    Expression finalBlock = Expression.Block(
                        parameters,
                        assignDimdexer,
                        loop);

                    forwardLambda = Expression.Lambda<Action>(finalBlock).Compile();
                }
                return forwardLambda;
            }
        }

        public static List<Expression> BuildBackwardExpressionList(
            Dictionary<Value<TType>, Expression> variableExpressions,
            Dictionary<Value<TType>, Expression> gradientExpressions,
            List<Expression> backwardExpressionList,
            ParameterExpression index,
            List<Value<TType>> topOSort)
        {
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
                    // Save the gradient to gradient using SetGradient method
                    MethodInfo setGradientMethod = typeof(Variable<TType>).GetMethod(nameof(SetGradient))!;
                    Expression setGradient = Expression.Call(
                        Expression.Constant(v),
                        setGradientMethod, 
                        [index, gradientExpressions[v]]);
                    backwardExpressionList.Add(setGradient);
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
                    ClearGradient();
                    gradientExpressions.Clear();
                    gradientExpressions.Add(this, Expression.Constant(TType.One));

                    // Create Dimdexer and assign it
                    ParameterExpression dimdexer = Expression.Parameter(typeof(Dimdexer), nameof(dimdexer));
                    Expression getShape = Expression.PropertyOrField(Expression.Constant(this), nameof(Shape));
                    Expression newDimdexer = Expression.New(typeof(Dimdexer).GetConstructor([typeof(Dimension[])])!, getShape);
                    Expression assignDimdexer = Expression.Assign(dimdexer, newDimdexer);

                    // Current index and assign it
                    ParameterExpression current = Expression.Variable(typeof(Dimdices), nameof(current));
                    Expression currentExpression = Expression.PropertyOrField(dimdexer, nameof(Dimdexer.Current));
                    Expression assignCurrent = Expression.Assign(current, currentExpression);


                    List<Expression> backwardExpressionList = [assignCurrent];

                    if (topOSort is null)
                    {
                        topOSort = [];
                        DFS(topOSort, []);
                        variableExpressions.Clear();
                        BuildForwardExpressionList(variableExpressions, backwardExpressionList, current, topOSort);
                        SaveParameters(variableExpressions, backwardExpressionList, current, false);
                    }
                    else
                    {
                        // Load data
                        foreach (var e in variableExpressions)
                        {
                            if (e.Value is ParameterExpression parameter)
                            {
                                Expression arrayAccess = Expression.MakeIndex(
                                    Expression.Constant(e.Key),
                                    typeof(Value<TType>).GetProperty("Item"),
                                    [current]);
                                backwardExpressionList.Add(Expression.Assign(parameter, arrayAccess));
                            }
                        }
                    }

                    BuildBackwardExpressionList(variableExpressions, gradientExpressions, backwardExpressionList, current, topOSort);

                    // Build block and compile Expression
                    List<ParameterExpression> parameters = variableExpressions.Values.OfType<ParameterExpression>().ToList();
                    parameters.AddRange(gradientExpressions.Values.OfType<ParameterExpression>());
                    parameters.Add(current);
                    parameters.Add(dimdexer);

                    // Condition MoveNext
                    MethodInfo methodMoveNext = typeof(Dimdexer).GetMethod(nameof(Dimdexer.MoveNext))!;

                    // Build loop expression until 'dimdexer.MoveNext()' is false
                    LabelTarget breakLabel = Expression.Label(nameof(breakLabel));
                    Expression loop = Expression.Loop(
                        Expression.IfThenElse(
                            Expression.Call(dimdexer, methodMoveNext),
                            Expression.Block(backwardExpressionList),
                            Expression.Break(breakLabel)
                        ),
                        breakLabel
                    );

                    Expression backwardExpression = Expression.Block(
                        parameters,
                        assignDimdexer,
                        loop);
                    backwardLambda = Expression.Lambda<Action>(backwardExpression).Compile();
                }
                return backwardLambda;
            }
        }
    }
}