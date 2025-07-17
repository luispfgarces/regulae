namespace Regulae.Providers.InMemory
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using Regulae.Providers.InMemory.DataModel;

    internal sealed class InMemoryRulesStorage : IInMemoryRulesStorage
    {
        private readonly ConcurrentDictionary<string, ConditionDataModel> conditions;
        private readonly ConcurrentDictionary<string, RulesetDataModel> rulesets;

        public InMemoryRulesStorage()
        {
            this.conditions = new ConcurrentDictionary<string, ConditionDataModel>(StringComparer.Ordinal);
            this.rulesets = new ConcurrentDictionary<string, RulesetDataModel>(StringComparer.Ordinal);
        }

        public void AddRule(RuleDataModel ruleDataModel)
        {
            var rulesetRules = this.GetRulesCollectionByRuleset(ruleDataModel.Ruleset);

            lock (rulesetRules)
            {
                if (rulesetRules.Exists(r => string.Equals(r.Name, ruleDataModel.Name, StringComparison.Ordinal)))
                {
                    throw new InvalidOperationException($"Rule with name '{ruleDataModel.Name}' already exists.");
                }

                AddRuleInternal(rulesetRules, ruleDataModel);
            }
        }

        public void CreateCondition(string condition, DataTypes dataType)
        {
            var conditionModel = new ConditionDataModel
            {
                Creation = DateTime.UtcNow,
                DataType = dataType,
                Name = condition,
            };

            if (!this.conditions.TryAdd(condition, conditionModel))
            {
                throw new InvalidOperationException($"Condition with name '{condition}' already exists.");
            }
        }

        public void CreateRuleset(string ruleset)
        {
            _ = this.rulesets.TryAdd(ruleset, new RulesetDataModel
            {
                Creation = DateTime.UtcNow,
                Name = ruleset,
                Rules = new List<RuleDataModel>(),
            });
        }

        public IReadOnlyCollection<RuleDataModel> GetAllRules()
            => this.rulesets.SelectMany(kvp => kvp.Value.Rules).ToArray();

        public IReadOnlyDictionary<string, ConditionDataModel> GetConditions() => this.conditions;

        public IReadOnlyCollection<RuleDataModel> GetRulesBy(string ruleset)
        {
            var rules = this.GetRulesCollectionByRuleset(ruleset);

            return rules;
        }

        public IReadOnlyCollection<RulesetDataModel> GetRulesets()
            => this.rulesets.Values.ToImmutableArray();

        public void UpdateRule(RuleDataModel ruleDataModel)
        {
            var rulesetRules = this.GetRulesCollectionByRuleset(ruleDataModel.Ruleset);

            lock (rulesetRules)
            {
                var existent = rulesetRules.Find(r => string.Equals(r.Name, ruleDataModel.Name, StringComparison.Ordinal));
                if (existent is null)
                {
                    throw new InvalidOperationException($"Rule with name '{ruleDataModel.Name}' does not exist, no update can be done.");
                }

                rulesetRules.Remove(existent);
                AddRuleInternal(rulesetRules, ruleDataModel);
            }
        }

        private static void AddRuleInternal(List<RuleDataModel> rulesetRules, RuleDataModel ruleDataModel)
        {
            var i = 0;
            while (i < rulesetRules.Count && rulesetRules[i].Priority < ruleDataModel.Priority)
            {
                i++;
            }

            rulesetRules.Insert(i, ruleDataModel);
        }

        private List<RuleDataModel> GetRulesCollectionByRuleset(string ruleset)
        {
            if (this.rulesets.TryGetValue(ruleset, out var rulesetDataModel))
            {
                return rulesetDataModel.Rules;
            }

            throw new InvalidOperationException($"A ruleset with name '{ruleset}' does not exist.");
        }
    }
}