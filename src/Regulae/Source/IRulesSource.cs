namespace Regulae.Source
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Regulae;

    internal interface IRulesSource
    {
        ValueTask AddRuleAsync(AddRuleArgs args);

        ValueTask CreateConditionAsync(CreateConditionArgs args);

        ValueTask CreateRulesetAsync(CreateRulesetArgs args);

        ValueTask<IReadOnlyDictionary<string, Condition>> GetConditionsAsync(GetConditionsArgs args);

        ValueTask<IReadOnlyCollection<Rule>> GetRulesAsync(GetRulesArgs args);

        ValueTask<IReadOnlyDictionary<string, Ruleset>> GetRulesetsAsync(GetRulesetsArgs args);

        ValueTask<IReadOnlyCollection<Rule>> GetRulesFilteredAsync(GetRulesFilteredArgs args);

        ValueTask UpdateRuleAsync(UpdateRuleArgs args);
    }
}