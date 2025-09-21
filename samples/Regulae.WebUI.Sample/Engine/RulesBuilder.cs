namespace Regulae.WebUI.Sample.Engine
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Regulae;
    using Regulae.WebUI.Sample.Exceptions;

    internal class RulesBuilder
    {
        private readonly IEnumerable<IRuleSpecificationsProvider> ruleSpecificationsProviders;

        public RulesBuilder(IEnumerable<IRuleSpecificationsProvider> ruleSpecificationsProviders) => this.ruleSpecificationsProviders = ruleSpecificationsProviders;

        public async Task BuildAsync(IRulesEngine rulesEngine)
        {
            foreach (var ruleSpecificationsProvider in this.ruleSpecificationsProviders)
            {
                foreach (var condition in ruleSpecificationsProvider.Conditions)
                {
                    var conditionOperationResult = await rulesEngine.CreateConditionAsync(
                        condition.Condition.ToString(),
                        condition.DataType);

                    if (!conditionOperationResult.IsSuccess)
                    {
                        throw new RulesBuilderException("Rules builder error creating conditions", conditionOperationResult.Errors.Select(e => e.Message));
                    }
                }

                foreach (var ruleset in ruleSpecificationsProvider.Rulesets)
                {
                    await rulesEngine.CreateRulesetAsync(ruleset.ToString());
                }

                var rulesSpecifications = ruleSpecificationsProvider.GetRulesSpecifications();

                foreach (var ruleSpecification in rulesSpecifications)
                {
                    if (!ruleSpecification.RuleBuilderResult.IsSuccess)
                    {
                        throw new RulesBuilderException("Rules builder error getting rules specifications", ruleSpecification.RuleBuilderResult.Errors.Select(e => e.Message));
                    }

                    var ruleOperationResult = await rulesEngine
                        .AddRuleAsync(
                            ruleSpecification.RuleBuilderResult.Rule!,
                            ruleSpecification.RuleAddPriorityOption);

                    if (!ruleOperationResult.IsSuccess)
                    {
                        throw new RulesBuilderException("Rules builder error adding rules to engine", ruleOperationResult.Errors.Select(e => e.Message));
                    }
                }
            }
        }
    }
}