namespace Regulae.Providers.InMemory
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Regulae;
    using Regulae.ConditionNodes;
    using Regulae.Core;
    using Regulae.Providers.InMemory.DataModel;

    internal sealed class RuleFactory : IRuleFactory
    {
        public Rule CreateRule(RuleDataModel ruleDataModel)
        {
            ArgumentNullException.ThrowIfNull(ruleDataModel);

            var contentContainer = new ObjectContentContainer(ruleDataModel.Content);

            return new Rule(ruleDataModel.Name, ruleDataModel.Ruleset, ruleDataModel.DateBegin, ruleDataModel.DateEnd, contentContainer)
            {
                Active = ruleDataModel.Active,
                Priority = ruleDataModel.Priority,
                RootCondition = ruleDataModel.RootCondition is { } ? ConvertConditionNode(ruleDataModel.RootCondition) : null!,
            };
        }

        public RuleDataModel CreateRule(Rule rule)
        {
            ArgumentNullException.ThrowIfNull(rule);

            var content = rule.ContentContainer.GetContentAs<object>();

            var ruleDataModel = new RuleDataModel
            {
                Content = content,
                Ruleset = rule.Ruleset,
                DateBegin = rule.DateBegin,
                DateEnd = rule.DateEnd,
                Name = rule.Name,
                Priority = rule.Priority,
                Active = rule.Active,
                RootCondition = rule.RootCondition is { } ? ConvertConditionNode(rule.RootCondition) : null!,
            };

            return ruleDataModel;
        }

        private static IConditionNode ConvertConditionNode(ConditionNodeDataModel conditionNodeDataModel)
        {
            if (conditionNodeDataModel.LogicalOperator == LogicalOperators.Eval)
            {
                return CreateValueConditionNode((ValueConditionNodeDataModel)conditionNodeDataModel);
            }

            var composedConditionNodeDataModel = (ComposedConditionNodeDataModel)conditionNodeDataModel;
            var childConditionNodeDataModels = composedConditionNodeDataModel.ChildConditionNodes;
            var count = childConditionNodeDataModels.Length;
            var childConditionNodes = new IConditionNode[count];

            for (var i = 0; i < count; i++)
            {
                childConditionNodes[i] = ConvertConditionNode(childConditionNodeDataModels[i]);
            }

            return new ComposedConditionNode(
                composedConditionNodeDataModel.LogicalOperator,
                childConditionNodes,
                new PropertiesDictionary(conditionNodeDataModel.Properties));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ValueConditionNodeDataModel ConvertValueConditionNode(ValueConditionNode valueConditionNode)
        {
            return new ValueConditionNodeDataModel
            {
                Condition = valueConditionNode.Condition,
                LogicalOperator = LogicalOperators.Eval,
                RightOperand = valueConditionNode.RightOperand,
                Operator = valueConditionNode.Operator,
                Properties = new PropertiesDictionary(valueConditionNode.Properties),
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ValueConditionNode CreateValueConditionNode(ValueConditionNodeDataModel conditionNodeDataModel)
        {
            return new ValueConditionNode(
                conditionNodeDataModel.Condition!,
                conditionNodeDataModel.Operator,
                conditionNodeDataModel.RightOperand!,
                new PropertiesDictionary(conditionNodeDataModel.Properties));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ComposedConditionNodeDataModel ConvertComposedConditionNode(ComposedConditionNode composedConditionNode)
        {
            var conditionNodeDataModels = new ConditionNodeDataModel[composedConditionNode.ChildConditionNodes.Count()];
            var i = 0;

            foreach (var child in composedConditionNode.ChildConditionNodes)
            {
                conditionNodeDataModels[i++] = ConvertConditionNode(child);
            }

            return new ComposedConditionNodeDataModel
            {
                ChildConditionNodes = conditionNodeDataModels,
                LogicalOperator = composedConditionNode.LogicalOperator,
                Properties = new PropertiesDictionary(composedConditionNode.Properties),
            };
        }

        private ConditionNodeDataModel ConvertConditionNode(IConditionNode conditionNode)
        {
            if (conditionNode.LogicalOperator == LogicalOperators.Eval)
            {
                return ConvertValueConditionNode((ValueConditionNode)conditionNode);
            }

            return ConvertComposedConditionNode((ComposedConditionNode)conditionNode);
        }
    }
}