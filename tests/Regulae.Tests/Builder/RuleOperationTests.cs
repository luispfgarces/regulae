namespace Regulae.Tests.Builder
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Regulae.Builder;
    using Xunit;

    public class RuleOperationTests
    {
        [Fact]
        public void Failure_ShouldThrowArgumentNullException_WhenErrorsIsNull()
        {
            // Act
            Action act = () => RuleOperation.Failure(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("errors");
        }

        [Fact]
        public void Failure_ShouldReturnResultWithErrors_WhenErrorsIsNotNull()
        {
            // Arrange
            var errors = new List<OperationError>
            {
                new OperationError("E1", "Error 1"),
                new OperationError("E2", "Error 2")
            };

            // Act
            var result = RuleOperation.Failure(errors);

            // Assert
            result.Should().NotBeNull();
            result.Rule.Should().BeNull();
            result.Errors.Should().BeEquivalentTo(errors);
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public void Success_ShouldThrowArgumentNullException_WhenRuleIsNull()
        {
            // Act
            Action act = () => RuleOperation.Success(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("rule");
        }

        [Fact]
        public void Success_ShouldReturnResultWithRuleAndNoErrors_WhenRuleIsNotNull()
        {
            // Arrange
            var rule = new Rule(
                name: "TestRule",
                ruleset: "TestRuleset",
                dateBegin: DateTime.UtcNow,
                dateEnd: null,
                contentContainer: new ObjectContentContainer(new object())
            );

            // Act
            var result = RuleOperation.Success(rule);

            // Assert
            result.Should().NotBeNull();
            result.Rule.Should().Be(rule);
            result.Errors.Should().BeEmpty();
            result.IsSuccess.Should().BeTrue();
        }
    }
}