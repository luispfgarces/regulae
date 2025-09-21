namespace Regulae.Tests
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Regulae;
    using Xunit;

    public class OperationResultTests
    {
        [Fact]
        public void Failure_GivenCollectionOfErrors_ReturnsRuleOperationResultWithErrorsAndNoSuccess()
        {
            // Arrange
            IList<OperationError> errors = new[] { OperationError.Create("E01", "Error1"), OperationError.Create("E02", "Error2") };

            // Act
            var ruleOperationResult = Operation.Failure(errors);

            // Assert
            ruleOperationResult.Should().NotBeNull();
            ruleOperationResult.IsSuccess.Should().BeFalse();
            ruleOperationResult.Errors.Should().BeSameAs(errors);
        }

        [Fact]
        public void Failure_GivenNullCollectionOfErrors_ThrowsArgumentNullException()
        {
            // Arrange
            IList<OperationError> errors = null;

            // Act
            var argumentNullException = Assert.Throws<ArgumentNullException>(() => Operation.Failure(errors));

            // Assert
            argumentNullException.Should().NotBeNull();
            argumentNullException.ParamName.Should().Be(nameof(errors));
        }

        [Fact]
        public void Success_NoConditionGiven_ReturnsRuleOperationResultWithoutErrorsAndSuccess()
        {
            // Act
            var ruleOperationResult = Operation.Success();

            // Assert
            ruleOperationResult.Should().NotBeNull();
            ruleOperationResult.IsSuccess.Should().BeTrue();
            ruleOperationResult.Errors.Should().NotBeNull()
                .And.BeEmpty();
        }
    }
}