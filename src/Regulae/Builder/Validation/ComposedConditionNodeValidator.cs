namespace Regulae.Builder.Validation
{
    using System.Linq;
    using FluentValidation;
    using Regulae;
    using Regulae.ConditionNodes;

    internal sealed class ComposedConditionNodeValidator : AbstractValidator<ComposedConditionNode>
    {
        private readonly ValueConditionNodeValidator valueConditionNodeValidator;

        public ComposedConditionNodeValidator()
        {
            this.valueConditionNodeValidator = new ValueConditionNodeValidator();

            this.RuleFor(c => c.LogicalOperator).IsContainedOn(LogicalOperators.And, LogicalOperators.Or, LogicalOperators.Xor).WithErrorCode(Constants.ErrorCodes.R0009);
            this.RuleFor(c => c.ChildConditionNodes).Must(c => c.Skip(1).Any()).WithErrorCode(Constants.ErrorCodes.R0010);
            this.RuleForEach(c => c.ChildConditionNodes).NotNull().WithErrorCode(Constants.ErrorCodes.R0011);
            this.RuleForEach(c => c.ChildConditionNodes).Custom((cn, cc) => cn.PerformValidation(new ConditionNodeValidationArgs<ComposedConditionNode>(this, cc, this.valueConditionNodeValidator)));
        }
    }
}