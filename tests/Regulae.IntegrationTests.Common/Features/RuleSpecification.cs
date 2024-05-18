namespace Regulae.IntegrationTests.Common.Features
{
    using Regulae;
    using Regulae.Generic;
    using Regulae.IntegrationTests.Common.Features.Stubs;

    public class RuleSpecification
    {
        public RuleSpecification(Rule<RulesetNames, ConditionNames> ruleBuilderResult, RuleAddPriorityOption ruleAddPriorityOption)
        {
            this.Rule = ruleBuilderResult;
            this.RuleAddPriorityOption = ruleAddPriorityOption;
        }

        public Rule<RulesetNames, ConditionNames> Rule { get; set; }
        public RuleAddPriorityOption RuleAddPriorityOption { get; set; }
    }
}