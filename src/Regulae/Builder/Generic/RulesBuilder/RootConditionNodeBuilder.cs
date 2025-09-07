namespace Regulae.Builder.Generic.RulesBuilder
{
    using System;
    using Regulae;
    using Regulae.Builder;
    using Regulae.Generic;

    internal sealed class RootConditionNodeBuilder<TCondition> : IRootConditionNodeBuilder<TCondition>
        where TCondition : notnull
    {
        public IConditionNode And(
            Func<IFluentConditionNodeBuilder<TCondition>, IFluentConditionNodeBuilder<TCondition>> conditionFunc)
        {
            return ConditionNodeFactory.CreateComposedNode(LogicalOperators.And, conditionFunc);
        }

        public IConditionNode Condition(IConditionNode conditionNode)
        {
            return conditionNode;
        }

        public IConditionNode Or(
            Func<IFluentConditionNodeBuilder<TCondition>, IFluentConditionNodeBuilder<TCondition>> conditionFunc)
        {
            return ConditionNodeFactory.CreateComposedNode(LogicalOperators.Or, conditionFunc);
        }

        public IConditionNode Value<T>(TCondition condition, Operators condOperator, T operand)
        {
            var conditionAsString = GenericConversions.Convert(condition);
            return ConditionNodeFactory.CreateValueNode(conditionAsString, condOperator, operand);
        }
    }
}