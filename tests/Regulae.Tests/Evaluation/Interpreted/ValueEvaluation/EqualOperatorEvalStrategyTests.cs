namespace Regulae.Tests.Evaluation.Interpreted.ValueEvaluation
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Regulae.Evaluation.Interpreted.ValueEvaluation;
    using Xunit;

    public class EqualOperatorEvalStrategyTests
    {
        public static IEnumerable<object[]> UnsupportedOperandTypesCombinations => new[]
        {
            new[] { 1, new object() },
            new[] { new object(), 1 }
        };

        [Fact]
        public void Eval_GivenAsIntegers1And1_ReturnsTrue()

        {
            // Arrange
            var expectedLeftOperand = 1;
            var expectedRightOperand = 1;

            var sut = new EqualOperatorEvalStrategy();

            // Act
            var actual = sut.Eval(expectedLeftOperand, expectedRightOperand);

            // Arrange
            actual.Should().BeTrue();
        }

        [Fact]
        public void Eval_GivenAsIntegers1And2_ReturnsFalse()
        {
            // Arrange
            var expectedLeftOperand = 1;
            var expectedRightOperand = 2;

            var sut = new EqualOperatorEvalStrategy();

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
            var sut = new EqualOperatorEvalStrategy();

            // Act
            var notSupportedException = Assert.Throws<NotSupportedException>(() => sut.Eval(leftOperand, rightOperand));

            // Assert
            notSupportedException.Should().NotBeNull();
            notSupportedException.Message.Should().Contain(nameof(IComparable));
        }
    }
}