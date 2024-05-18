namespace Regulae.Tests.Evaluation.Interpreted.ValueEvaluation
{
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Regulae.Evaluation.Interpreted.ValueEvaluation;
    using Xunit;

    public class InOperatorEvalStrategyTests
    {
        [Fact]
        public void Eval_GivenInteger1AndCollectionContainingInteger1_ReturnsTrue()
        {
            // Arrange
            object leftOperand = 1;
            var rightOperand = Enumerable.Range(1, 3).Cast<object>();

            var inOperatorEvalStrategy = new InOperatorEvalStrategy();

            // Act
            var result = inOperatorEvalStrategy.Eval(leftOperand, rightOperand);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void Eval_GivenInteger2AndCollectionNotContainingInteger2_ReturnsFalse()
        {
            // Arrange
            object leftOperand = 2;
            var rightOperand = Enumerable.Range(6, 3).Cast<object>();

            var inOperatorEvalStrategy = new InOperatorEvalStrategy();

            // Act
            var result = inOperatorEvalStrategy.Eval(leftOperand, rightOperand);

            // Assert
            result.Should().BeFalse();
        }
    }
}