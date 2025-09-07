namespace Regulae.Generic
{
    using Regulae;
    using Regulae.ConditionNodes;
    using Regulae.Generic.ConditionNodes;

    internal static class RuleExtensions
    {
        public static IConditionNode<TCondition> ToGenericConditionNode<TCondition>(this IConditionNode rootCondition)
            where TCondition : notnull
        {
            if (rootCondition.LogicalOperator == LogicalOperators.Eval)
            {
                var condition = (ValueConditionNode)rootCondition;

                return new ValueConditionNode<TCondition>(condition);
            }

            var composedConditionNode = (ComposedConditionNode)rootCondition;
            return new ComposedConditionNode<TCondition>(composedConditionNode);
        }

        public static Rule<TRuleset, TCondition> ToGenericRule<TRuleset, TCondition>(this Rule rule)
            where TRuleset : notnull
            where TCondition : notnull
            => new(rule);
    }
}