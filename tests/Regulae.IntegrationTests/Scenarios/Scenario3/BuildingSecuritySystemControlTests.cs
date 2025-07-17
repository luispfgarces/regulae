namespace Regulae.IntegrationTests.Scenarios.Scenario3
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Regulae;
    using Regulae.Extensions;
    using Regulae.IntegrationTests.Common.Scenarios;
    using Regulae.IntegrationTests.Common.Scenarios.Scenario3;
    using Regulae.Providers.InMemory;
    using Xunit;

    public class BuildingSecuritySystemControlTests
    {
        [Theory]
        [InlineData(EvaluationStrategies.Interpreted)]
        [InlineData(EvaluationStrategies.Compiled)]
        public async Task BuildingSecuritySystem_FireScenario_ReturnsActionsToTrigger(EvaluationStrategies evaluationStrategy)
        {
            // Assert
            const SecuritySystemActionables securitySystemActionable = SecuritySystemActionables.FireSystem;

            var expectedMatchDate = new DateTime(2018, 06, 01);
            var expectedConditions = new Dictionary<SecuritySystemConditions, object>
            {
                { SecuritySystemConditions.TemperatureCelsius, 100.0m },
                { SecuritySystemConditions.SmokeRate, 55.0m },
                { SecuritySystemConditions.PowerStatus, "Online" },
            };

            var rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .SetInMemoryDataSource()
                .Configure(options =>
                {
                    options.UseEvaluationStrategy(evaluationStrategy);
                })
                .Build();

            await ScenarioLoader.LoadScenarioAsync(rulesEngine, new Scenario3Data());
            var genericRulesEngine = rulesEngine.MakeGeneric<SecuritySystemActionables, SecuritySystemConditions>();

            // Act
            var actual = await genericRulesEngine.MatchManyAsync(securitySystemActionable, expectedMatchDate, expectedConditions);

            // Assert
            actual.Should().NotBeNull();

            IEnumerable<SecuritySystemAction> securitySystemActions = actual.Select(r => r.ContentContainer.GetContentAs<SecuritySystemAction>()).ToList();

            securitySystemActions.Should().Contain(ssa => ssa.ActionName == "CallFireBrigade")
                .And.Contain(ssa => ssa.ActionName == "CallPolice")
                .And.Contain(ssa => ssa.ActionName == "ActivateSprinklers")
                .And.HaveCount(3);
        }

        [Theory]
        [InlineData(EvaluationStrategies.Interpreted)]
        [InlineData(EvaluationStrategies.Compiled)]
        public async Task BuildingSecuritySystem_PowerFailureScenario_ReturnsActionsToTrigger(EvaluationStrategies evaluationStrategy)
        {
            // Assert
            const SecuritySystemActionables securitySystemActionable = SecuritySystemActionables.PowerSystem;

            var expectedMatchDate = new DateTime(2018, 06, 01);
            var expectedConditions = new Dictionary<SecuritySystemConditions, object>
            {
                { SecuritySystemConditions.TemperatureCelsius, 100.0m },
                { SecuritySystemConditions.SmokeRate, 55.0m },
                { SecuritySystemConditions.PowerStatus, "Offline" },
            };

            var rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .SetInMemoryDataSource()
                .Configure(options =>
                {
                    options.UseEvaluationStrategy(evaluationStrategy);
                })
                .Build();

            await ScenarioLoader.LoadScenarioAsync(rulesEngine, new Scenario3Data());
            var genericRulesEngine = rulesEngine.MakeGeneric<SecuritySystemActionables, SecuritySystemConditions>();

            // Act
            var actual = await genericRulesEngine.MatchManyAsync(securitySystemActionable, expectedMatchDate, expectedConditions);

            // Assert
            actual.Should().NotBeNull();

            IEnumerable<SecuritySystemAction> securitySystemActions = actual.Select(r => r.ContentContainer.GetContentAs<SecuritySystemAction>()).ToList();

            securitySystemActions.Should().Contain(ssa => ssa.ActionName == "EnableEmergencyLights")
                .And.Contain(ssa => ssa.ActionName == "EnableEmergencyPower")
                .And.Contain(ssa => ssa.ActionName == "CallPowerGridPicket");
        }

        [Theory]
        [InlineData(EvaluationStrategies.Interpreted)]
        [InlineData(EvaluationStrategies.Compiled)]
        public async Task BuildingSecuritySystem_PowerShutdownScenario_ReturnsActionsToTrigger(EvaluationStrategies evaluationStrategy)
        {
            // Assert
            const SecuritySystemActionables securitySystemActionable = SecuritySystemActionables.PowerSystem;

            var expectedMatchDate = new DateTime(2018, 06, 01);
            var expectedConditions = new Dictionary<SecuritySystemConditions, object>
            {
                { SecuritySystemConditions.TemperatureCelsius, 100.0m },
                { SecuritySystemConditions.SmokeRate, 55.0m },
                { SecuritySystemConditions.PowerStatus, "Shutdown" },
            };

            var rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .SetInMemoryDataSource()
                .Configure(options =>
                {
                    options.UseEvaluationStrategy(evaluationStrategy);
                })
                .Build();

            await ScenarioLoader.LoadScenarioAsync(rulesEngine, new Scenario3Data());
            var genericRulesEngine = rulesEngine.MakeGeneric<SecuritySystemActionables, SecuritySystemConditions>();

            // Act
            var actual = await genericRulesEngine.MatchManyAsync(securitySystemActionable, expectedMatchDate, expectedConditions);

            // Assert
            actual.Should().NotBeNull();

            IEnumerable<SecuritySystemAction> securitySystemActions = actual.Select(r => r.ContentContainer.GetContentAs<SecuritySystemAction>()).ToList();

            securitySystemActions.Should().Contain(ssa => ssa.ActionName == "EnableEmergencyLights")
                .And.HaveCount(1);
        }
    }
}