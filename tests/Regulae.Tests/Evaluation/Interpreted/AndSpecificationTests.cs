namespace Regulae.Tests.Evaluation.Interpreted
{
    using FluentAssertions;
    using Moq;
    using Regulae.Evaluation.Interpreted;
    using Xunit;

    public class AndSpecificationTests
    {
        [Fact]
        public void IsSatisfiedBy_GivenLeftSpecificationEvalAsFalse_ShortCircuitsDoesNotEvalRightSpecificationAndReturnsFalse()
        {
            // Arrange
            var expectedInput = new object();

            var mockLeftSpecification = new Mock<ISpecification<object>>();
            mockLeftSpecification.Setup(x => x.IsSatisfiedBy(It.Is<object>(o => o == expectedInput)))
                .Returns(false);
            var mockRightSpecification = new Mock<ISpecification<object>>();
            mockRightSpecification.Setup(x => x.IsSatisfiedBy(It.Is<object>(o => o == expectedInput)))
                .Returns(false);

            var expectedLeftSpecification = mockLeftSpecification.Object;
            var expectedRightSpecification = mockRightSpecification.Object;

            var sut = new AndSpecification<object>(expectedLeftSpecification, expectedRightSpecification);

            // Act
            var actual = sut.IsSatisfiedBy(expectedInput);

            // Assert
            actual.Should().BeFalse();

            mockLeftSpecification.Verify(x => x.IsSatisfiedBy(It.Is<object>(o => o == expectedInput)), Times.Once());
            mockRightSpecification.Verify(x => x.IsSatisfiedBy(It.Is<object>(o => o == expectedInput)), Times.Never());
        }

        [Fact]
        public void IsSatisfiedBy_GivenLeftSpecificationEvalAsTrueAndRightSpecificationEvalAsFalse_EvalsBothSpecificationsAndReturnsFalse()
        {
            // Arrange
            var expectedInput = new object();

            var mockLeftSpecification = new Mock<ISpecification<object>>();
            mockLeftSpecification.Setup(x => x.IsSatisfiedBy(It.Is<object>(o => o == expectedInput)))
                .Returns(true);
            var mockRightSpecification = new Mock<ISpecification<object>>();
            mockRightSpecification.Setup(x => x.IsSatisfiedBy(It.Is<object>(o => o == expectedInput)))
                .Returns(false);

            var expectedLeftSpecification = mockLeftSpecification.Object;
            var expectedRightSpecification = mockRightSpecification.Object;

            var sut = new AndSpecification<object>(expectedLeftSpecification, expectedRightSpecification);

            // Act
            var actual = sut.IsSatisfiedBy(expectedInput);

            // Assert
            actual.Should().BeFalse();

            mockLeftSpecification.Verify(x => x.IsSatisfiedBy(It.Is<object>(o => o == expectedInput)), Times.Once());
            mockRightSpecification.Verify(x => x.IsSatisfiedBy(It.Is<object>(o => o == expectedInput)), Times.Once());
        }

        [Fact]
        public void IsSatisfiedBy_GivenLeftSpecificationEvalAsTrueAndRightSpecificationEvalAsTrue_EvalsBothSpecificationsAndReturnsTrue()
        {
            // Arrange
            var expectedInput = new object();

            var mockLeftSpecification = new Mock<ISpecification<object>>();
            mockLeftSpecification.Setup(x => x.IsSatisfiedBy(It.Is<object>(o => o == expectedInput)))
                .Returns(true);
            var mockRightSpecification = new Mock<ISpecification<object>>();
            mockRightSpecification.Setup(x => x.IsSatisfiedBy(It.Is<object>(o => o == expectedInput)))
                .Returns(true);

            var expectedLeftSpecification = mockLeftSpecification.Object;
            var expectedRightSpecification = mockRightSpecification.Object;

            var sut = new AndSpecification<object>(expectedLeftSpecification, expectedRightSpecification);

            // Act
            var actual = sut.IsSatisfiedBy(expectedInput);

            // Assert
            actual.Should().BeTrue();

            mockLeftSpecification.Verify(x => x.IsSatisfiedBy(It.Is<object>(o => o == expectedInput)), Times.Once());
            mockRightSpecification.Verify(x => x.IsSatisfiedBy(It.Is<object>(o => o == expectedInput)), Times.Once());
        }
    }
}