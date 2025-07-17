namespace Regulae.Providers.MongoDb.IntegrationTests.Scenarios.Scenario3
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using FluentAssertions;
    using MongoDB.Driver;
    using Newtonsoft.Json;
    using Regulae;
    using Regulae.Extensions;
    using Regulae.IntegrationTests.Common.Scenarios.Scenario3;
    using Regulae.Providers.MongoDb;
    using Regulae.Providers.MongoDb.DataModel;
    using Regulae.Providers.MongoDb.IntegrationTests;
    using Xunit;

    public sealed class BuildingSecuritySystemControlTests : IDisposable
    {
        private readonly MongoClient mongoClient;
        private readonly MongoDbProviderSettings mongoDbProviderSettings;

        public BuildingSecuritySystemControlTests()
        {
            this.mongoClient = CreateMongoClient();
            this.mongoDbProviderSettings = CreateProviderSettings();

            var rulesFile = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("Regulae.Providers.MongoDb.IntegrationTests.Scenarios.Scenario3.regulae-tests.security-system-actionables.json");

            IEnumerable<RuleDataModel> rules;
            using (var streamReader = new StreamReader(rulesFile ?? throw new InvalidOperationException("Could not load rules file.")))
            {
                var json = streamReader.ReadToEnd();

                var array = JsonConvert.DeserializeObject<IEnumerable<RuleDataModel>>(json, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                });

                rules = array!.Select(t =>
                {
                    SecuritySystemAction securitySystemAction = t.Content.ToObject<SecuritySystemAction>();
                    dynamic dynamicContent = new ExpandoObject();
                    dynamicContent.ActionId = securitySystemAction.ActionId;
                    dynamicContent.ActionName = securitySystemAction.ActionName;
                    t.Content = dynamicContent;

                    return t;
                }).ToList();
            }

            var rulesets = rules
                .Select(r => new RulesetDataModel
                {
                    Name = r.Ruleset,
                })
                .Distinct()
                .Select(r => new RulesetDataModel
                {
                    Creation = DateTime.UtcNow,
                    Id = Guid.NewGuid(),
                    Name = r.Name,
                })
                .ToArray();

            var conditions = rules
                .SelectMany(r =>
                {
                    return GetConditions(r.RootCondition);
                    static IEnumerable<ConditionDataModel> GetConditions(ConditionNodeDataModel conditionNode)
                    {
                        if (conditionNode is ComposedConditionNodeDataModel composedConditionNodeDataModel)
                        {
                            foreach (var childCondition in composedConditionNodeDataModel.ChildConditionNodes)
                            {
                                foreach (var condition in GetConditions(childCondition))
                                {
                                    yield return condition;
                                }
                            }

                            yield break;
                        }

                        var valueConditionNodeDataModel = (ValueConditionNodeDataModel)conditionNode;
                        yield return new ConditionDataModel
                        {
                            Name = valueConditionNodeDataModel.Condition,
                            DataType = valueConditionNodeDataModel.RightOperand.DataType.ToString(),
                        };
                    }
                })
                .Distinct()
                .Select(c => new ConditionDataModel
                {
                    Creation = DateTime.UtcNow,
                    Id = Guid.NewGuid(),
                    Name = c.Name,
                    DataType = c.DataType,
                })
                .ToArray();

            var mongoDatabase = this.mongoClient.GetDatabase(this.mongoDbProviderSettings.DatabaseName);

            mongoDatabase.DropCollection(this.mongoDbProviderSettings.RulesetsCollectionName);
            var conditionsMongoCollection = mongoDatabase.GetCollection<ConditionDataModel>(this.mongoDbProviderSettings.ConditionsCollectionName);
            conditionsMongoCollection.InsertMany(conditions);

            mongoDatabase.DropCollection(this.mongoDbProviderSettings.RulesetsCollectionName);
            var rulesetsMongoCollection = mongoDatabase.GetCollection<RulesetDataModel>(this.mongoDbProviderSettings.RulesetsCollectionName);
            rulesetsMongoCollection.InsertMany(rulesets);

            mongoDatabase.DropCollection(this.mongoDbProviderSettings.RulesCollectionName);
            var rulesMongoCollection = mongoDatabase.GetCollection<RuleDataModel>(this.mongoDbProviderSettings.RulesCollectionName);
            rulesMongoCollection.InsertMany(rules);
        }

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
                .SetMongoDbDataSource(this.mongoClient, this.mongoDbProviderSettings)
                .Configure(opt => opt.UseEvaluationStrategy(evaluationStrategy))
                .Build();
            var genericRulesEngine = rulesEngine.MakeGeneric<SecuritySystemActionables, SecuritySystemConditions>();

            // Act
            var newRuleResult = Rule.Create<SecuritySystemActionables, SecuritySystemConditions>("Activate ventilation system rule")
                .InRuleset(SecuritySystemActionables.FireSystem)
                .SetContent(new SecuritySystemAction
                {
                    ActionId = new Guid("ef0d65ae-ec76-492a-84db-5cb9090c3eaa"),
                    ActionName = "ActivateVentilationSystem"
                })
                .Since(new DateTime(2018, 01, 01))
                .ApplyWhen(b => b.Value(SecuritySystemConditions.SmokeRate, Operators.GreaterThanOrEqual, 30.0m))
                .Build();
            var newRule = newRuleResult.Rule;

            _ = await genericRulesEngine.AddRuleAsync(newRule, RuleAddPriorityOption.AtLargestNumber);

            var actual = await genericRulesEngine.MatchManyAsync(securitySystemActionable, expectedMatchDate, expectedConditions);

            // Assert
            actual.Should().NotBeNull();

            var securitySystemActions = actual.Select(r => r.ContentContainer.GetContentAs<SecuritySystemAction>()).ToList();

            securitySystemActions.Should().Contain(ssa => ssa.ActionName == "CallFireBrigade")
                .And.Contain(ssa => ssa.ActionName == "CallPolice")
                .And.Contain(ssa => ssa.ActionName == "ActivateSprinklers")
                .And.Contain(ssa => ssa.ActionName == "ActivateVentilationSystem")
                .And.HaveCount(4);
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
                .SetMongoDbDataSource(this.mongoClient, this.mongoDbProviderSettings)
                .Configure(opt => opt.UseEvaluationStrategy(evaluationStrategy))
                .Build();
            var genericRulesEngine = rulesEngine.MakeGeneric<SecuritySystemActionables, SecuritySystemConditions>();

            // Act
            var actual = await genericRulesEngine.MatchManyAsync(securitySystemActionable, expectedMatchDate, expectedConditions);

            // Assert
            actual.Should().NotBeNull();

            var securitySystemActions = actual.Select(r => r.ContentContainer.GetContentAs<SecuritySystemAction>()).ToList();

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
                .SetMongoDbDataSource(this.mongoClient, this.mongoDbProviderSettings)
                .Configure(opt => opt.UseEvaluationStrategy(evaluationStrategy))
                .Build();
            var genericRulesEngine = rulesEngine.MakeGeneric<SecuritySystemActionables, SecuritySystemConditions>();

            // Act
            var actual = await genericRulesEngine.MatchManyAsync(securitySystemActionable, expectedMatchDate, expectedConditions);

            // Assert
            actual.Should().NotBeNull();

            var securitySystemActions = actual.Select(r => r.ContentContainer.GetContentAs<SecuritySystemAction>()).ToList();

            securitySystemActions.Should().Contain(ssa => ssa.ActionName == "EnableEmergencyLights")
                .And.HaveCount(1);
        }

        public void Dispose()
        {
            var mongoDatabase = this.mongoClient.GetDatabase(this.mongoDbProviderSettings.DatabaseName);
            mongoDatabase.DropCollection(this.mongoDbProviderSettings.RulesCollectionName);
            mongoDatabase.DropCollection(this.mongoDbProviderSettings.RulesetsCollectionName);
            mongoDatabase.DropCollection(this.mongoDbProviderSettings.ConditionsCollectionName);
        }

        private static MongoClient CreateMongoClient() => new($"mongodb://{SettingsProvider.GetMongoDbHost()}:27017");

        private static MongoDbProviderSettings CreateProviderSettings() => new()
        {
            DatabaseName = "regulae-tests",
            RulesCollectionName = "security-system-actionables",
        };
    }
}