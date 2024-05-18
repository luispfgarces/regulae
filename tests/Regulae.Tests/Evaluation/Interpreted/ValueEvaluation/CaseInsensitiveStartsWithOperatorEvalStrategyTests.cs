namespace Regulae.Tests.Evaluation.Interpreted.ValueEvaluation
{
    using System;
    using FluentAssertions;
    using Regulae.Evaluation.Interpreted.ValueEvaluation;
    using Xunit;

    public class CaseInsensitiveStartsWithOperatorEvalStrategyTests
    {
        [Fact]
        public void Eval_GivenIntegers1And2_ThrowsNotSupportedException()
        {
            // Arrange
            var expectedLeftOperand = 1;
            var expectedRightOperand = 2;

            var sut = new CaseInsensitiveStartsWithOperatorEvalStrategy();

            // Act
            var notSupportedException = Assert.Throws<NotSupportedException>(() => sut.Eval(expectedLeftOperand, expectedRightOperand));

            // Assert
            notSupportedException.Should().NotBeNull();
            notSupportedException.Message.Should().Contain("System.Int32");
        }

        [Theory]
        [InlineData("Random Message One Two Three", "Message")]
        [InlineData("Random Message One Two Three", "Randmo")]
        [InlineData("Random Message One Two Three", "The")]
        public void Eval_GivenStringsRandomMessageOneTwoThree_ReturnsFalse(string expectedLeftOperand, string expectedRightOperand)
        {
            // Arrange
            var sut = new CaseInsensitiveStartsWithOperatorEvalStrategy();

            // Act
            var actual = sut.Eval(expectedLeftOperand, expectedRightOperand);

            // Assert
            actual.Should().BeFalse();
        }

        [Theory]
        [InlineData("The quick brown fox jumps over the lazy dog", "The")]
        [InlineData("the quick brown fox jumps over the lazy dog", "The")]
        [InlineData("The quick brown fox jumps over the lazy dog", "the")]
        public void Eval_GivenStringsTheQuickBrownFoxJumpsOverTheLazyDog_CaseInsensitive_ReturnsTrue(string expectedLeftOperand, string expectedRightOperand)
        {
            // Arrange
            var sut = new CaseInsensitiveStartsWithOperatorEvalStrategy();

            // Act
            var actual = sut.Eval(expectedLeftOperand, expectedRightOperand);

            // Assert
            actual.Should().BeTrue();
        }
    }
}