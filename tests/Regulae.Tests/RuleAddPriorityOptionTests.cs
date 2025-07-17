namespace Regulae.Tests
{
    using FluentAssertions;
    using Regulae;
    using Xunit;

    public class RuleAddPriorityOptionTests
    {
        [Fact]
        public void AtLargestNumber_NoConditions_ReturnsNewObjectSettedAtLargestNumberPriority()
        {
            // Arrange
            var priorityOption = PriorityOptions.AtLargestNumber;

            // Act
            var actual = RuleAddPriorityOption.AtLargestNumber;

            // Assert

            actual.Should().NotBeNull();
            actual.AtNumberOptionValue.Should().Be(0);
            actual.AtRuleNameOptionValue.Should().BeNull();
            actual.PriorityOption.Should().Be(priorityOption);
        }

        [Fact]
        public void AtNumber_GivenPriority1_ReturnsNewObjectSettedAtNumber1()
        {
            // Arrange
            var priorityNumber = 1;
            var priorityOption = PriorityOptions.AtNumber;

            // Act
            var actual = RuleAddPriorityOption.AtNumber(priorityNumber);

            // Assert

            actual.Should().NotBeNull();
            actual.AtNumberOptionValue.Should().Be(priorityNumber);
            actual.AtRuleNameOptionValue.Should().BeNull();
            actual.PriorityOption.Should().Be(priorityOption);
        }

        [Fact]
        public void AtRuleName_GivenRuleNameSample_ReturnsNewObjectSettedAtRuleNamePriorityAndAtNameSample()
        {
            // Arrange
            var ruleName = "Sample";
            var priorityOption = PriorityOptions.AtRuleName;

            // Act
            var actual = RuleAddPriorityOption.AtRuleName(ruleName);

            // Assert

            actual.Should().NotBeNull();
            actual.AtNumberOptionValue.Should().Be(0);
            actual.AtRuleNameOptionValue.Should().Be(ruleName);
            actual.PriorityOption.Should().Be(priorityOption);
        }

        [Fact]
        public void AtSmallestNumber_NoConditions_ReturnsNewObjectSettedAtSmallestNumberPriority()
        {
            // Arrange
            var priorityOption = PriorityOptions.AtSmallestNumber;

            // Act
            var actual = RuleAddPriorityOption.AtSmallestNumber;

            // Assert

            actual.Should().NotBeNull();
            actual.AtNumberOptionValue.Should().Be(0);
            actual.AtRuleNameOptionValue.Should().BeNull();
            actual.PriorityOption.Should().Be(priorityOption);
        }
    }
}