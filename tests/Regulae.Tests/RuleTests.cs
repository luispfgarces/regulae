namespace Regulae.Tests
{
    using System;
    using FluentAssertions;
    using Moq;
    using Regulae;
    using Regulae.ConditionNodes;
    using Regulae.Tests.TestStubs;
    using Xunit;

    public class RuleTests
    {
        [Fact]
        public void Clone_WithRuleWithoutRootCondition_ReturnsCopy()
        {
            // Arrange
            var rule = new Rule("Name", "Ruleset", DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1), new ContentContainer(_ => new object()))
            {
                Priority = 1,
                RootCondition = null,
            };

            // Act
            var actual = rule.Clone();

            // Assert
            actual.Should().BeEquivalentTo(rule);
        }

        [Fact]
        public void Clone_WithRuleWithRootCondition_ReturnsCopy()
        {
            // Arrange
            var rule = new Rule("Name", "Ruleset", DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1), new ContentContainer(_ => new object()))
            {
                Priority = 1,
                RootCondition = new ValueConditionNode(ConditionNames.PluviosityRate.ToString(), Operators.GreaterThanOrEqual, 80.0m),
            };
            rule.RootCondition.Properties["key1"] = "value1";
            rule.RootCondition.Properties["key2"] = new object();

            // Act
            var actual = rule.Clone();

            // Assert
            actual.Should().BeEquivalentTo(rule);
        }

        [Fact]
        public void ContentContainer_HavingSettedInstance_ReturnsProvidedInstance()
        {
            // Arrange
            var expected = new ContentContainer((_) => null);

            var sut = new Rule("Name", "Ruleset", DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1), expected);

            // Act
            var actual = sut.ContentContainer;

            // Assert
            actual.Should().BeSameAs(expected);
        }

        [Fact]
        public void DateBegin_HavingSettedValue_ReturnsProvidedValue()
        {
            // Arrange
            var expected = new DateTime(2018, 07, 19);

            var sut = new Rule("Name", "Ruleset", expected, DateTime.UtcNow.AddDays(1), new ContentContainer(_ => new object()));

            // Act
            var actual = sut.DateBegin;

            // Assert
            actual.Should().Be(expected);
        }

        [Fact]
        public void DateEnd_HavingSettedValue_ReturnsProvidedValue()
        {
            // Arrange
            var expected = new DateTime(2018, 07, 19);

            var sut = new Rule("Name", "Ruleset", DateTime.UtcNow.AddDays(-1), expected, new ContentContainer(_ => new object()));

            // Act
            var actual = sut.DateEnd;

            // Assert
            actual.Should().Be(expected);
        }

        [Fact]
        public void DateEnd_NotHavingSettedValue_ReturnsNull()
        {
            // Arrange
            var sut = new Rule("Name", "Ruleset", DateTime.UtcNow.AddDays(-1), null, new ContentContainer(_ => new object()));

            // Act
            var actual = sut.DateEnd;

            // Assert
            actual.Should().BeNull();
        }

        [Fact]
        public void Name_HavingSettedValue_ReturnsProvidedValue()
        {
            // Arrange
            var expected = "My awesome name";

            var sut = new Rule(expected, "Ruleset", DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1), new ContentContainer(_ => new object()));

            // Act
            var actual = sut.Name;

            // Assert
            actual.Should().Be(expected);
        }

        [Fact]
        public void Priority_HavingSettedValue_ReturnsProvidedValue()
        {
            // Arrange
            var expected = 123;

            var sut = new Rule("Name", "Ruleset", DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1), new ContentContainer(_ => new object()))
            {
                Priority = expected
            };

            // Act
            var actual = sut.Priority;

            // Arrange
            actual.Should().Be(expected);
        }

        [Fact]
        public void RootCondition_HavingSettedInstance_ReturnsProvidedInstance()
        {
            // Arrange
            var mockConditionNode = new Mock<IConditionNode>();
            var expected = mockConditionNode.Object;

            var sut = new Rule("Name", "Ruleset", DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1), new ContentContainer(_ => new object()))
            {
                RootCondition = expected
            };

            // Act
            var actual = sut.RootCondition;

            // Assert
            actual.Should().BeSameAs(expected);
        }
    }
}