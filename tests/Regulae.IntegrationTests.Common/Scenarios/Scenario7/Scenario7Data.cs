namespace Regulae.IntegrationTests.Common.Scenarios.Scenario7
{
    using System.Collections.Generic;
    using Regulae;
    using Regulae.Extensions;
    using Regulae.Generic;
    using Regulae.IntegrationTests.Common.Scenarios;

    public class Scenario7Data : IScenarioData<Rulesets, ConditionNames>
    {
        public IEnumerable<(ConditionNames, DataTypes)> AllConditions => new[]
        {
            (ConditionNames.ReleaseYear, DataTypes.Integer),
            (ConditionNames.Artist, DataTypes.String),
            (ConditionNames.Lyrics, DataTypes.String),
        };

        public IEnumerable<Rule<Rulesets, ConditionNames>> AllRules => this.GetRules();

        public IEnumerable<Rulesets> AllRulesets => new[] { Rulesets.Songs };

        private IEnumerable<Rule<Rulesets, ConditionNames>> GetRules()
        {
            var rule1Result = Rule.Create<Rulesets, ConditionNames>("Benchmark 2 - Bohemian Rapsody")
                .InRuleset(Rulesets.Songs)
                .SetContent("Bohemian Rapsody")
                .SinceUtc(2000, 1, 1)
                .ApplyWhen(c => c
                    .And(x => x
                        .Value(ConditionNames.Artist, Operators.Equal, "Queen")
                        .Value(ConditionNames.Lyrics, Operators.Contains, "real life")
                        .Value(ConditionNames.ReleaseYear, Operators.GreaterThanOrEqual, 1973)
                        .Value(ConditionNames.ReleaseYear, Operators.GreaterThanOrEqual, 1977)
                    )
                )
                .Build();

            var rule2Result = Rule.Create<Rulesets, ConditionNames>("Benchmark 2 - Stairway to Heaven")
                .InRuleset(Rulesets.Songs)
                .SetContent("Stairway to Heaven")
                .SinceUtc(2000, 1, 1)
                .ApplyWhen(c => c
                    .And(x => x
                        .Value(ConditionNames.Artist, Operators.Equal, "Led Zeppelin")
                        .Or(sub => sub
                            .Value(ConditionNames.Lyrics, Operators.Contains, "all that glitters is gold")
                            .Value(ConditionNames.Lyrics, Operators.Contains, "it makes me wonder")
                        )
                        .Value(ConditionNames.ReleaseYear, Operators.GreaterThanOrEqual, 1973)
                        .Value(ConditionNames.ReleaseYear, Operators.GreaterThanOrEqual, 1977)
                    )
                )
                .Build();

            return new[] { rule1Result.Rule, rule2Result.Rule };
        }
    }
}