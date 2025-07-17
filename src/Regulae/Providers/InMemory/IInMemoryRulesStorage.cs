namespace Regulae.Providers.InMemory
{
    using System.Collections.Generic;
    using Regulae.Providers.InMemory.DataModel;

    internal interface IInMemoryRulesStorage
    {
        void AddRule(RuleDataModel ruleDataModel);

        void CreateCondition(string name, DataTypes dataType);

        void CreateRuleset(string name);

        IReadOnlyCollection<RuleDataModel> GetAllRules();

        IReadOnlyDictionary<string, ConditionDataModel> GetConditions();

        IReadOnlyCollection<RuleDataModel> GetRulesBy(string ruleset);

        IReadOnlyCollection<RulesetDataModel> GetRulesets();

        void UpdateRule(RuleDataModel ruleDataModel);
    }
}