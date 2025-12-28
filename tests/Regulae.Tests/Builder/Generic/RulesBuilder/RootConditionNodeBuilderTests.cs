namespace Regulae.Tests.Builder.Generic.RulesBuilder
{
    using FluentAssertions;
    using Moq;
    using Regulae;
    using Regulae.Builder.Generic.RulesBuilder;
    using Regulae.ConditionNodes;
    using Xunit;

    public class RootConditionNodeBuilderTests
    {
        private enum TestCondition { ConditionA, ConditionB }

        [Fact]
        public void And_CallsFactoryWithAndOperator()
        {
            // Arrange
            var builder = new RootConditionNodeBuilder<TestCondition>();
            var called = false;

            // Act
            var node = builder.And(b =>
            {
                called = true;
                return b;
            });

            // Assert
            node.Should().NotBeNull()
                .And.BeOfType<ComposedConditionNode>()
                .And.Subject.As<ComposedConditionNode>()
                .LogicalOperator.Should().Be(LogicalOperators.And);
            called.Should().BeTrue();
        }

        [Fact]
        public void Or_CallsFactoryWithOrOperator()
        {
            // Arrange
            var builder = new RootConditionNodeBuilder<TestCondition>();
            var called = false;

            // Act
            var node = builder.Or(b =>
            {
                called = true; return b;
            });

            // Assert
            node.Should().NotBeNull()
                .And.BeOfType<ComposedConditionNode>()
                .And.Subject.As<ComposedConditionNode>()
                .LogicalOperator.Should().Be(LogicalOperators.Or);
            called.Should().BeTrue();
        }

        [Fact]
        public void Xor_CallsFactoryWithXorOperator()
        {
            // Arrange
            var builder = new RootConditionNodeBuilder<TestCondition>();
            var called = false;

            // Act
            var node = builder.Xor(b =>
            {
                called = true; return b;
            });

            // Assert
            node.Should().NotBeNull()
                .And.BeOfType<ComposedConditionNode>()
                .And.Subject.As<ComposedConditionNode>()
                .LogicalOperator.Should().Be(LogicalOperators.Xor);
            called.Should().BeTrue();
        }

        [Fact]
        public void Condition_ReturnsPassedNode()
        {
            // Arrange
            var builder = new RootConditionNodeBuilder<TestCondition>();
            var mockNode = Mock.Of<IConditionNode>();

            // Act
            var result = builder.Condition(mockNode);

            // Assert
            result.Should().BeSameAs(mockNode);
        }

        [Fact]
        public void Value_CreatesValueNode()
        {
            // Arrange
            var builder = new RootConditionNodeBuilder<TestCondition>();
            var condition = TestCondition.ConditionA;
            var operand = 42;

            // Act
            var node = builder.Value(condition, Operators.Equal, operand);

            // Assert
            node.Should().NotBeNull()
                .And.BeOfType<ValueConditionNode>();
            node.LogicalOperator.Should().Be(LogicalOperators.Eval);
            var valueNode = node as ValueConditionNode;
            valueNode.Condition.Should().Be(condition.ToString());
            valueNode.Operator.Should().Be(Operators.Equal);
            valueNode.RightOperand.Value.Should().Be(operand);
        }
    }
}
