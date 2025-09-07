namespace Regulae.Builder.Validation
{
    using FluentValidation;

    internal sealed class GenericConditionNodeValidationArgs<TCondition, TValidationContext>
        where TCondition : notnull
    {
        public GenericConditionNodeValidationArgs(
            GenericComposedConditionNodeValidator<TCondition> composedConditionNodeValidator,
            ValidationContext<TValidationContext> validationContext,
            GenericValueConditionNodeValidator<TCondition> valueConditionNodeValidator)
        {
            this.ComposedConditionNodeValidator = composedConditionNodeValidator;
            this.ValidationContext = validationContext;
            this.ValueConditionNodeValidator = valueConditionNodeValidator;
        }

        public GenericComposedConditionNodeValidator<TCondition> ComposedConditionNodeValidator { get; set; }
        public ValidationContext<TValidationContext> ValidationContext { get; set; }
        public GenericValueConditionNodeValidator<TCondition> ValueConditionNodeValidator { get; set; }
    }
}