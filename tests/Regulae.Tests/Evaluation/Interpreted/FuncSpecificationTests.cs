namespace Regulae.Tests.Evaluation.Interpreted
{
    using System;
    using FluentAssertions;
    using Moq;
    using Regulae.Evaluation.Interpreted;
    using Xunit;

    public class FuncSpecificationTests
    {
        [Fact]
        public void And_GivenOtherSpecification_ReturnsAndSpecification()
        {
            // Arrange
            var mockOtherSpecification = new Mock<ISpecification<object>>();
            var expectedOtherSpecification = mockOtherSpecification.Object;
            Func<object, bool> expectedEvalFunc = (input) => true;

            var sut = new FuncSpecification<object>(expectedEvalFunc);

            // Act
            ISpecification<object> actual = sut.And(expectedOtherSpecification);

            // Assert
            actual.Should().BeOfType<AndSpecification<object>>();
        }

        [Fact]
        public void IsSatisfiedBy_GivenFuncEvalsAsFalse_ReturnsFalse()
        {
            // Arrange
            var expectedInput = new object();
            Func<object, bool> expectedEvalFunc = (input) => false;

            var sut = new FuncSpecification<object>(expectedEvalFunc);

            // Act
            var actual = sut.IsSatisfiedBy(expectedInput);

            // Assert
            actual.Should().BeFalse();
        }

        [Fact]
        public void IsSatisfiedBy_GivenFuncEvalsAsTrue_ReturnsTrue()
        {
            // Arrange
            var expectedInput = new object();
            Func<object, bool> expectedEvalFunc = (input) => true;

            var sut = new FuncSpecification<object>(expectedEvalFunc);

            // Act
            var actual = sut.IsSatisfiedBy(expectedInput);

            // Assert
            actual.Should().BeTrue();
        }

        [Fact]
        public void Or_GivenOtherSpecification_ReturnsOrSpecification()
        {
            // Arrange
            var mockOtherSpecification = new Mock<ISpecification<object>>();
            var expectedOtherSpecification = mockOtherSpecification.Object;
            Func<object, bool> expectedEvalFunc = (input) => true;

            var sut = new FuncSpecification<object>(expectedEvalFunc);

            // Act
            ISpecification<object> actual = sut.Or(expectedOtherSpecification);

            // Assert
            actual.Should().BeOfType<OrSpecification<object>>();
        }
    }
}