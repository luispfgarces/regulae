namespace Regulae.Evaluation.Compiled
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Regulae;
    using Regulae.Evaluation;

    internal struct BuildValueConditionNodeExpressionArgs : IEquatable<BuildValueConditionNodeExpressionArgs>
    {
        public DataTypeConfiguration DataTypeConfiguration { get; set; }

        public Expression LeftOperandExpression { get; set; }

        public Operators Operator { get; set; }

        public ParameterExpression ResultVariableExpression { get; set; }

        public Expression RightOperandExpression { get; set; }

        public Expression TestLeftOperand { get; set; }

        public readonly bool Equals(BuildValueConditionNodeExpressionArgs other)
            => EqualityComparer<DataTypeConfiguration>.Default.Equals(this.DataTypeConfiguration, other.DataTypeConfiguration)
                && EqualityComparer<Expression>.Default.Equals(this.LeftOperandExpression, other.LeftOperandExpression)
                && EqualityComparer<Operators>.Default.Equals(this.Operator, other.Operator)
                && EqualityComparer<ParameterExpression>.Default.Equals(this.ResultVariableExpression, other.ResultVariableExpression)
                && EqualityComparer<Expression>.Default.Equals(this.RightOperandExpression, other.RightOperandExpression)
                && EqualityComparer<Expression>.Default.Equals(this.TestLeftOperand, other.TestLeftOperand);

        public override readonly bool Equals(object obj)
            => obj is BuildValueConditionNodeExpressionArgs args && this.Equals(args);

        public override readonly int GetHashCode()
            => HashCode.Combine(this.DataTypeConfiguration, this.LeftOperandExpression, this.Operator, this.ResultVariableExpression, this.RightOperandExpression, this.TestLeftOperand);
    }
}