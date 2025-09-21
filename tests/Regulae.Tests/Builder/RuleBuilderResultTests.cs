namespace Regulae.Tests.Builder
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Regulae;
    using Regulae.Builder;
    using Xunit;

    public class RuleBuilderResultTests
    {
        [Fact]
        public void Ctor_GivenAllValidParameters_ReturnsRuleBuilderResultWithFailure()
        {
            // Arrange
            IList<OperationError> expectedErrors = [OperationError.Create("E1", "Error1"), OperationError.Create("E2", "Error2")];

            // Act
            var ruleBuilderResult = new RuleBuilderResult(null!, expectedErrors);

            // Assert
            ruleBuilderResult.Should().NotBeNull();
            ruleBuilderResult.IsSuccess.Should().BeFalse();
            ruleBuilderResult.Errors.Should().NotBeNull().And.BeEquivalentTo(expectedErrors);
            ruleBuilderResult.Rule.Should().BeNull();
        }

        [Fact]
        public void Ctor_GivenNullErrorsCollection_ThrowsArgumentNullException()
        {
            // Arrange
            IList<OperationError> expectedErrors = null;

            // Act
            var argumentNullException = Assert.Throws<ArgumentNullException>(() => new RuleBuilderResult(null!, expectedErrors));

            // Arrange
            argumentNullException.Should().NotBeNull();
            argumentNullException.ParamName.Should().Be("errors");
        }

        [Fact]
        public void Ctor_GivenAllValidParameters_ReturnsRuleBuilderResultWithSuccessAndNewRule()
        {
            // Arrange
            var rule = new Rule("Test rule", "Test ruleset", DateTime.UtcNow, null, new ObjectContentContainer(new object()));

            // Act
            var ruleBuilderResult = new RuleBuilderResult(rule, []);

            // Assert
            ruleBuilderResult.Should().NotBeNull();
            ruleBuilderResult.IsSuccess.Should().BeTrue();
            ruleBuilderResult.Errors.Should().NotBeNull().And.BeEmpty();
            ruleBuilderResult.Rule.Should().NotBeNull().And.Be(rule);
        }
    }
}