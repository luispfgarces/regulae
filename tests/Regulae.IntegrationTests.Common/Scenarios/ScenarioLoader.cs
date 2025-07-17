namespace Regulae.IntegrationTests.Common.Scenarios
{
    using Regulae;

    public static class ScenarioLoader
    {
        public static async Task LoadScenarioAsync<TRuleset, TCondition>(
            IRulesEngine rulesEngine,
            IScenarioData<TRuleset, TCondition> scenarioData)
        {
            foreach (var condition in scenarioData.AllConditions)
            {
                await rulesEngine.CreateConditionAsync(condition.Item1!.ToString(), condition.Item2);
            }

            foreach (var ruleset in scenarioData.AllRulesets)
            {
                await rulesEngine.CreateRulesetAsync(ruleset!.ToString());
            }

            foreach (var rule in scenarioData.AllRules)
            {
                await rulesEngine.AddRuleAsync(rule, RuleAddPriorityOption.AtSmallestNumber);
            }
        }
    }
}