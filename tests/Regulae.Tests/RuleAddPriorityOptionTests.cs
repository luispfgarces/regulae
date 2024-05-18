namespace Regulae.Tests
{
    using FluentAssertions;
    using Regulae;
    using Xunit;

    public class RuleAddPriorityOptionTests
    {
        [Fact]
        public void AtBottom_NoConditions_ReturnsNewObjectSettedAtBottomPriority()
        {
            // Arrange
            var priorityOption = PriorityOptions.AtBottom;

            // Act
            var actual = RuleAddPriorityOption.AtBottom;

            // Assert

            actual.Should().NotBeNull();
            actual.AtPriorityNumberOptionValue.Should().Be(0);
            actual.AtRuleNameOptionValue.Should().BeNull();
            actual.PriorityOption.Should().Be(priorityOption);
        }

        [Fact]
        public void AtTop_NoConditions_ReturnsNewObjectSettedAtTopPriority()
        {
            // Arrange
            var priorityOption = PriorityOptions.AtTop;

            // Act
            var actual = RuleAddPriorityOption.AtTop;

            // Assert

            actual.Should().NotBeNull();
            actual.AtPriorityNumberOptionValue.Should().Be(0);
            actual.AtRuleNameOptionValue.Should().BeNull();
            actual.PriorityOption.Should().Be(priorityOption);
        }

        [Fact]
        public void ByPriorityNumber_GivenPriority1_ReturnsNewObjectSettedByPriorityNumberAndAtPriority1()
        {
            // Arrange
            var priorityNumber = 1;
            var priorityOption = PriorityOptions.AtPriorityNumber;

            // Act
            var actual = RuleAddPriorityOption.ByPriorityNumber(priorityNumber);

            // Assert

            actual.Should().NotBeNull();
            actual.AtPriorityNumberOptionValue.Should().Be(priorityNumber);
            actual.AtRuleNameOptionValue.Should().BeNull();
            actual.PriorityOption.Should().Be(priorityOption);
        }

        [Fact]
        public void ByRuleName_GivenRuleNameSample_ReturnsNewObjectSettedByRuleNamePriorityAndAtNameSample()
        {
            // Arrange
            var ruleName = "Sample";
            var priorityOption = PriorityOptions.AtRuleName;

            // Act
            var actual = RuleAddPriorityOption.ByRuleName(ruleName);

            // Assert

            actual.Should().NotBeNull();
            actual.AtPriorityNumberOptionValue.Should().Be(0);
            actual.AtRuleNameOptionValue.Should().Be(ruleName);
            actual.PriorityOption.Should().Be(priorityOption);
        }
    }
}