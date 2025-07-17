namespace Regulae.Providers.InMemory
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Regulae;

    /// <summary>
    /// The rules data source implementation for usage backed with a in-memory database.
    /// </summary>
    /// <seealso cref="IRulesDataSource"/>
    public class InMemoryProviderRulesDataSource : IRulesDataSource
    {
        private readonly IInMemoryRulesStorage inMemoryRulesStorage;
        private readonly IRuleFactory ruleFactory;

        internal InMemoryProviderRulesDataSource(
            IInMemoryRulesStorage inMemoryRulesStorage,
            IRuleFactory ruleFactory)
        {
            this.inMemoryRulesStorage = inMemoryRulesStorage ?? throw new ArgumentNullException(nameof(inMemoryRulesStorage));
            this.ruleFactory = ruleFactory ?? throw new ArgumentNullException(nameof(ruleFactory));
        }

        /// <summary>
        /// Adds a new rule to data source.
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">rule</exception>
        public ValueTask AddRuleAsync(Rule rule)
        {
            if (rule is null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            var ruleDataModel = this.ruleFactory.CreateRule(rule);

            this.inMemoryRulesStorage.AddRule(ruleDataModel);

            return new ValueTask();
        }

        /// <inheritdoc/>
        public ValueTask CreateConditionAsync(string name, DataTypes dataType)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            this.inMemoryRulesStorage.CreateCondition(name, dataType);

            return new ValueTask();
        }

        /// <summary>
        /// Creates a new ruleset on the data source.
        /// </summary>
        /// <param name="name">the ruleset name.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">ruleset</exception>
        /// <exception cref="InvalidOperationException">The ruleset '{ruleset}' already exists.</exception>
        public ValueTask CreateRulesetAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            var rulesets = this.inMemoryRulesStorage.GetRulesets();

            if (rulesets.Any(rs => string.Equals(rs.Name, name, StringComparison.Ordinal)))
            {
                throw new InvalidOperationException($"The ruleset '{name}' already exists.");
            }

            this.inMemoryRulesStorage.CreateRuleset(name);

            return new ValueTask();
        }

        /// <inheritdoc/>
        public ValueTask<IReadOnlyDictionary<string, Condition>> GetConditionsAsync()
        {
            var conditionDataModels = this.inMemoryRulesStorage.GetConditions();
            var conditions = new Dictionary<string, Condition>(conditionDataModels.Count, StringComparer.Ordinal);
            foreach (var keyValuePair in conditionDataModels)
            {
                var conditionDataModel = keyValuePair.Value;
                conditions.Add(keyValuePair.Key, new Condition(conditionDataModel.Name, conditionDataModel.Creation, conditionDataModel.DataType));
            }

            return new ValueTask<IReadOnlyDictionary<string, Condition>>(conditions);
        }

        /// <summary>
        /// Gets the rules categorized with specified <paramref name="ruleset"/> between <paramref
        /// name="dateBegin"/> and <paramref name="dateEnd"/>.
        /// </summary>
        /// <param name="ruleset">the ruleset.</param>
        /// <param name="dateBegin">the filtering begin date.</param>
        /// <param name="dateEnd">the filtering end date.</param>
        /// <returns></returns>
        public ValueTask<IReadOnlyCollection<Rule>> GetRulesAsync(string ruleset, DateTime dateBegin, DateTime dateEnd)
        {
            var filteredByRuleset = this.inMemoryRulesStorage.GetRulesBy(ruleset);

            var filteredRules = new List<Rule>(filteredByRuleset.Count);
            foreach (var ruleDataModel in filteredByRuleset)
            {
                if (ruleDataModel.DateBegin <= dateEnd && (ruleDataModel.DateEnd is null || ruleDataModel.DateEnd > dateBegin))
                {
                    filteredRules.Add(this.ruleFactory.CreateRule(ruleDataModel));
                }
            }

            return new ValueTask<IReadOnlyCollection<Rule>>(filteredRules);
        }

        /// <summary>
        /// Gets the rules filtered by specified arguments.
        /// </summary>
        /// <param name="rulesFilterArgs">The rules filter arguments.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">rulesFilterArgs</exception>
        public ValueTask<IReadOnlyCollection<Rule>> GetRulesByAsync(RulesFilterArgs rulesFilterArgs)
        {
            if (rulesFilterArgs is null)
            {
                throw new ArgumentNullException(nameof(rulesFilterArgs));
            }

            var ruleDataModels = this.inMemoryRulesStorage.GetAllRules();

            var filteredRules = new Rule[ruleDataModels.Count];
            var i = 0;
            foreach (var ruleDataModel in ruleDataModels)
            {
                if (!Equals(rulesFilterArgs.Ruleset, default(string))
                    && !Equals(ruleDataModel.Ruleset, rulesFilterArgs.Ruleset))
                {
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(rulesFilterArgs.Name)
                    && !string.Equals(ruleDataModel.Name, rulesFilterArgs.Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                if (rulesFilterArgs.Priority.HasValue
                    && ruleDataModel.Priority == rulesFilterArgs.Priority)
                {
                    continue;
                }

                var rule = this.ruleFactory.CreateRule(ruleDataModel);
                filteredRules[i++] = rule;
            }

            if (filteredRules.Length > i)
            {
                Array.Resize(ref filteredRules, i);
            }

            return new ValueTask<IReadOnlyCollection<Rule>>(filteredRules);
        }

        /// <summary>
        /// Gets the rulesets from the data source.
        /// </summary>
        /// <returns></returns>
        public ValueTask<IReadOnlyDictionary<string, Ruleset>> GetRulesetsAsync()
        {
            var rulesetDataModels = this.inMemoryRulesStorage.GetRulesets();
            var rulesets = new Dictionary<string, Ruleset>(rulesetDataModels.Count, StringComparer.Ordinal);
            foreach (var rulesetDataModel in rulesetDataModels)
            {
                rulesets.Add(rulesetDataModel.Name, new Ruleset(rulesetDataModel.Name, rulesetDataModel.Creation));
            }

            return new ValueTask<IReadOnlyDictionary<string, Ruleset>>(rulesets);
        }

        /// <summary>
        /// Updates the existent rule on data source.
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">rule</exception>
        public ValueTask UpdateRuleAsync(Rule rule)
        {
            if (rule is null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            var newRuleDataModel = this.ruleFactory.CreateRule(rule);

            this.inMemoryRulesStorage.UpdateRule(newRuleDataModel);

            return new ValueTask();
        }
    }
}