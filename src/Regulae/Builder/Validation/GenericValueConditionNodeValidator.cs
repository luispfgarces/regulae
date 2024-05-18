namespace Regulae.Builder.Validation
{
    using FluentValidation;
    using Regulae.Generic.ConditionNodes;

    internal sealed class GenericValueConditionNodeValidator<TCondition> : AbstractValidator<ValueConditionNode<TCondition>>
    {
        public GenericValueConditionNodeValidator()
        {
            this.RuleFor(c => c.Condition)
                .NotEmpty()
                .IsInEnum()
                .When(c => c.Condition!.GetType().IsEnum);
        }
    }
}