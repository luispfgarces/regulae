namespace Regulae.InMemory.Sample.Engine
{
    using Regulae;
    using Regulae.Builder.Generic;
    using Regulae.InMemory.Sample.Enums;

    internal class RuleSpecification
    {
        public RuleAddPriorityOption RuleAddPriorityOption { get; set; }

        public RuleBuilderResult<RulesetNames, ConditionNames> RuleBuilderResult { get; set; }
    }
}