namespace Regulae.Tests.Evaluation.Interpreted.ValueEvaluation
{
    using System;
    using FluentAssertions;
    using Regulae.Evaluation.Interpreted.ValueEvaluation;
    using Xunit;

    public class CaseInsensitiveEndsWithOperatorEvalStrategyTests
    {
        [Fact]
        public void Eval_GivenIntegers1And2_ThrowsNotSupportedException()
        {
            // Arrange
            var expectedLeftOperand = 1;
            var expectedRightOperand = 2;

            var sut = new CaseInsensitiveEndsWithOperatorEvalStrategy();

            // Act
            var notSupportedException = Assert.Throws<NotSupportedException>(() => sut.Eval(expectedLeftOperand, expectedRightOperand));

            // Assert
            notSupportedException.Should().NotBeNull();
            notSupportedException.Message.Should().Contain("System.Int32");
        }

        [Theory]
        [InlineData("Random Message One Two Three", "Threee")]
        [InlineData("Random Message One Two Three", "Random")]
        [InlineData("Random Message One Two Three", "The")]
        public void Eval_GivenStringsRandomMessageOneTwoThree_ReturnsFalse(string expectedLeftOperand, string expectedRightOperand)
        {
            // Arrange
            var sut = new CaseInsensitiveEndsWithOperatorEvalStrategy();

            // Act
            var actual = sut.Eval(expectedLeftOperand, expectedRightOperand);

            // Assert
            actual.Should().BeFalse();
        }

        [Theory]
        [InlineData("The quick brown fox jumps over the lazy dog", "dog")]
        [InlineData("The quick brown fox jumps over the lazy Dog", "dog")]
        [InlineData("The quick brown fox jumps over the lazy dog", "Dog")]
        public void Eval_GivenStringsTheQuickBrownFoxJumpsOverTheLazyDogAndFox_CaseInsensitive_ReturnsTrue(string expectedLeftOperand, string expectedRightOperand)
        {
            // Arrange
            var sut = new CaseInsensitiveEndsWithOperatorEvalStrategy();

            // Act
            var actual = sut.Eval(expectedLeftOperand, expectedRightOperand);

            // Assert
            actual.Should().BeTrue();
        }
    }
}