namespace Regulae.IntegrationTests.Scenarios.Scenario5
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.Extensions.DependencyInjection;
    using Regulae;
    using Regulae.Extensions;
    using Regulae.IntegrationTests.Common.Scenarios.Scenario5;
    using Regulae.Providers.InMemory;
    using Xunit;

    public class BestServerTests
    {
        public static readonly IEnumerable<object[]> DataTest = new List<object[]>
        {
            new object[]
            {
                new Dictionary<BestServerConditions, object>
                {
                    { BestServerConditions.Price,100 },
                    { BestServerConditions.Memory,12 },
                    { BestServerConditions.StoragePartionable,true },
                    { BestServerConditions.Brand,"AMD" },
                },
                "Best Server Top5"
            },
            new object[]
            {
                new Dictionary<BestServerConditions, object>
                {
                    { BestServerConditions.Price,110 },
                    { BestServerConditions.Memory,12 },
                    { BestServerConditions.StoragePartionable,true },
                    { BestServerConditions.Brand,"AMD" },
                },
                "Best Server Default"
            },
            new object[]
            {
                new Dictionary<BestServerConditions, object>
                {
                    { BestServerConditions.Price,100 },
                    { BestServerConditions.Memory,12 },
                    { BestServerConditions.StoragePartionable,true },
                },
                "Best Server Default"
            }
        };

        [Fact]
        public async Task BestServer_DeactivatingBestServerTop5_ReturnsBestServerDefault()
        {
            // Arrange
            var serviceProvider = new ServiceCollection()
                .AddInMemoryRulesDataSource(ServiceLifetime.Singleton)
                .BuildServiceProvider();

            var rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                 .SetInMemoryDataSource(serviceProvider)
                 .Build();
            var genericRulesEngine = rulesEngine.MakeGeneric<BestServerConfigurations, BestServerConditions>(opt =>
            {
                opt.AutoCreateConditions = true;
            });

            await genericRulesEngine.CreateRulesetAsync(BestServerConfigurations.BestServerEvaluation);

            // Act 1 - Create rule with "in" operator
            var ruleBuilderResult = Rule.Create<BestServerConfigurations, BestServerConditions>("Best Server Top5")
                .InRuleset(BestServerConfigurations.BestServerEvaluation)
                .SetContent("Top5")
                .Since(DateTime.Parse("2021-05-29T11:00:00Z")).Until(DateTime.Parse("2021-05-31Z"))
                .ApplyWhen(c => c
                    .And(a => a
                        .Value(BestServerConditions.Price, Operators.In, new[] { 100m, 200m, 300m })
                        .Value(BestServerConditions.Memory, Operators.In, new[] { 12, 16, 24, 36 })
                        .Value(BestServerConditions.Memory, Operators.NotIn, new[] { 4, 8 })
                        .Value(BestServerConditions.StoragePartionable, Operators.In, new[] { true })
                        .Value(BestServerConditions.Brand, Operators.In, new[] { "AMD", "Intel", "Cisco" })
                    )
                )
                .Build();

            // Assert 1
            ruleBuilderResult.Should().NotBeNull();
            var errors = ruleBuilderResult.Errors.Any()
                ? ruleBuilderResult.Errors
                    .Select(e => e.Message)
                    .Aggregate((s1, s2) => $"{s1}\n- {s2}")
                : string.Empty;
            ruleBuilderResult.IsSuccess.Should().BeTrue(
                $"errors have occurred while creating rule: \n[\n- {errors}\n]");

            // Act 2 - Add new rule with "in" operator
            var rule = ruleBuilderResult.Rule;

            var addRuleResult = await genericRulesEngine.AddRuleAsync(rule, RuleAddPriorityOption.AtSmallestNumber);

            // Assert 2 - Verify if rule was added
            addRuleResult.Should().NotBeNull();
            addRuleResult.IsSuccess.Should().BeTrue();

            // Act 3 - Create rule default
            ruleBuilderResult = Rule.Create<BestServerConfigurations, BestServerConditions>("Best Server Default")
                .InRuleset(BestServerConfigurations.BestServerEvaluation)
                .SetContent("Default")
                .Since(DateTime.Parse("2021-05-29Z")).Until(DateTime.Parse("2021-05-31Z"))
                .Build();

            // Assert 3
            ruleBuilderResult.Should().NotBeNull();
            errors = ruleBuilderResult.Errors.Any()
                ? ruleBuilderResult.Errors
                    .Select(e => e.Message)
                    .Aggregate((s1, s2) => $"{s1}\n- {s2}")
                : string.Empty;
            ruleBuilderResult.IsSuccess.Should().BeTrue(
                $"errors have occurred while creating rule: \n[\n- {errors}\n]");

            // Act 4 - Add new default rule
            addRuleResult = await genericRulesEngine.AddRuleAsync(ruleBuilderResult.Rule, RuleAddPriorityOption.AtLargestNumber);

            // Assert 4 - Verify if rule was added
            addRuleResult.Should().NotBeNull();
            addRuleResult.IsSuccess.Should().BeTrue();

            // Act 5 - Evaluate new rule with "in" operator
            var matchDateTime = DateTime.Parse("2021-05-29T12:34:52Z");
            var conditions = new Dictionary<BestServerConditions, object>
            {
                { BestServerConditions.Price, 100 },
                { BestServerConditions.Memory, 12 },
                { BestServerConditions.StoragePartionable, true },
                { BestServerConditions.Brand, "AMD" },
            };

            var actual = await genericRulesEngine.MatchOneAsync(BestServerConfigurations.BestServerEvaluation, matchDateTime, conditions);

            // Assert 5
            actual.Should().NotBeNull();
            actual.Should().BeEquivalentTo(rule);

            // Act 6 - Update Best Server Top5 rule deactivate
            var updateRuleResult = await genericRulesEngine.DeactivateRuleAsync(rule);

            // Assert 6
            updateRuleResult.Should().NotBeNull();
            updateRuleResult.IsSuccess.Should().BeTrue();

            // Act 7 - Evaluate rule should be default now
            actual = await genericRulesEngine.MatchOneAsync(BestServerConfigurations.BestServerEvaluation, matchDateTime, conditions);

            // Assert 7
            actual.Should().NotBeNull();
            actual.Should().BeEquivalentTo(ruleBuilderResult.Rule);
        }

        [Theory]
        [MemberData(nameof(DataTest))]
        public async Task BestServer_InEvaluation(Dictionary<BestServerConditions, object> conditions, string expectedRuleName)
        {
            // Arrange
            var rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .SetInMemoryDataSource()
                .Build();
            var genericRulesEngine = rulesEngine.MakeGeneric<BestServerConfigurations, BestServerConditions>(opt =>
            {
                opt.AutoCreateConditions = true;
            });

            await genericRulesEngine.CreateRulesetAsync(BestServerConfigurations.BestServerEvaluation);

            // Act 1 - Create rule with "in" operator
            var ruleBuilderResult = Rule.Create<BestServerConfigurations, BestServerConditions>("Best Server Top5")
                .InRuleset(BestServerConfigurations.BestServerEvaluation)
                .SetContent("Top5")
                .Since(DateTime.Parse("2021-05-29Z"))
                .Until(DateTime.Parse("2021-05-31Z"))
                .ApplyWhen(c => c
                    .And(a => a
                        .Value(BestServerConditions.Price, Operators.In, new[] { 100m, 200m, 300m })
                        .Value(BestServerConditions.Memory, Operators.In, new[] { 12, 16, 24, 36 })
                        .Value(BestServerConditions.Memory, Operators.NotIn, new[] { 4, 8 })
                        .Value(BestServerConditions.StoragePartionable, Operators.In, new[] { true })
                        .Value(BestServerConditions.Brand, Operators.In, new[] { "AMD", "Intel", "Cisco" })
                    )
                )
                .Build();

            // Act 2 - Create rule default
            var ruleBuilderResultDefault = Rule.Create<BestServerConfigurations, BestServerConditions>("Best Server Default")
                .InRuleset(BestServerConfigurations.BestServerEvaluation)
                .SetContent("Default")
                .Since(DateTime.Parse("2021-05-29Z"))
                .Until(DateTime.Parse("2021-05-31Z"))
                .Build();

            // Assert 1
            ruleBuilderResult.Should().NotBeNull();
            var errors = ruleBuilderResult.Errors.Any()
                ? ruleBuilderResult.Errors
                    .Select(e => e.Message)
                    .Aggregate((s1, s2) => $"{s1}\n- {s2}")
                : string.Empty;
            ruleBuilderResult.IsSuccess.Should().BeTrue(
                $"errors have occurred while creating rule: \n[\n- {errors}\n]");

            // Assert 3
            ruleBuilderResultDefault.Should().NotBeNull();
            errors = ruleBuilderResultDefault.Errors.Any()
                ? ruleBuilderResultDefault.Errors
                    .Select(e => e.Message)
                    .Aggregate((s1, s2) => $"{s1}\n- {s2}")
                : string.Empty;
            ruleBuilderResultDefault.IsSuccess.Should().BeTrue(
                $"errors have occurred while creating rule default: \n[\n- {errors}\n]");

            // Act 2 - Add new rule with "in" operator
            await rulesEngine.AddRuleAsync(ruleBuilderResultDefault.Rule, RuleAddPriorityOption.AtNumber(2));
            await rulesEngine.AddRuleAsync(ruleBuilderResult.Rule, RuleAddPriorityOption.AtNumber(1));

            var matchDateTime = DateTime.Parse("2021-05-29T12:34:52Z");

            var actual = await genericRulesEngine.MatchOneAsync(BestServerConfigurations.BestServerEvaluation, matchDateTime, conditions);

            // Assert 3
            actual.Should().NotBeNull();
            actual.Name.Should().BeEquivalentTo(expectedRuleName);
        }

        [Fact]
        public async Task BestServer_UpdatingBestServerTop5_ReturnsBestServerDefault()
        {
            // Arrange
            var serviceProvider = new ServiceCollection()
                .AddInMemoryRulesDataSource(ServiceLifetime.Singleton)
                .BuildServiceProvider();

            var rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                 .SetInMemoryDataSource(serviceProvider)
                 .Build();
            var genericRulesEngine = rulesEngine.MakeGeneric<BestServerConfigurations, BestServerConditions>(opt =>
            {
                opt.AutoCreateConditions = true;
            });

            await genericRulesEngine.CreateRulesetAsync(BestServerConfigurations.BestServerEvaluation);

            // Act 1 - Create rule with "in" operator
            var ruleBuilderResult = Rule.Create<BestServerConfigurations, BestServerConditions>("Best Server Top5")
                .InRuleset(BestServerConfigurations.BestServerEvaluation)
                .SetContent("Top5")
                .Since(DateTime.Parse("2021-05-29T11:00:00Z"))
                .Until(DateTime.Parse("2021-05-31Z"))
                .ApplyWhen(c => c
                    .And(a => a
                        .Value(BestServerConditions.Price, Operators.In, new[] { 100m, 200m, 300m })
                        .Value(BestServerConditions.Memory, Operators.In, new[] { 12, 16, 24, 36 })
                        .Value(BestServerConditions.Memory, Operators.NotIn, new[] { 4, 8 })
                        .Value(BestServerConditions.StoragePartionable, Operators.In, new[] { true })
                        .Value(BestServerConditions.Brand, Operators.In, new[] { "AMD", "Intel", "Cisco" })
                    )
                )
                .Build();

            // Assert 1
            ruleBuilderResult.Should().NotBeNull();
            var errors = ruleBuilderResult.Errors.Any()
                ? ruleBuilderResult.Errors
                    .Select(e => e.Message)
                    .Aggregate((s1, s2) => $"{s1}\n- {s2}")
                : string.Empty;
            ruleBuilderResult.IsSuccess.Should().BeTrue(
                $"errors have occurred while creating rule: \n[\n- {errors}\n]");

            // Act 2 - Add new rule with "in" operator
            var rule = ruleBuilderResult.Rule;

            var addRuleResult = await genericRulesEngine.AddRuleAsync(rule, RuleAddPriorityOption.AtSmallestNumber);

            // Assert 2 - Verify if rule was added
            addRuleResult.Should().NotBeNull();
            addRuleResult.IsSuccess.Should().BeTrue();

            // Act 3 - Create rule default
            var ruleBuilderResultDefault = Rule.Create<BestServerConfigurations, BestServerConditions>("Best Server Default")
                .InRuleset(BestServerConfigurations.BestServerEvaluation)
                .SetContent("Default")
                .Since(DateTime.Parse("2021-05-29Z"))
                .Until(DateTime.Parse("2021-05-31Z"))
                .Build();

            // Assert 3
            ruleBuilderResultDefault.Should().NotBeNull();
            errors = ruleBuilderResultDefault.Errors.Any()
                ? ruleBuilderResultDefault.Errors
                    .Select(e => e.Message)
                    .Aggregate((s1, s2) => $"{s1}\n- {s2}")
                : string.Empty;
            ruleBuilderResultDefault.IsSuccess.Should().BeTrue(
                $"errors have occurred while creating rule: \n[\n- {errors}\n]");

            // Act 4 - Add new default rule
            addRuleResult = await genericRulesEngine.AddRuleAsync(ruleBuilderResultDefault.Rule, RuleAddPriorityOption.AtLargestNumber);

            // Assert 4 - Verify if rule was added
            addRuleResult.Should().NotBeNull();
            addRuleResult.IsSuccess.Should().BeTrue();

            // Act 5 - Evaluate new rule with "in" operator
            var matchDateTime = DateTime.Parse("2021-05-29T12:34:52Z");
            var conditions = new Dictionary<BestServerConditions, object>
            {
                { BestServerConditions.Price, 100 },
                { BestServerConditions.Memory, 12 },
                { BestServerConditions.StoragePartionable, true },
                { BestServerConditions.Brand, "AMD" },
            };

            var actual = await genericRulesEngine.MatchOneAsync(BestServerConfigurations.BestServerEvaluation, matchDateTime, conditions);

            // Assert 5
            actual.Should().NotBeNull();
            actual.Should().BeEquivalentTo(rule);

            // Act 6 - Update Best Server Top5 date end
            rule.DateEnd = DateTime.Parse("2021-05-29T12:10:00Z");
            var updateRuleResult = await genericRulesEngine.UpdateRuleAsync(rule);

            // Assert 6
            updateRuleResult.Should().NotBeNull();
            updateRuleResult.IsSuccess.Should().BeTrue();

            // Act 7 - Evaluate rule should be default now
            actual = await genericRulesEngine.MatchOneAsync(BestServerConfigurations.BestServerEvaluation, matchDateTime, conditions);

            // Assert 7
            actual.Should().NotBeNull();
            actual.Should().BeEquivalentTo(ruleBuilderResultDefault.Rule);
        }
    }
}