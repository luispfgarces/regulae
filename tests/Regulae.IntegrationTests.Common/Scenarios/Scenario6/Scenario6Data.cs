namespace Regulae.IntegrationTests.Common.Scenarios.Scenario6
{
    using System;
    using System.Collections.Generic;
    using Regulae;
    using Regulae.Generic;
    using Regulae.IntegrationTests.Common.Scenarios;

    public class Scenario6Data : IScenarioData<Rulesets, ConditionNames>
    {
        public IDictionary<ConditionNames, object> Conditions => new Dictionary<ConditionNames, object>
        {
            { ConditionNames.StringCondition, "Let's benchmark this!" },
        };

        public DateTime MatchDate => DateTime.Parse("2022-10-01");

        public IEnumerable<Rule<Rulesets, ConditionNames>> Rules => this.GetRules();

        private IEnumerable<Rule<Rulesets, ConditionNames>> GetRules()
        {
            var ruleResult = Rule.Create<Rulesets, ConditionNames>("Benchmark 1 - Test rule")
                .InRuleset(Rulesets.Sample1)
                .SetContent("Dummy Content")
                .Since(DateTime.Parse("2000-01-01"))
                .ApplyWhen(ConditionNames.StringCondition, Operators.Equal, "Let's benchmark this!")
                .Build();

            return new[] { ruleResult.Rule };
        }
    }
}