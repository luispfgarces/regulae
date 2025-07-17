namespace Regulae.IntegrationTests.Common.Scenarios.Scenario1
{
    using System;
    using System.Collections.Generic;
    using Regulae;
    using Regulae.Generic;

    public class Scenario1Data : IScenarioData<Scenario1RulesetNames, Scenario1ConditionNames>
    {
        private static string dataSourceFilePath = $@"{Environment.CurrentDirectory}/Scenarios/Scenario1/regulae-tests.body-mass-index.datasource.json";
        private readonly IEnumerable<Rule<Scenario1RulesetNames, Scenario1ConditionNames>> rules;

        public Scenario1Data()
        {
            this.rules = RulesFromJsonFile.Load.FromJsonFile<Scenario1RulesetNames, Scenario1ConditionNames>(dataSourceFilePath, typeof(Formula));
        }

        public IEnumerable<(Scenario1ConditionNames, DataTypes)> AllConditions => new[]
        {
            (Scenario1ConditionNames.Age, DataTypes.Integer),
        };

        public IEnumerable<Rule<Scenario1RulesetNames, Scenario1ConditionNames>> AllRules => this.rules;

        public IEnumerable<Scenario1RulesetNames> AllRulesets => new[] { Scenario1RulesetNames.BodyMassIndexFormula };
    }
}