namespace Regulae.WebUI.Sample.Engine
{
    using Regulae;
    using Regulae.Builder.Generic;
    using Regulae.WebUI.Sample.Enums;

    internal sealed class RuleSpecification : RuleSpecificationBase<RulesetNames, ConditionNames>
    {
        public RuleSpecification(
            RuleBuilderResult<RulesetNames, ConditionNames> ruleBuilderResult,
            RuleAddPriorityOption ruleAddPriorityOption)
            : base(ruleBuilderResult, ruleAddPriorityOption)
        {
        }
    }
}