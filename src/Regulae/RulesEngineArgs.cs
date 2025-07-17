namespace Regulae
{
    using Regulae.Core;
    using Regulae.Evaluation;
    using Regulae.Management;
    using Regulae.Source;
    using Regulae.Validation;

    internal struct RulesEngineArgs
    {
        public IConditionsConverter ConditionsConverter { get; set; }
        public IConditionsEvalEngine ConditionsEvalEngine { get; set; }
        public IRuleConditionsExtractor RuleConditionsExtractor { get; set; }
        public IRuleSanitizer RuleSanitizer { get; set; }
        public RulesEngineOptions RulesEngineOptions { get; set; }
        public IRulesSource RulesSource { get; set; }
        public IValidatorProvider ValidatorProvider { get; set; }
    }
}