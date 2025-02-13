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
        public delegate Expression ComputeGradientDelegate(
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
            HashSet<Dimension> shape = [.. childs.SelectMany(c => c.Shape)];
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

        #region Forward pass
        public static List<Expression> BuildForwardExpressionList(Dictionary<Value<TType>, Expression> variableExpressions, List<Expression> forwardExpressionList, Expression index, List<Value<TType>> topOSort)
        {
            for (int i = 0; i < topOSort.Count; i++)
            {
                Value<TType> e = topOSort[i];
                Debug.Assert(!variableExpressions.ContainsKey(e));

                Expression variable = Expression.Variable(typeof(TType), e.ToString().Replace(" ", string.Empty));
                variableExpressions[e] = variable;
                Expression element = e switch
                {
                    Constant<TType> c => Expression.MakeIndex(Expression.Constant(c), typeof(Constant<TType>).GetProperty("Item"), [index]),
                    Variable<TType> v => Expression.MakeIndex(Expression.Constant(v), typeof(Variable<TType>).GetProperty("Item"), [index]),
                    NariOpValue<TType> n => n.GetForwardComputation(variableExpressions, index),
                    _ => throw new InvalidOperationException("Unknown type")
                };
                forwardExpressionList.Add(Expression.Assign(variable, element));
            }
            topOSort[^1].IsOutput = true;
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
                        if (e.Key is not Constant<TType>)
                        {

                            Expression arrayAccess = Expression.MakeIndex(Expression.Constant(e.Key), typeof(Value<TType>).GetProperty("Item"), [index]);
                            forwardExpressionList.Add(Expression.Assign(arrayAccess, parameter));
                        }
                        parameters.Add(parameter);
                    }
                }
            }

            return parameters;
        }


        private Action? forwardLambda;
        private Action GetForwardLambda()
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
        public void Forward() => GetForwardLambda()();
        #endregion

        #region Backward pass
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
                        /// TODO : Avoid computing gradient for constant or unnecessary variables
                        var grad = n.ChildrensCompute[j](variableExpressions, gradientExpressions, backwardExpressionList);
                        var operand = n.Operands[j];
                        operand.AssignGradientExpession(gradientExpressions, backwardExpressionList, index, operand, grad);
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
        private Action GetBackwardLambda()
        {
            if (backwardLambda is null)
            {
                //InitGradient();
                //gradientExpressions.Clear();
                gradientExpressions.Add(this, Expression.Constant(TType.One));

                // Create Dimdexer and assign it
                ParameterExpression dimdexer = Expression.Parameter(typeof(Dimdexer), nameof(dimdexer));
                Expression getShape = Expression.PropertyOrField(Expression.Constant(this), nameof(Shape));
                Expression newDimdexer = Expression.New(typeof(Dimdexer).GetConstructor([typeof(Dimension[])])!, getShape);
                Expression assignDimdexer = Expression.Assign(dimdexer, newDimdexer);

                // Current index and assign it
                ParameterExpression current = Expression.Variable(typeof(Dimdices), nameof(current));
                Expression currentExpression = Expression.Property(dimdexer, nameof(Dimdexer.Current));
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
                    // Load forward variables
                    /// TODO : Limit to only load non-constant or scalar variables. Load constant and scalar should be done before the loop
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
                HashSet<ParameterExpression> parameters = [.. variableExpressions.Values.OfType<ParameterExpression>()];
                foreach (var e in gradientExpressions)
                {
                    if (e.Value is ParameterExpression parameter)
                    {
                        parameters.Add(parameter);
                    }
                }
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
        public void Backward() => GetBackwardLambda()();
        #endregion
    }
}