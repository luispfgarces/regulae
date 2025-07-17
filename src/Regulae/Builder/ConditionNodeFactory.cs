namespace Regulae.Builder
{
    using System;
    using Regulae;
    using Regulae.Builder.Generic.RulesBuilder;
    using Regulae.Builder.RulesBuilder;
    using Regulae.ConditionNodes;

    /// <summary>
    /// Factory for creating condition nodes.
    /// </summary>
    public static class ConditionNodeFactory
    {
        /// <summary>
        /// Creates a composed condition node.
        /// </summary>
        /// <param name="logicalOperator">The logical operator.</param>
        /// <param name="conditionFunc">
        /// The function containing the logic for the composed condition node.
        /// </param>
        /// <returns></returns>
        public static IConditionNode CreateComposedNode(
            LogicalOperators logicalOperator,
            Func<IFluentConditionNodeBuilder, IFluentConditionNodeBuilder> conditionFunc)
        {
            var composedConditionNodeBuilder = new FluentConditionNodeBuilder(logicalOperator);

            var composedConditionNode = conditionFunc(composedConditionNodeBuilder)
                .Build();

            return composedConditionNode;
        }

        /// <summary>
        /// Creates a composed condition node.
        /// </summary>
        /// <typeparam name="TCondition">The condition type that strongly types conditions.</typeparam>
        /// <param name="logicalOperator">The logical operator.</param>
        /// <param name="conditionFunc">
        /// The function containing the logic for the composed condition node.
        /// </param>
        /// <returns></returns>
        public static IConditionNode CreateComposedNode<TCondition>(
            LogicalOperators logicalOperator,
            Func<IFluentConditionNodeBuilder<TCondition>, IFluentConditionNodeBuilder<TCondition>> conditionFunc)
        {
            var composedConditionNodeBuilder = new FluentConditionNodeBuilder<TCondition>(logicalOperator);

            var composedConditionNode = conditionFunc(composedConditionNodeBuilder)
                .Build();

            return composedConditionNode;
        }

        /// <summary>
        /// Creates a value condition node.
        /// </summary>
        /// <typeparam name="T">the operand type.</typeparam>
        /// <param name="condition">The condition name.</param>
        /// <param name="condOperator">The condition operator.</param>
        /// <param name="operand">The condition operand.</param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">The data type is not supported: {typeof(T).FullName}.</exception>
        public static IValueConditionNode CreateValueNode<T>(
            string condition, Operators condOperator, T operand)
        {
            var rightOperand = new Operand(operand);
            return new ValueConditionNode(condition, condOperator, rightOperand);
        }
    }
}