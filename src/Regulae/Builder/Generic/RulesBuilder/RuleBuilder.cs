namespace Regulae.Builder.Generic.RulesBuilder
{
    using System;
    using System.Linq;
    using Regulae;
    using Regulae.Builder.Generic;
    using Regulae.Builder.RulesBuilder;
    using Regulae.Builder.Validation;
    using Regulae.Generic;
    using Regulae.Serialization;

    internal sealed class RuleBuilder<TRuleset, TCondition> :
        IRuleBuilder<TRuleset, TCondition>,
        IRuleConfigureContent<TRuleset, TCondition>,
        IRuleConfigureDateBegin<TRuleset, TCondition>,
        IRuleConfigureDateEndOptional<TRuleset, TCondition>,
        IRuleConfigureRuleset<TRuleset, TCondition>
        where TRuleset : notnull
        where TCondition : notnull
    {
        private readonly GenericRuleValidator<TRuleset, TCondition> genericRuleValidator = GenericRuleValidator<TRuleset, TCondition>.Instance;
        private readonly RuleBuilder ruleBuilder;

        public RuleBuilder(string name)
        {
            this.ruleBuilder = new RuleBuilder(name);
        }

        public IRuleBuilder<TRuleset, TCondition> ApplyWhen(IConditionNode condition)
        {
            this.ruleBuilder.ApplyWhen(condition);
            return this;
        }

        public IRuleBuilder<TRuleset, TCondition> ApplyWhen(Func<IRootConditionNodeBuilder<TCondition>, IConditionNode> conditionFunc)
        {
            var rootConditionNodeBuilder = new RootConditionNodeBuilder<TCondition>();
            var condition = conditionFunc(rootConditionNodeBuilder);
            return this.ApplyWhen(condition);
        }

        public IRuleBuilder<TRuleset, TCondition> ApplyWhen<TDataType>(
            TCondition condition, Operators condOperator, TDataType operand)
        {
            var rootConditionNodeBuilder = new RootConditionNodeBuilder<TCondition>();
            var valueCondition = rootConditionNodeBuilder.Value(condition, condOperator, operand);
            return this.ApplyWhen(valueCondition);
        }

        public RuleBuilderResult<TRuleset, TCondition> Build()
        {
            var ruleBuilderResult = this.ruleBuilder.Build();

            if (ruleBuilderResult.IsSuccess)
            {
                var genericRule = new Rule<TRuleset, TCondition>(ruleBuilderResult.Rule!);
                var validationResult = this.genericRuleValidator.Validate(genericRule);
                if (validationResult.IsValid)
                {
                    return RuleBuilderResult.Success<TRuleset, TCondition>(genericRule);
                }

                return RuleBuilderResult.Failure<TRuleset, TCondition>([.. validationResult.Errors.Select(ve => OperationError.Create(ve.ErrorCode, ve.ErrorMessage))]);
            }

            return RuleBuilderResult.Failure<TRuleset, TCondition>(ruleBuilderResult.Errors);
        }

        public IRuleConfigureContent<TRuleset, TCondition> InRuleset(TRuleset ruleset)
        {
            this.ruleBuilder.InRuleset(GenericConversions.Convert(ruleset));
            return this;
        }

        public IRuleConfigureDateBegin<TRuleset, TCondition> SetContent(object content)
        {
            this.ruleBuilder.SetContent(content);
            return this;
        }

        public IRuleConfigureDateBegin<TRuleset, TCondition> SetContent(object content, IContentSerializationProvider contentSerializationProvider)
        {
            this.ruleBuilder.SetContent(content, contentSerializationProvider);
            return this;
        }

        public IRuleConfigureDateEndOptional<TRuleset, TCondition> Since(DateTime dateBegin)
        {
            this.ruleBuilder.Since(dateBegin);
            return this;
        }

        public IRuleBuilder<TRuleset, TCondition> Until(DateTime? dateEnd)
        {
            this.ruleBuilder.Until(dateEnd);
            return this;
        }

        public IRuleBuilder<TRuleset, TCondition> WithActive(bool active)
        {
            this.ruleBuilder.WithActive(active);
            return this;
        }
    }
}