namespace Regulae.IntegrationTests.Common.Scenarios.Scenario3
{
    using System;
    using System.Collections.Generic;
    using Regulae;
    using Regulae.Generic;

    public class Scenario3Data : IScenarioData<SecuritySystemActionables, SecuritySystemConditions>
    {
        private static string dataSourceFilePath = $@"{Environment.CurrentDirectory}/Scenarios/Scenario3/regulae-tests.security-system-actionables.json";
        private readonly IEnumerable<Rule<SecuritySystemActionables, SecuritySystemConditions>> rules;

        public Scenario3Data()
        {
            this.rules = RulesFromJsonFile.Load.FromJsonFile<SecuritySystemActionables, SecuritySystemConditions>(dataSourceFilePath, typeof(SecuritySystemAction));
        }

        public IEnumerable<(SecuritySystemConditions, DataTypes)> AllConditions => new[]
        {
            (SecuritySystemConditions.TemperatureCelsius, DataTypes.Decimal),
            (SecuritySystemConditions.PowerStatus, DataTypes.String),
            (SecuritySystemConditions.SmokeRate, DataTypes.Decimal),
        };

        public IEnumerable<Rule<SecuritySystemActionables, SecuritySystemConditions>> AllRules => this.rules;

        public IEnumerable<SecuritySystemActionables> AllRulesets => new[] { SecuritySystemActionables.PowerSystem, SecuritySystemActionables.FireSystem };
    }
}