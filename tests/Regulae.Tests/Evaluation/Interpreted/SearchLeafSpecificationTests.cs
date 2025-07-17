namespace Regulae.Tests.Evaluation.Interpreted
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Moq;
    using Regulae.ConditionNodes;
    using Regulae.Evaluation.Interpreted;
    using Regulae.Evaluation.Interpreted.ValueEvaluation.Dispatchers;
    using Xunit;

    public class SearchLeafSpecificationTests
    {
        private readonly IConditionEvalDispatcher conditionEvalDispatcherMock;
        private readonly IConditionEvalDispatcherProvider conditionEvalDispatcherProviderMock;
        private readonly IValueConditionNode valueConditionNodeMock;

        public SearchLeafSpecificationTests()
        {
            this.conditionEvalDispatcherMock = Mock.Of<IConditionEvalDispatcher>();
            this.conditionEvalDispatcherProviderMock = Mock.Of<IConditionEvalDispatcherProvider>();
            this.valueConditionNodeMock = Mock.Of<IValueConditionNode>();
        }

        [Fact]
        public void And_GivenOtherSpecification_ReturnsAndSpecification()
        {
            // Arrange
            var mockOtherSpecification = new Mock<ISpecification<IDictionary<string, Operand>>>();
            var expectedOtherSpecification = mockOtherSpecification.Object;

            var sut = new SearchLeafSpecification(
                this.valueConditionNodeMock,
                this.conditionEvalDispatcherProviderMock);

            // Act
            var actual = sut.And(expectedOtherSpecification);

            // Assert
            actual.Should().BeOfType<AndSpecification<IDictionary<string, Operand>>>();
        }

        [Fact]
        public void IsSatisfiedBy_GivenConditionsWithNeededInputCondition_ReturnsConditionEvalDispatchResult()
        {
            // Arrange
            var expectedInput = new Dictionary<string, Operand>(StringComparer.Ordinal)
            {
                { "TestCondition", new Operand(0, DataTypes.Integer, Cardinalities.One) },
            };
            Mock.Get(this.valueConditionNodeMock)
                .SetupGet(x => x.RightOperand)
                .Returns(new Operand(1, DataTypes.Integer, Cardinalities.One));
            Mock.Get(this.valueConditionNodeMock)
                .SetupGet(x => x.Condition)
                .Returns("TestCondition");
            Mock.Get(this.conditionEvalDispatcherProviderMock)
                .Setup(x => x.GetEvalDispatcher(It.IsAny<Operand>(), It.IsAny<Operators>(), It.IsAny<Operand>()))
                .Returns(this.conditionEvalDispatcherMock);
            Mock.Get(this.conditionEvalDispatcherMock)
                .Setup(x => x.EvalDispatch(It.IsAny<Operand>(), It.IsAny<Operators>(), It.IsAny<Operand>()))
                .Returns(true);

            var sut = new SearchLeafSpecification(
                this.valueConditionNodeMock,
                this.conditionEvalDispatcherProviderMock);

            // Act
            var actual = sut.IsSatisfiedBy(expectedInput);

            // Assert
            actual.Should().BeTrue();
        }

        [Fact]
        public void IsSatisfiedBy_GivenConditionsWithoutNeededInputCondition_ReturnsTrue()
        {
            // Arrange
            var expectedInput = new Dictionary<string, Operand>(StringComparer.Ordinal);
            Mock.Get(this.valueConditionNodeMock)
                .SetupGet(x => x.RightOperand)
                .Returns(new Operand(1, DataTypes.Integer, Cardinalities.One));
            Mock.Get(this.valueConditionNodeMock)
                .SetupGet(x => x.Condition)
                .Returns("TestCondition");
            Mock.Get(this.conditionEvalDispatcherProviderMock)
                .Setup(x => x.GetEvalDispatcher(It.IsAny<Operand>(), It.IsAny<Operators>(), It.IsAny<Operand>()))
                .Returns(this.conditionEvalDispatcherMock);
            Mock.Get(this.conditionEvalDispatcherMock)
                .Setup(x => x.EvalDispatch(It.IsAny<Operand>(), It.IsAny<Operators>(), It.IsAny<Operand>()))
                .Returns(true);

            var sut = new SearchLeafSpecification(
                this.valueConditionNodeMock,
                this.conditionEvalDispatcherProviderMock);

            // Act
            var actual = sut.IsSatisfiedBy(expectedInput);

            // Assert
            actual.Should().BeTrue();
        }

        [Fact]
        public void Or_GivenOtherSpecification_ReturnsOrSpecification()
        {
            // Arrange
            var mockOtherSpecification = new Mock<ISpecification<IDictionary<string, Operand>>>();
            var expectedOtherSpecification = mockOtherSpecification.Object;

            var sut = new MatchWithDiscardOnMissingConditionLeafSpecification(
                this.valueConditionNodeMock,
                this.conditionEvalDispatcherProviderMock);

            // Act
            var actual = sut.Or(expectedOtherSpecification);

            // Assert
            actual.Should().BeOfType<OrSpecification<IDictionary<string, Operand>>>();
        }
    }
}