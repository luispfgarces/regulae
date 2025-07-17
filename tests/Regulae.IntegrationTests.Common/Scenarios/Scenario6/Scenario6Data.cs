namespace Regulae.IntegrationTests.Common.Scenarios.Scenario6
{
    using System;
    using System.Collections.Generic;
    using Regulae;
    using Regulae.Generic;
    using Regulae.IntegrationTests.Common.Scenarios;

    public class Scenario6Data : IScenarioData<Rulesets, ConditionNames>
    {
        public IEnumerable<(ConditionNames, DataTypes)> AllConditions => new[]
        {
            (ConditionNames.IntegerCondition, DataTypes.Integer),
            (ConditionNames.BooleanCondition, DataTypes.Boolean),
            (ConditionNames.DecimalCondition, DataTypes.Decimal),
            (ConditionNames.StringCondition, DataTypes.String),
        };

        public IEnumerable<Rule<Rulesets, ConditionNames>> AllRules => this.GetRules();

        public IEnumerable<Rulesets> AllRulesets => new[] { Rulesets.Sample1 };

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