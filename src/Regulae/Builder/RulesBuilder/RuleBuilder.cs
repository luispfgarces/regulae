namespace Regulae.Builder.RulesBuilder
{
    using System;
    using System.Linq;
    using Regulae;
    using Regulae.Builder;
    using Regulae.Builder.Validation;
    using Regulae.Serialization;

    internal sealed class RuleBuilder :
        IRuleBuilder,
        IRuleConfigureContent,
        IRuleConfigureDateBegin,
        IRuleConfigureDateEndOptional,
        IRuleConfigureRuleset
    {
        private readonly string name;
        private readonly RuleValidator ruleValidator = RuleValidator.Instance;

        private bool? active;
        private IContentContainer? contentContainer;
        private DateTime dateBegin;
        private DateTime? dateEnd;
        private IConditionNode? rootCondition;
        private string? ruleset;

        public RuleBuilder(string name)
        {
            this.name = name;
        }

        public IRuleBuilder ApplyWhen(IConditionNode condition)
        {
            this.rootCondition = condition;
            return this;
        }

        public IRuleBuilder ApplyWhen(Func<IRootConditionNodeBuilder, IConditionNode> conditionFunc)
        {
            var rootConditionNodeBuilder = new RootConditionNodeBuilder();
            var condition = conditionFunc(rootConditionNodeBuilder);
            return this.ApplyWhen(condition);
        }

        public IRuleBuilder ApplyWhen<T>(string condition, Operators condOperator, T operand)
        {
            var rootConditionNodeBuilder = new RootConditionNodeBuilder();
            var valueCondition = rootConditionNodeBuilder.Value(condition, condOperator, operand);
            return this.ApplyWhen(valueCondition);
        }

        public RuleBuilderResult Build()
        {
            var rule = new Rule(this.name, this.ruleset!, this.dateBegin, this.dateEnd, this.contentContainer!)
            {
                Active = this.active ?? true,
                RootCondition = this.rootCondition,
            };

            var validationResult = this.ruleValidator.Validate(rule);

            if (validationResult.IsValid)
            {
                return RuleOperation.Success(rule);
            }

            return RuleOperation.Failure([.. validationResult.Errors.Select(ve => OperationError.Create(ve.ErrorCode, ve.ErrorMessage))]);
        }

        public IRuleConfigureContent InRuleset(string ruleset)
        {
            this.ruleset = ruleset;
            return this;
        }

        public IRuleConfigureDateBegin SetContent(object content)
        {
            this.contentContainer = new ObjectContentContainer(content);
            return this;
        }

        public IRuleConfigureDateBegin SetContent(object content, IContentSerializationProvider contentSerializationProvider)
        {
            ArgumentNullException.ThrowIfNull(contentSerializationProvider);

            this.contentContainer = new SerializedContentContainer(this.ruleset!, content, contentSerializationProvider);
            return this;
        }

        public IRuleConfigureDateEndOptional Since(DateTime dateBegin)
        {
            this.dateBegin = dateBegin;
            return this;
        }

        public IRuleBuilder Until(DateTime? dateEnd)
        {
            this.dateEnd = dateEnd;
            return this;
        }

        public IRuleBuilder WithActive(bool active)
        {
            this.active = active;
            return this;
        }
    }
}