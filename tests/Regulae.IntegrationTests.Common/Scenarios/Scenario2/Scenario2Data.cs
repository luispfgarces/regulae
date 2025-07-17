namespace Regulae.IntegrationTests.Common.Scenarios.Scenario2
{
    using System;
    using System.Collections.Generic;
    using Regulae;
    using Regulae.Generic;

    public class Scenario2Data : IScenarioData<CarInsuranceRulesetNames, CarInsuranceConditionNames>
    {
        private static string dataSourceFilePath = $@"{Environment.CurrentDirectory}/Scenarios/Scenario2/regulae-tests.car-insurance-advisor.json";
        private readonly IEnumerable<Rule<CarInsuranceRulesetNames, CarInsuranceConditionNames>> rules;

        public Scenario2Data()
        {
            this.rules = RulesFromJsonFile.Load.FromJsonFile<CarInsuranceRulesetNames, CarInsuranceConditionNames>(
                dataSourceFilePath,
                typeof(CarInsuranceAdvices),
                serializedContent: false);
        }

        public IEnumerable<(CarInsuranceConditionNames, DataTypes)> AllConditions => new[]
        {
            (CarInsuranceConditionNames.RepairCostsCommercialValueRate, DataTypes.Decimal),
            (CarInsuranceConditionNames.SelfDamageCoverage, DataTypes.Decimal),
            (CarInsuranceConditionNames.RepairCosts, DataTypes.Decimal),
            (CarInsuranceConditionNames.ClaimDescription, DataTypes.String),
        };

        public IEnumerable<Rule<CarInsuranceRulesetNames, CarInsuranceConditionNames>> AllRules => this.rules;

        public IEnumerable<CarInsuranceRulesetNames> AllRulesets => new[] { CarInsuranceRulesetNames.CarInsuranceAdvice };
    }
}