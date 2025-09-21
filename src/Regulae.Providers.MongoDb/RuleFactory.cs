namespace Regulae.Providers.MongoDb
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Regulae;
    using Regulae.ConditionNodes;
    using Regulae.Providers.MongoDb.DataModel;
    using Regulae.Serialization;

    internal sealed class RuleFactory : IRuleFactory
    {
        private const string RMNG0001 = "RMNG0001";
        private readonly IContentSerializationProvider contentSerializationProvider;

        public RuleFactory(IContentSerializationProvider contentSerializationProvider)
        {
            this.contentSerializationProvider = contentSerializationProvider;
        }

        public Rule CreateRule(RuleDataModel ruleDataModel)
        {
            ArgumentNullException.ThrowIfNull(ruleDataModel);

            var ruleBuilderResult = Rule.Create(ruleDataModel.Name)
                .InRuleset(ruleDataModel.Ruleset)
                .SetContent((object)ruleDataModel.Content, this.contentSerializationProvider)
                .Since(ruleDataModel.DateBegin)
                .Until(ruleDataModel.DateEnd)
                .WithActive(ruleDataModel.Active ?? true)
                .ApplyWhen(_ => ruleDataModel.RootCondition is { } ? ConvertConditionNode(ruleDataModel.RootCondition) : null)
                .Build();

            if (!ruleBuilderResult.IsSuccess)
            {
                throw new InvalidRuleException($"An invalid rule was loaded from data source. Rule Name: {ruleDataModel.Name}", ruleBuilderResult.Errors);
            }

            ruleBuilderResult.Rule.Priority = ruleDataModel.Priority;

            if (ruleBuilderResult.Rule.Priority <= 0)
            {
                throw new InvalidRuleException(
                    $"An invalid rule was loaded from data source. Rule Name: {ruleDataModel.Name}",
                    [OperationError.Create(RMNG0001, string.Create(CultureInfo.InvariantCulture, $"Loaded rule priority number is invalid: {ruleBuilderResult.Rule.Priority}."))]);
            }

            return ruleBuilderResult.Rule;
        }

        public RuleDataModel CreateRule(Rule rule)
        {
            ArgumentNullException.ThrowIfNull(rule);

            var content = rule.ContentContainer.GetContentAs<object>();
            var serializedContent = this.contentSerializationProvider.GetContentSerializer(rule.Ruleset).Serialize(content);

            var ruleDataModel = new RuleDataModel
            {
                Active = rule.Active,
                Content = serializedContent,
                DateBegin = rule.DateBegin,
                DateEnd = rule.DateEnd,
                Name = rule.Name,
                Priority = rule.Priority,
                RootCondition = rule.RootCondition is { } ? this.ConvertConditionNode(rule.RootCondition) : null,
                Ruleset = rule.Ruleset,
            };

            return ruleDataModel;
        }

        private static IConditionNode ConvertConditionNode(ConditionNodeDataModel conditionNodeDataModel)
        {
            if (conditionNodeDataModel.LogicalOperator == LogicalOperators.Eval)
            {
                return CreateValueConditionNode(conditionNodeDataModel as ValueConditionNodeDataModel);
            }

            var composedConditionNodeDataModel = conditionNodeDataModel as ComposedConditionNodeDataModel;
            var childConditionNodeDataModels = composedConditionNodeDataModel.ChildConditionNodes;
            var count = childConditionNodeDataModels.Length;
            var childConditionNodes = new IConditionNode[count];
            for (var i = 0; i < count; i++)
            {
                childConditionNodes[i] = ConvertConditionNode(childConditionNodeDataModels[i]);
            }

            var composedConditionNode = new ComposedConditionNode(
                composedConditionNodeDataModel.LogicalOperator,
                childConditionNodes);
            foreach (var property in composedConditionNodeDataModel.Properties)
            {
                composedConditionNode.Properties[property.Key] = property.Value;
            }

            return composedConditionNode;
        }

        private static ValueConditionNodeDataModel ConvertValueConditionNode(ValueConditionNode valueConditionNode)
        {
            var properties = FilterProperties(valueConditionNode.Properties);

            return new ValueConditionNodeDataModel
            {
                Condition = Convert.ToString(valueConditionNode.Condition, CultureInfo.InvariantCulture),
                LogicalOperator = LogicalOperators.Eval,
                RightOperand = new OperandDataModel
                {
                    Cardinality = valueConditionNode.RightOperand.Cardinality,
                    DataType = valueConditionNode.RightOperand.DataType,
                    Value = valueConditionNode.RightOperand.Value,
                },
                Operator = valueConditionNode.Operator,
                Properties = properties,
            };
        }

        private static ValueConditionNode CreateValueConditionNode(ValueConditionNodeDataModel conditionNodeDataModel)
        {
            var rightOperandValue = conditionNodeDataModel.RightOperand.DataType switch
            {
                DataTypes.Integer when conditionNodeDataModel.RightOperand.Cardinality == Cardinalities.One => Convert.ToInt32(conditionNodeDataModel.RightOperand.Value, CultureInfo.InvariantCulture),
                DataTypes.Decimal when conditionNodeDataModel.RightOperand.Cardinality == Cardinalities.One => Convert.ToDecimal(conditionNodeDataModel.RightOperand.Value, CultureInfo.InvariantCulture),
                DataTypes.String when conditionNodeDataModel.RightOperand.Cardinality == Cardinalities.One => Convert.ToString(conditionNodeDataModel.RightOperand.Value, CultureInfo.InvariantCulture),
                DataTypes.Boolean when conditionNodeDataModel.RightOperand.Cardinality == Cardinalities.One => Convert.ToBoolean(conditionNodeDataModel.RightOperand.Value, CultureInfo.InvariantCulture),
                DataTypes.Integer or DataTypes.Decimal or DataTypes.String or DataTypes.Boolean => conditionNodeDataModel.RightOperand.Value,
                _ => throw new NotSupportedException($"Unsupported data type: {conditionNodeDataModel.RightOperand.DataType}."),
            };
            var rightOperand = new Operand(
                rightOperandValue,
                conditionNodeDataModel.RightOperand.DataType,
                conditionNodeDataModel.RightOperand.Cardinality);

            var valueConditionNode = new ValueConditionNode(
                conditionNodeDataModel.Condition,
                conditionNodeDataModel.Operator,
                rightOperand);

            foreach (var property in conditionNodeDataModel.Properties)
            {
                valueConditionNode.Properties[property.Key] = property.Value;
            }

            return valueConditionNode;
        }

        private static Dictionary<string, object> FilterProperties(IDictionary<string, object> source)
        {
            var properties = new Dictionary<string, object>(StringComparer.Ordinal);
            foreach (var property in source)
            {
                if (property.Key.StartsWith("_compilation", StringComparison.Ordinal))
                {
                    continue;
                }

                properties[property.Key] = property.Value;
            }

            return properties;
        }

        private ComposedConditionNodeDataModel ConvertComposedConditionNode(ComposedConditionNode composedConditionNode)
        {
            var conditionNodeDataModels = new ConditionNodeDataModel[composedConditionNode.ChildConditionNodes.Count()];
            var i = 0;
            foreach (var child in composedConditionNode.ChildConditionNodes)
            {
                conditionNodeDataModels[i++] = this.ConvertConditionNode(child);
            }

            var properties = FilterProperties(composedConditionNode.Properties);

            return new ComposedConditionNodeDataModel
            {
                ChildConditionNodes = conditionNodeDataModels,
                LogicalOperator = composedConditionNode.LogicalOperator,
                Properties = properties,
            };
        }

        private ConditionNodeDataModel ConvertConditionNode(IConditionNode conditionNode)
        {
            if (conditionNode.LogicalOperator == LogicalOperators.Eval)
            {
                return ConvertValueConditionNode(conditionNode as ValueConditionNode);
            }

            return this.ConvertComposedConditionNode(conditionNode as ComposedConditionNode);
        }
    }
}