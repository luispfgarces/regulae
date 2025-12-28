namespace Regulae.Tests.Providers.InMemory
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using Regulae;
    using Regulae.Providers.InMemory;
    using Regulae.Providers.InMemory.DataModel;
    using Xunit;

    public class InMemoryRulesStorageTests
    {
        [Fact]
        public void CreateRuleset_AddsNewRuleset()
        {
            // Arrange
            var storage = new InMemoryRulesStorage();

            // Act
            storage.CreateRuleset("rs1");

            // Assert
            var rulesets = storage.GetRulesets();

            rulesets.Should().ContainSingle(r => r.Name == "rs1");
        }

        [Fact]
        public void CreateRuleset_WhenExists_DoesNotThrowButDoesNotDuplicate()
        {
            // Arrange
            var storage = new InMemoryRulesStorage();

            // Act
            storage.CreateRuleset("rs2");
            storage.CreateRuleset("rs2");

            // Assert
            var rulesets = storage.GetRulesets();
            rulesets.Should().ContainSingle(r => r.Name == "rs2");
        }

        [Fact]
        public void CreateCondition_AddsCondition_AndThrowsWhenDuplicate()
        {
            // Arrange
            var storage = new InMemoryRulesStorage();

            // Act
            storage.CreateCondition("c1", DataTypes.String);
            Action act = () => storage.CreateCondition("c1", DataTypes.String);

            // Assert
            act.Should().Throw<InvalidOperationException>();

            var conditions = storage.GetConditions();
            conditions.Should().ContainKey("c1");
            conditions["c1"].DataType.Should().Be(DataTypes.String);
        }

        [Fact]
        public void AddRule_AddsAndOrdersByPriority_AndThrowsOnDuplicateName()
        {
            // Arrange
            var storage = new InMemoryRulesStorage();
            storage.CreateRuleset("rset");

            var now = DateTime.UtcNow;
            var r1 = new RuleDataModel { Name = "A", Priority = 10, DateBegin = now.AddDays(-1), Ruleset = "rset", Content = new object() };
            var r2 = new RuleDataModel { Name = "B", Priority = 5, DateBegin = now.AddDays(-1), Ruleset = "rset", Content = new object() };
            var r3 = new RuleDataModel { Name = "C", Priority = 7, DateBegin = now.AddDays(-1), Ruleset = "rset", Content = new object() };

            // Act 1
            storage.AddRule(r1);
            storage.AddRule(r2);
            storage.AddRule(r3);

            // Assert 1
            var rules = storage.GetRulesBy("rset").ToList();
            rules.Should().HaveCount(3);
            // should be ordered by Priority ascending
            rules[0].Name.Should().Be("B");
            rules[1].Name.Should().Be("C");
            rules[2].Name.Should().Be("A");

            // Act 2 - try to add rule with
            // duplicate name
            Action act = () => storage.AddRule(new RuleDataModel { Name = "A", Priority = 1, DateBegin = DateTime.UtcNow, Ruleset = "rset", Content = new object() });

            // Assert 2
            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void UpdateRule_ReplacesAndReorders_ThrowsWhenNotExist()
        {
            // Arrange
            var storage = new InMemoryRulesStorage();
            storage.CreateRuleset("rsu");

            var now = DateTime.UtcNow;
            var r1 = new RuleDataModel { Name = "A", Priority = 1, DateBegin = now.AddDays(-1), Ruleset = "rsu", Content = new object() };
            var r2 = new RuleDataModel { Name = "B", Priority = 2, DateBegin = now.AddDays(-1), Ruleset = "rsu", Content = new object() };
            storage.AddRule(r1);
            storage.AddRule(r2);

            // update A to priority 5
            var updated = new RuleDataModel { Name = "A", Priority = 5, DateBegin = now.AddDays(-1), Ruleset = "rsu", Content = new object() };

            // Act 1
            storage.UpdateRule(updated);

            // Assert 1
            var rules = storage.GetRulesBy("rsu").ToList();
            rules[0].Name.Should().Be("B");
            rules[1].Name.Should().Be("A");

            // Act 2 - try to update non-existing rule
            Action act = () => storage.UpdateRule(new RuleDataModel { Name = "X", Priority = 1, DateBegin = DateTime.UtcNow, Ruleset = "rsu", Content = new object() });

            // Assert 2
            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void GetRulesBy_WhenRulesetMissing_Throws()
        {
            // Arrange
            var storage = new InMemoryRulesStorage();

            // Act
            Action act = () => storage.GetRulesBy("missing");

            // Assert
            act.Should().Throw<InvalidOperationException>();
        }
    }
}
