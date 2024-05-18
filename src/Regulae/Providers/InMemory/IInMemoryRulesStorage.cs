namespace Regulae.Providers.InMemory
{
    using System.Collections.Generic;
    using Regulae.Providers.InMemory.DataModel;

    internal interface IInMemoryRulesStorage
    {
        void AddRule(RuleDataModel ruleDataModel);

        void CreateRuleset(string ruleset);

        IReadOnlyCollection<RuleDataModel> GetAllRules();

        IReadOnlyCollection<RuleDataModel> GetRulesBy(string ruleset);

        IReadOnlyCollection<RulesetDataModel> GetRulesets();

        void UpdateRule(RuleDataModel ruleDataModel);
    }
}