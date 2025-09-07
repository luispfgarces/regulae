namespace Regulae.ConditionNodes
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Regulae;
    using Regulae.Core;

    /// <summary>
    /// A generic implementation for a valued condition node.
    /// </summary>
    /// <seealso cref="IValueConditionNode"/>
    [DebuggerDisplay("{RightOperand.DataType.ToString(),nq} condition: <{Condition,nq}> {Operator} {RightOperand.Value}")]
    public class ValueConditionNode : IValueConditionNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValueConditionNode"/> class.
        /// </summary>
        /// <param name="condition">The condition name.</param>
        /// <param name="operator">The operator.</param>
        /// <param name="rightOperand">The right operand.</param>
        public ValueConditionNode(string condition, Operators @operator, Operand rightOperand)
            : this(condition, @operator, rightOperand, new PropertiesDictionary(Constants.DefaultPropertiesDictionarySize))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueConditionNode"/> class.
        /// </summary>
        /// <param name="condition">The condition name.</param>
        /// <param name="operator">The operator.</param>
        /// <param name="rightOperand">The right operand.</param>
        /// <param name="properties">The properties.</param>
        public ValueConditionNode(string condition, Operators @operator, Operand rightOperand, IDictionary<string, object> properties)
        {
            this.Condition = condition;
            this.RightOperand = rightOperand;
            this.Operator = @operator;
            this.Properties = properties;
        }

        /// <inheritdoc/>
        public string Condition { get; }

        /// <inheritdoc/>
        public LogicalOperators LogicalOperator => LogicalOperators.Eval;

        /// <inheritdoc/>
        public Operators Operator { get; }

        /// <inheritdoc/>
        public IDictionary<string, object> Properties { get; }

        /// <inheritdoc/>
        public Operand RightOperand { get; internal set; }

        /// <inheritdoc/>
        public IConditionNode Clone()
            => new ValueConditionNode(
                this.Condition,
                this.Operator,
                this.RightOperand,
                new PropertiesDictionary(this.Properties));

        /// <summary>
        /// Determines whether the specified <see cref="object"/>, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object? obj) => obj is ValueConditionNode node && StringComparer.Ordinal.Equals(this.Condition, node.Condition) && this.LogicalOperator == node.LogicalOperator && EqualityComparer<object>.Default.Equals(this.RightOperand, node.RightOperand) && this.Operator == node.Operator && EqualityComparer<IDictionary<string, object>>.Default.Equals(this.Properties, node.Properties);

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data
        /// structures like a hash table.
        /// </returns>
        public override int GetHashCode()
            => HashCode.Combine(this.Condition, this.LogicalOperator, this.RightOperand, this.Operator, this.Properties);
    }
}