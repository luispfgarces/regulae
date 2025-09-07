namespace Regulae.Builder.Validation
{
    using FluentValidation;
    using Regulae.Generic.ConditionNodes;

    internal sealed class GenericComposedConditionNodeValidator<TCondition> : AbstractValidator<ComposedConditionNode<TCondition>>
        where TCondition : notnull
    {
        private readonly GenericValueConditionNodeValidator<TCondition> valueConditionNodeValidator;

        public GenericComposedConditionNodeValidator()
        {
            this.valueConditionNodeValidator = new GenericValueConditionNodeValidator<TCondition>();

            this.RuleForEach(c => c.ChildConditionNodes)
                .NotNull()
                .Custom((cn, cc) => cn.PerformValidation(new GenericConditionNodeValidationArgs<TCondition, ComposedConditionNode<TCondition>>(this, cc, this.valueConditionNodeValidator)));
        }
    }
}