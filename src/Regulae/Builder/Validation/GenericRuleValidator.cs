namespace Regulae.Builder.Validation
{
    using FluentValidation;
    using Regulae.Generic;

    internal sealed class GenericRuleValidator<TRuleset, TCondition> : AbstractValidator<Rule<TRuleset, TCondition>>
        where TRuleset : notnull
        where TCondition : notnull
    {
        private static readonly GenericRuleValidator<TRuleset, TCondition> ruleValidator;

        private readonly GenericComposedConditionNodeValidator<TCondition> composedConditionNodeValidator;

        private readonly GenericValueConditionNodeValidator<TCondition> valueConditionNodeValidator;

        static GenericRuleValidator()
        {
            ruleValidator = new GenericRuleValidator<TRuleset, TCondition>();
        }

        private GenericRuleValidator()
        {
            this.composedConditionNodeValidator = new GenericComposedConditionNodeValidator<TCondition>();
            this.valueConditionNodeValidator = new GenericValueConditionNodeValidator<TCondition>();
            this.RuleFor(r => r.RootCondition).Custom((cn, cc) => cn.PerformValidation(new GenericConditionNodeValidationArgs<TCondition, Rule<TRuleset, TCondition>>(this.composedConditionNodeValidator, cc, this.valueConditionNodeValidator)));
        }

        public static GenericRuleValidator<TRuleset, TCondition> Instance => ruleValidator;
    }
}