namespace Regulae.Source
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Regulae;

    internal interface IRulesSourceMiddleware
    {
        ValueTask HandleAddRuleAsync(
            AddRuleArgs args,
            AddRuleDelegate next);

        ValueTask HandleCreateConditionAsync(
            CreateConditionArgs args,
            CreateConditionDelegate next);

        ValueTask HandleCreateRulesetAsync(
            CreateRulesetArgs args,
            CreateRulesetDelegate next);

        ValueTask<IReadOnlyDictionary<string, Condition>> HandleGetConditionsAsync(
            GetConditionsArgs args,
            GetConditionsDelegate next);

        ValueTask<IReadOnlyCollection<Rule>> HandleGetRulesAsync(
            GetRulesArgs args,
            GetRulesDelegate next);

        ValueTask<IReadOnlyDictionary<string, Ruleset>> HandleGetRulesetsAsync(
            GetRulesetsArgs args,
            GetRulesetsDelegate next);

        ValueTask<IReadOnlyCollection<Rule>> HandleGetRulesFilteredAsync(
            GetRulesFilteredArgs args,
            GetRulesFilteredDelegate next);

        ValueTask HandleUpdateRuleAsync(
            UpdateRuleArgs args,
            UpdateRuleDelegate next);
    }
}