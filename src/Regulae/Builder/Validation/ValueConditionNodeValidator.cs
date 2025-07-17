namespace Regulae.Builder.Validation
{
    using FluentValidation;
    using Regulae;
    using Regulae.ConditionNodes;

    internal sealed class ValueConditionNodeValidator : AbstractValidator<ValueConditionNode>
    {
        public ValueConditionNodeValidator()
        {
            this.RuleFor(c => c.Condition)
                .NotEmpty();

            this.RuleFor(c => c.Operator)
                .IsInEnum();

            this.RuleFor(c => c.Operator)
                .IsContainedOn(Operators.Equal, Operators.NotEqual, Operators.Contains, Operators.NotContains, Operators.StartsWith, Operators.EndsWith, Operators.CaseInsensitiveStartsWith, Operators.CaseInsensitiveEndsWith, Operators.NotStartsWith, Operators.NotEndsWith)
                .When(c => c.RightOperand.DataType == DataTypes.String && c.RightOperand.Cardinality == Cardinalities.One)
                .WithMessage(cn => $"Condition nodes with data type '{cn.RightOperand.DataType}' can't define a operator of type '{cn.Operator}'.");

            this.RuleFor(c => c.Operator)
                .IsContainedOn(Operators.Equal, Operators.NotEqual)
                .When(c => c.RightOperand.DataType == DataTypes.Boolean && c.RightOperand.Cardinality == Cardinalities.One)
                .WithMessage(cn => $"Condition nodes with data type '{cn.RightOperand.DataType}' can't define a operator of type '{cn.Operator}'.");

            this.RuleFor(c => c.Operator)
                .IsContainedOn(Operators.Equal, Operators.NotEqual, Operators.GreaterThan, Operators.GreaterThanOrEqual, Operators.LesserThan, Operators.LesserThanOrEqual)
                .When(c => c.RightOperand.DataType == DataTypes.Integer && c.RightOperand.Cardinality == Cardinalities.One)
                .WithMessage(cn => $"Condition nodes with data type '{cn.RightOperand.DataType}' can't define a operator of type '{cn.Operator}'.");

            this.RuleFor(c => c.Operator)
                .IsContainedOn(Operators.Equal, Operators.NotEqual, Operators.GreaterThan, Operators.GreaterThanOrEqual, Operators.LesserThan, Operators.LesserThanOrEqual)
                .When(c => c.RightOperand.DataType == DataTypes.Decimal && c.RightOperand.Cardinality == Cardinalities.One)
                .WithMessage(cn => $"Condition nodes with a right operand of data type '{cn.RightOperand.DataType}' can't define a operator of type '{cn.Operator}'.");

            this.RuleFor(c => c.Operator)
                .IsContainedOn(Operators.In, Operators.NotIn, Operators.Equal, Operators.NotEqual)
                .When(c => c.RightOperand.DataType == DataTypes.String && c.RightOperand.Cardinality == Cardinalities.Many)
                .WithMessage(cn => $"Condition nodes with a right operand of data type '{cn.RightOperand.DataType}' and cardinality '{cn.RightOperand.Cardinality}' can't define a operator of type '{cn.Operator}'.");

            this.RuleFor(c => c.Operator)
                .IsContainedOn(Operators.In, Operators.NotIn, Operators.Equal, Operators.NotEqual)
                .When(c => c.RightOperand.DataType == DataTypes.Integer && c.RightOperand.Cardinality == Cardinalities.Many)
                .WithMessage(cn => $"Condition nodes with a right operand of data type '{cn.RightOperand.DataType}' and cardinality '{cn.RightOperand.Cardinality}' can't define a operator of type '{cn.Operator}'.");

            this.RuleFor(c => c.Operator)
                .IsContainedOn(Operators.In, Operators.NotIn, Operators.Equal, Operators.NotEqual)
                .When(c => c.RightOperand.DataType == DataTypes.Decimal && c.RightOperand.Cardinality == Cardinalities.Many)
                .WithMessage(cn => $"Condition nodes with a right operand of data type '{cn.RightOperand.DataType}' and cardinality '{cn.RightOperand.Cardinality}' can't define a operator of type '{cn.Operator}'.");

            this.RuleFor(c => c.Operator)
                .IsContainedOn(Operators.In, Operators.NotIn, Operators.Equal, Operators.NotEqual)
                .When(c => c.RightOperand.DataType == DataTypes.Boolean && c.RightOperand.Cardinality == Cardinalities.Many)
                .WithMessage(cn => $"Condition nodes with a right operand of data type '{cn.RightOperand.DataType}' and cardinality '{cn.RightOperand.Cardinality}' can't define a operator of type '{cn.Operator}'.");
        }
    }
}