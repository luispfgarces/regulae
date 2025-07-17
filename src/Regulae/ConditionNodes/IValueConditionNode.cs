namespace Regulae.ConditionNodes
{
    using Regulae;

    /// <summary>
    /// Defines the interface contract for a condition node based on a value comparison.
    /// </summary>
    public interface IValueConditionNode : IConditionNode
    {
        /// <summary>
        /// Gets the condition name.
        /// </summary>
        string Condition { get; }

        /// <summary>
        /// Gets the condition node operator.
        /// </summary>
        Operators Operator { get; }

        /// <summary>
        /// Gets the condition's right operand.
        /// </summary>
        Operand RightOperand { get; }
    }
}