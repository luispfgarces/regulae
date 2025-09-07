namespace Regulae.WebUI.Sample.Engine
{
    using Regulae;
    using Regulae.Builder.Generic;

    internal class RuleSpecificationBase<TRuleset, TCondition>
        where TRuleset : notnull
        where TCondition : notnull
    {
        public RuleSpecificationBase(
            RuleBuilderResult<TRuleset, TCondition> ruleBuilderResult,
            RuleAddPriorityOption ruleAddPriorityOption)
        {
            this.RuleBuilderResult = ruleBuilderResult;
            this.RuleAddPriorityOption = ruleAddPriorityOption;
        }

        public RuleAddPriorityOption RuleAddPriorityOption { get; set; }

        public RuleBuilderResult<TRuleset, TCondition> RuleBuilderResult { get; set; }
    }
}