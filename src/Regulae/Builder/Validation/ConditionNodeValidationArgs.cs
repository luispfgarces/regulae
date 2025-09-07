namespace Regulae.Builder.Validation
{
    using FluentValidation;

    internal sealed class ConditionNodeValidationArgs<TValidationContext>
    {
        public ConditionNodeValidationArgs(
            ComposedConditionNodeValidator composedConditionNodeValidator,
            ValidationContext<TValidationContext> validationContext,
            ValueConditionNodeValidator valueConditionNodeValidator)
        {
            this.ComposedConditionNodeValidator = composedConditionNodeValidator;
            this.ValidationContext = validationContext;
            this.ValueConditionNodeValidator = valueConditionNodeValidator;
        }

        public ComposedConditionNodeValidator ComposedConditionNodeValidator { get; set; }
        public ValidationContext<TValidationContext> ValidationContext { get; set; }
        public ValueConditionNodeValidator ValueConditionNodeValidator { get; set; }
    }
}