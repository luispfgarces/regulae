namespace Regulae.Generic.ConditionNodes
{
    using Regulae;
    using Regulae.Generic;

    /// <summary>
    /// Defines the interface contract for a condition node based on a value comparison.
    /// </summary>
    /// <typeparam name="TCondition">The condition type that strongly types conditions.</typeparam>
    public interface IValueConditionNode<TCondition> : IConditionNode<TCondition>
    {
        /// <summary>
        /// Gets the condition name.
        /// </summary>
        TCondition Condition { get; }

        /// <summary>
        /// Gets the condition node data type.
        /// </summary>
        DataTypes DataType { get; }

        /// <summary>
        /// Gets the condition's operand.
        /// </summary>
        object Operand { get; }

        /// <summary>
        /// Gets the condition node operator.
        /// </summary>
        Operators Operator { get; }
    }
}