namespace Regulae
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Exposes the interface contract for a rules data source.
    /// </summary>
    public interface IRulesDataSource
    {
        /// <summary>
        /// Adds a new rule to data source.
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <returns></returns>
        ValueTask AddRuleAsync(Rule rule);

        /// <summary>
        /// Creates a new condition on the data source.
        /// </summary>
        /// <param name="name">The condition.</param>
        /// <param name="dataType">Type of the data.</param>
        /// <returns></returns>
        ValueTask CreateConditionAsync(string name, DataTypes dataType);

        /// <summary>
        /// Creates a new ruleset on the data source.
        /// </summary>
        /// <param name="name">the ruleset name.</param>
        /// <returns></returns>
        ValueTask CreateRulesetAsync(string name);

        /// <summary>
        /// Gets the conditions from the data source.
        /// </summary>
        /// <returns></returns>
        ValueTask<IReadOnlyDictionary<string, Condition>> GetConditionsAsync();

        /// <summary>
        /// Gets the rules categorized with specified <paramref name="ruleset"/> between <paramref
        /// name="dateBegin"/> and <paramref name="dateEnd"/>.
        /// </summary>
        /// <param name="ruleset">the ruleset categorization.</param>
        /// <param name="dateBegin">the filtering begin date.</param>
        /// <param name="dateEnd">the filtering end date.</param>
        /// <returns></returns>
        ValueTask<IReadOnlyCollection<Rule>> GetRulesAsync(string ruleset, DateTime dateBegin, DateTime dateEnd);

        /// <summary>
        /// Gets the rules filtered by specified arguments.
        /// </summary>
        /// <param name="rulesFilterArgs">The rules filter arguments.</param>
        /// <returns></returns>
        ValueTask<IReadOnlyCollection<Rule>> GetRulesByAsync(RulesFilterArgs rulesFilterArgs);

        /// <summary>
        /// Gets the rulesets from the data source.
        /// </summary>
        /// <returns></returns>
        ValueTask<IReadOnlyDictionary<string, Ruleset>> GetRulesetsAsync();

        /// <summary>
        /// Updates the existent rule on data source.
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <returns></returns>
        ValueTask UpdateRuleAsync(Rule rule);
    }
}