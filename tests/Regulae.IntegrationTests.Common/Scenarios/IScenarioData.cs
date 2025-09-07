namespace Regulae.IntegrationTests.Common.Scenarios
{
    using System.Collections.Generic;
    using Regulae.Generic;

    public interface IScenarioData<TRuleset, TCondition>
        where TRuleset : notnull
        where TCondition : notnull
    {
        IEnumerable<(TCondition, DataTypes)> AllConditions { get; }
        IEnumerable<Rule<TRuleset, TCondition>> AllRules { get; }
        IEnumerable<TRuleset> AllRulesets { get; }
    }
}