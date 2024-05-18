namespace Regulae.Tests.Evaluation.Interpreted.ValueEvaluation
{
    using FluentAssertions;
    using Regulae.Evaluation.Interpreted.ValueEvaluation;
    using System;
    using Xunit;

    public class StartsWithOperatorEvalStrategyTests
    {
        [Fact]
        public void Eval_GivenIntegers1And2_ThrowsNotSupportedException()
        {
            // Arrange
            var expectedLeftOperand = 1;
            var expectedRightOperand = 2;

            var sut = new StartsWithOperatorEvalStrategy();

            // Act
            var notSupportedException = Assert.Throws<NotSupportedException>(() => sut.Eval(expectedLeftOperand, expectedRightOperand));

            // Arrange
            notSupportedException.Should().NotBeNull();
            notSupportedException.Message.Should().Contain("System.Int32");
        }

        [Fact]
        public void Eval_GivenStringsTheQuickBrownFoxJumpsOverTheLazyDogAndFox_ReturnsTrue()
        {
            // Arrange
            var expectedLeftOperand = "The quick brown fox jumps over the lazy dog";
            var expectedRightOperand = "The";

            var sut = new StartsWithOperatorEvalStrategy();

            // Act
            var actual = sut.Eval(expectedLeftOperand, expectedRightOperand);

            // Arrange
            actual.Should().BeTrue();
        }

        [Fact]
        public void Eval_GivenStringsxpto12345emailpt_ReturnsFalse()
        {
            // Arrange
            var expectedLeftOperand = "xpto12345@email.pt";
            var expectedRightOperand = "abc";

            var sut = new StartsWithOperatorEvalStrategy();

            // Act
            var actual = sut.Eval(expectedLeftOperand, expectedRightOperand);

            // Arrange
            actual.Should().BeFalse();
        }

        [Fact]
        public void Eval_GivenStringsxpto12345emailpt_ReturnsTrue()
        {
            // Arrange
            var expectedLeftOperand = "xpto12345@email.pt";
            var expectedRightOperand = "xpto";

            var sut = new StartsWithOperatorEvalStrategy();

            // Act
            var actual = sut.Eval(expectedLeftOperand, expectedRightOperand);

            // Arrange
            actual.Should().BeTrue();
        }
    }
}