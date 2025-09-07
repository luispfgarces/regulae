namespace Regulae.Evaluation.Compiled
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;
    using Regulae;
    using Regulae.ConditionNodes;
    using Regulae.Evaluation;
    using Regulae.Evaluation.Compiled.ExpressionBuilders;

    internal sealed class RuleConditionsExpressionBuilder : IRuleConditionsExpressionBuilder
    {
        private static readonly MethodInfo multiplicityEvaluateMethod = typeof(MultiplicityEvaluator)
            .GetMethod(nameof(MultiplicityEvaluator.Evaluate))!;
        private static readonly FieldInfo operandCardinalityField = typeof(Operand).GetField("Cardinality")!;
        private static readonly FieldInfo operandValueField = typeof(Operand).GetField("Value")!;

        private static readonly MethodInfo tryGetValueMethod = typeof(IDictionary<string, Operand>)
            .GetMethod(nameof(IDictionary<string, Operand>.TryGetValue))!;

        private readonly IDataTypesConfigurationProvider dataTypesConfigurationProvider;
        private readonly RulesEngineOptions rulesEngineOptions;
        private readonly IValueConditionNodeExpressionBuilderProvider valueConditionNodeExpressionBuilderProvider;

        public RuleConditionsExpressionBuilder(
            IValueConditionNodeExpressionBuilderProvider valueConditionNodeExpressionBuilderProvider,
            IDataTypesConfigurationProvider dataTypesConfigurationProvider,
            RulesEngineOptions rulesEngineOptions)
        {
            this.valueConditionNodeExpressionBuilderProvider = valueConditionNodeExpressionBuilderProvider;
            this.dataTypesConfigurationProvider = dataTypesConfigurationProvider;
            this.rulesEngineOptions = rulesEngineOptions;
        }

        public Expression<Func<IDictionary<string, Operand>, bool>> BuildExpression(IConditionNode rootConditionNode, MatchModes matchMode)
        {
            var expressionResult = ExpressionBuilder.NewExpression("EvaluateConditions")
                .WithParameters(p =>
                {
                    p.CreateParameter<IDictionary<string, Operand>>("conditions");
                })
                .HavingReturn<bool>(defaultValue: false)
                .SetImplementation(x =>
                {
                    var parameterExpression = x.GetParameter("conditions");
                    var leftOperandVariableExpression = x.CreateVariable<Operand>("leftOperand");
                    var resultVariableExpression = x.CreateVariable<bool>("Result");

                    this.BuildExpression(x, rootConditionNode, parameterExpression, matchMode);

                    x.Return(resultVariableExpression);
                })
                .Build();

            return Expression.Lambda<Func<IDictionary<string, Operand>, bool>>(
                body: expressionResult.Implementation,
                parameters: expressionResult.Parameters);
        }

        private static void BuildBehaviorWhenLeftOperandMissing(
            IExpressionBlockBuilder builder,
            ParameterExpression leftOperandVariableExpression,
            Expression leftOperandValueFieldAccessExpression,
            MissingConditionBehaviors missingConditionBehavior)
        {
            var resultVariableExpression = builder.GetVariable("Result");
            var jumpToLabelTarget = builder.CreateLabelTarget("LabelEndValueConditionNode");
            builder.If(
                test => builder.OrElse(
                    builder.Equal(leftOperandVariableExpression, builder.Constant<object>(value: null!)),
                    builder.Equal(leftOperandValueFieldAccessExpression, builder.Constant<object>(value: null!))),
                then => then.Block(block =>
                {
                    block.Assign(resultVariableExpression, block.Constant(value: missingConditionBehavior != MissingConditionBehaviors.Discard));
                    block.Goto(jumpToLabelTarget);
                }));
        }

        private void BuildBehaviorForManyMultiplicities(
            IExpressionBlockBuilder builder,
            ValueConditionNode valueConditionNode,
            OperatorMetadata operatorMetadata,
            Expression leftOperandValueFieldAccessExpression,
            Expression testPresentLeftOperand)
        {
            var leftOperandVariableExpression = builder.GetVariable("leftOperand");
            var resultVariableExpression = builder.GetVariable("Result");
            var dataTypeConfiguration = this.dataTypesConfigurationProvider.GetDataTypeConfiguration(valueConditionNode.RightOperand.DataType);
            var multiplicityVariableExpression = builder.CreateVariable<Multiplicities>("Multiplicity");
            builder.Assign(multiplicityVariableExpression, builder.Call(
                instance: null!,
                multiplicityEvaluateMethod,
                [
                    builder.AccessField(leftOperandVariableExpression, operandCardinalityField),
                    builder.Constant(valueConditionNode.RightOperand.Cardinality),
                ]));

            builder.Switch(multiplicityVariableExpression, @switch =>
            {
                foreach (var multiplicity in operatorMetadata.SupportedMultiplicities)
                {
                    var multiplicityTransformed = multiplicity.ToString();
                    var scopeName = new StringBuilder(builder.ScopeName)
                        .Append(valueConditionNode.Condition)
                        .Append(multiplicityTransformed)
                        .ToString();
                    @switch.Case(
                        builder.Constant(multiplicity),
                        caseBuilder => caseBuilder.Block(scopeName, block =>
                        {
                            var valueConditionNodeExpressionBuilder = this.valueConditionNodeExpressionBuilderProvider
                                .GetExpressionBuilder(multiplicity);
                            var args = new BuildValueConditionNodeExpressionArgs
                            {
                                DataTypeConfiguration = dataTypeConfiguration,
                                LeftOperandExpression = leftOperandValueFieldAccessExpression,
                                Operator = operatorMetadata.Operator,
                                ResultVariableExpression = resultVariableExpression,
                                RightOperandExpression = builder.Constant(valueConditionNode.RightOperand.Value),
                                TestLeftOperand = testPresentLeftOperand,
                            };
                            valueConditionNodeExpressionBuilder.Build(
                                block,
                                args);
                        }));
                }
                @switch.Default(defaultBuilder => defaultBuilder.Empty());
            });
        }

        private void BuildBehaviorForSingleMultiplicity(
            IExpressionBlockBuilder builder,
            ValueConditionNode valueConditionNode,
            OperatorMetadata operatorMetadata,
            Expression leftOperandValueFieldAccessExpression,
            Expression testPresentLeftOperand)
        {
            var resultVariableExpression = builder.GetVariable("Result");
            var dataTypeConfiguration = this.dataTypesConfigurationProvider.GetDataTypeConfiguration(valueConditionNode.RightOperand.DataType);
            var multiplicity = operatorMetadata.SupportedMultiplicities.First();
            var valueConditionNodeExpressionBuilder = this.valueConditionNodeExpressionBuilderProvider
                                .GetExpressionBuilder(multiplicity);
            var args = new BuildValueConditionNodeExpressionArgs
            {
                DataTypeConfiguration = dataTypeConfiguration,
                LeftOperandExpression = leftOperandValueFieldAccessExpression,
                Operator = operatorMetadata.Operator,
                ResultVariableExpression = resultVariableExpression,
                RightOperandExpression = builder.Constant(valueConditionNode.RightOperand.Value),
                TestLeftOperand = testPresentLeftOperand,
            };
            valueConditionNodeExpressionBuilder.Build(
                builder,
                args);
        }

        private void BuildBehaviorForValueConditionNode(
            IExpressionBlockBuilder builder,
            ValueConditionNode valueConditionNode,
            ParameterExpression conditionsVariableExpression,
            MatchModes matchMode)
        {
            // Variables, constants, and labels.
            var leftOperandVariableExpression = builder.GetVariable("leftOperand");
            var jumpLabelNeeded = false;

            // Line 1.
            builder.AddExpression(
                builder.Call(
                    instance: conditionsVariableExpression,
                    tryGetValueMethod,
                    [builder.Constant(valueConditionNode.Condition), leftOperandVariableExpression]));

            var leftOperandValueFieldAccessExpression = builder.AccessField(leftOperandVariableExpression, operandValueField);
            if (this.rulesEngineOptions.MissingConditionBehavior == MissingConditionBehaviors.Discard || matchMode == MatchModes.Search)
            {
                jumpLabelNeeded = true;
                BuildBehaviorWhenLeftOperandMissing(builder, leftOperandVariableExpression, leftOperandValueFieldAccessExpression, this.rulesEngineOptions.MissingConditionBehavior);
            }

            // Line 4.
            var testPresentLeftOperand = builder.AndAlso(
                builder.NotEqual(leftOperandVariableExpression, builder.Constant<object>(value: null!)),
                builder.NotEqual(leftOperandValueFieldAccessExpression, builder.Constant<object>(value: null!)));
            var operatorMetadata = OperatorsMetadata.AllByOperator[valueConditionNode.Operator];
            if (operatorMetadata.SupportedMultiplicities.Count == 1)
            {
                this.BuildBehaviorForSingleMultiplicity(builder, valueConditionNode, operatorMetadata, leftOperandValueFieldAccessExpression, testPresentLeftOperand);
            }
            else
            {
                this.BuildBehaviorForManyMultiplicities(builder, valueConditionNode, operatorMetadata, leftOperandValueFieldAccessExpression, testPresentLeftOperand);
            }

            // Line 6.
            if (jumpLabelNeeded)
            {
                builder.Label(builder.GetLabelTarget("LabelEndValueConditionNode"));
            }
        }

        private void BuildExpression(
                    IExpressionBlockBuilder builder,
            IConditionNode conditionNode,
            ParameterExpression conditionsVariableExpression,
            MatchModes matchMode)
        {
            switch (conditionNode)
            {
                case ComposedConditionNode composedConditionNode:
                    var conditionExpressions = new List<Expression>(composedConditionNode.ChildConditionNodes.Count());
                    var counter = 0;
                    foreach (var childConditionNode in composedConditionNode.ChildConditionNodes)
                    {
                        var scopeNameBuilder = new StringBuilder(builder.ScopeName);
                        _ = scopeNameBuilder.Length == 0 ? scopeNameBuilder.Append("cnd") : scopeNameBuilder.Append("InnerCnd");
                        var scopeName = scopeNameBuilder.Append(counter).ToString();
                        var blockExpression = builder.Block(scopeName, x =>
                        {
                            var childResultVariableExpression = x.CreateVariable<bool>("Result");
                            this.BuildExpression(x, childConditionNode, conditionsVariableExpression, matchMode);
                            conditionExpressions.Add(childResultVariableExpression);
                        });
                        builder.AddExpression(blockExpression);
                        counter++;
                    }

                    var conditionExpression = composedConditionNode.LogicalOperator switch
                    {
                        LogicalOperators.And => builder.AndAlso(conditionExpressions),
                        LogicalOperators.Or => builder.OrElse(conditionExpressions),
                        _ => throw new NotSupportedException($"Unsupported logical operator on composed condition node: '{conditionNode.LogicalOperator}'."),
                    };
                    builder.Assign(builder.GetVariable("Result"), conditionExpression);
                    break;

                case ValueConditionNode valueConditionNode:
                    this.BuildBehaviorForValueConditionNode(builder, valueConditionNode, conditionsVariableExpression, matchMode);
                    break;

                default:
                    throw new NotSupportedException($"Unsupported condition node: '{conditionNode.GetType().Name}'.");
            }
        }
    }
}