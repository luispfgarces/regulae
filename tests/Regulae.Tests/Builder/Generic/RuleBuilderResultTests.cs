namespace Regulae.Tests.Builder.Generic
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Regulae.Builder.Generic;
    using Regulae.Generic;
    using Regulae.Tests.TestStubs;
    using Xunit;

    public class RuleBuilderResultTests
    {
        [Fact]
        public void Failure_ShouldThrowArgumentNullException_WhenErrorsIsNull()
        {
            // Act
            Action act = () => RuleBuilderResult.Failure<RulesetNames, ConditionNames>(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithParameterName("errors");
        }

        [Fact]
        public void Failure_ShouldReturnResultWithErrors_AndNullRule()
        {
            // Arrange
            var errors = new List<OperationError>
            {
                new OperationError("E1", "Error 1"),
                new OperationError("E2", "Error 2")
            };

            // Act
            var result = RuleBuilderResult.Failure<RulesetNames, ConditionNames>(errors);

            // Assert
            result.Rule.Should().BeNull();
            result.Errors.Should().BeEquivalentTo(errors);
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public void Success_ShouldThrowArgumentNullException_WhenRuleIsNull()
        {
            // Act
            Action act = () => RuleBuilderResult.Success<RulesetNames, ConditionNames>(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithParameterName("rule");
        }

        [Fact]
        public void Success_ShouldReturnResultWithRule_AndNoErrors()
        {
            // Arrange
            var rule = new Rule("Test rule", "Test ruleset", DateTime.UtcNow, null, new ObjectContentContainer(new object()));
            var genericRule = rule.ToGenericRule<RulesetNames, ConditionNames>();

            // Act
            var result = RuleBuilderResult.Success<RulesetNames, ConditionNames>(genericRule);

            // Assert
            result.Rule.Should().BeSameAs(genericRule);
            result.Errors.Should().BeEmpty();
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void RuleBuilderResult_Constructor_ShouldSetProperties()
        {
            // Arrange
            var rule = new Rule("Test rule", "Test ruleset", DateTime.UtcNow, null, new ObjectContentContainer(new object()));
            var genericRule = rule.ToGenericRule<RulesetNames, ConditionNames>();
            var errors = new List<OperationError>
            {
                new OperationError("E1", "Error 1")
            };

            // Act
            var result = new RuleBuilderResult<RulesetNames, ConditionNames>(genericRule, errors);

            // Assert
            result.Rule.Should().BeSameAs(genericRule);
            result.Errors.Should().BeEquivalentTo(errors);
        }
    }
}
