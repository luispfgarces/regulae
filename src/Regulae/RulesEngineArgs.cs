namespace Regulae
{
    using Regulae.Core;
    using Regulae.Evaluation;
    using Regulae.Management;
    using Regulae.Source;
    using Regulae.Validation;

    internal struct RulesEngineArgs
    {
        public IAddRuleController AddRuleController { get; init; }
        public IConditionsConverter ConditionsConverter { get; init; }
        public IConditionsEvalEngine ConditionsEvalEngine { get; init; }
        public IRuleConditionsExtractor RuleConditionsExtractor { get; init; }
        public RulesEngineOptions RulesEngineOptions { get; init; }
        public IRulesSource RulesSource { get; init; }
        public IValidatorProvider ValidatorProvider { get; init; }
        public IUpdateRuleController UpdateRuleController { get; init; }
    }
}