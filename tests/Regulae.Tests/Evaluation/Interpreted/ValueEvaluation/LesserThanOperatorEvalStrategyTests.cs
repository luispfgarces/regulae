namespace Regulae.Tests.Evaluation.Interpreted.ValueEvaluation
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Regulae.Evaluation.Interpreted.ValueEvaluation;
    using Xunit;

    public class LesserThanOperatorEvalStrategyTests
    {
        public static IEnumerable<object[]> UnsupportedOperandTypesCombinations => new[]
        {
            new[] { 1, new object() },
            new[] { new object(), 1 }
        };

        [Fact]
        public void Eval_GivenAsIntegers0And1_ReturnsTrue()
        {
            // Assert
            var expectedLeftOperand = 0;
            var expectedRightOperand = 1;

            var sut = new LesserThanOperatorEvalStrategy();

            // Act
            var actual = sut.Eval(expectedLeftOperand, expectedRightOperand);

            // Arrange
            actual.Should().BeTrue();
        }

        [Fact]
        public void Eval_GivenAsIntegers1And1_ReturnsFalse()
        {
            // Assert
            var expectedLeftOperand = 1;
            var expectedRightOperand = 1;

            var sut = new LesserThanOperatorEvalStrategy();

            // Act
            var actual = sut.Eval(expectedLeftOperand, expectedRightOperand);

            // Arrange
            actual.Should().BeFalse();
        }

        [Fact]
        public void Eval_GivenAsIntegers2And1_ReturnsFalse()
        {
            // Assert
            var expectedLeftOperand = 2;
            var expectedRightOperand = 1;

            var sut = new LesserThanOperatorEvalStrategy();

            // Act
            var actual = sut.Eval(expectedLeftOperand, expectedRightOperand);

            // Arrange
            actual.Should().BeFalse();
        }

        [MemberData(nameof(UnsupportedOperandTypesCombinations))]
        [Theory]
        public void Eval_GivenUnsupportedOperandTypes_ThrowsNotSupportedException(object leftOperand, object rightOperand)
        {
            // Arrange
            var sut = new LesserThanOperatorEvalStrategy();

            // Act
            var notSupportedException = Assert.Throws<NotSupportedException>(() => sut.Eval(leftOperand, rightOperand));

            // Assert
            notSupportedException.Should().NotBeNull();
            notSupportedException.Message.Should().Contain(nameof(IComparable));
        }
    }
}