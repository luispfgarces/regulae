namespace Regulae.Tests.Builder.Generic.RulesBuilder
{
    using FluentAssertions;
    using Moq;
    using Regulae;
    using Regulae.Builder.Generic.RulesBuilder;
    using Regulae.ConditionNodes;
    using Xunit;

    public class FluentConditionNodeBuilderTests
    {
        private enum TestCondition { ConditionA, ConditionB }

        [Fact]
        public void Build_ShouldReturnComposedConditionNode_WithCorrectLogicalOperatorAndConditions()
        {
            // Arrange
            var builder = new FluentConditionNodeBuilder<TestCondition>(LogicalOperators.And);
            var node1 = Mock.Of<IConditionNode>();
            var node2 = Mock.Of<IConditionNode>();

            builder.Condition(node1).Condition(node2);

            // Act
            var result = builder.Build();

            // Assert
            result.Should().BeOfType<ComposedConditionNode>();
            result.LogicalOperator.Should().Be(LogicalOperators.And);
            var composedNode = result as ComposedConditionNode;
            composedNode.ChildConditionNodes.Should().ContainInOrder(node1, node2);
        }

        [Fact]
        public void And_ShouldAddComposedConditionNode_WithAndOperator()
        {
            // Arrange
            var builder = new FluentConditionNodeBuilder<TestCondition>(LogicalOperators.Or);

            // Act
            builder.And(b => b.Value(TestCondition.ConditionA, Operators.Equal, 1));

            var result = builder.Build();

            // Assert
            var composedNode = result as ComposedConditionNode;
            composedNode.ChildConditionNodes.Should().ContainSingle(node =>
                    node.LogicalOperator == LogicalOperators.And);
        }

        [Fact]
        public void Or_ShouldAddComposedConditionNode_WithOrOperator()
        {
            // Arrange
            var builder = new FluentConditionNodeBuilder<TestCondition>(LogicalOperators.And);

            // Act
            builder.Or(b => b.Value(TestCondition.ConditionB, Operators.NotEqual, 2));

            var result = builder.Build();

            // Assert
            var composedNode = result as ComposedConditionNode;
            composedNode.ChildConditionNodes.Should().ContainSingle(node =>
                    node.LogicalOperator == LogicalOperators.Or);
        }

        [Fact]
        public void Xor_ShouldAddComposedConditionNode_WithXorOperator()
        {
            // Arrange
            var builder = new FluentConditionNodeBuilder<TestCondition>(LogicalOperators.And);

            // Act
            builder.Xor(b => b.Value(TestCondition.ConditionA, Operators.Equal, 3));

            var result = builder.Build();

            // Assert
            var composedNode = result as ComposedConditionNode;
            composedNode.ChildConditionNodes.Should().ContainSingle(node =>
                    node.LogicalOperator == LogicalOperators.Xor);
        }

        [Fact]
        public void Value_ShouldAddValueConditionNode()
        {
            // Arrange
            var builder = new FluentConditionNodeBuilder<TestCondition>(LogicalOperators.And);

            // Act
            builder.Value(TestCondition.ConditionA, Operators.GreaterThan, 5);

            var result = builder.Build();

            // Assert
            var composedNode = result as ComposedConditionNode;
            composedNode.ChildConditionNodes.Should().ContainSingle(node =>
                    node.LogicalOperator == LogicalOperators.Eval);
        }

        [Fact]
        public void Condition_ShouldAddGivenConditionNode()
        {
            // Arrange
            var builder = new FluentConditionNodeBuilder<TestCondition>(LogicalOperators.And);
            var node = Mock.Of<IConditionNode>();

            // Act
            builder.Condition(node);
            var result = builder.Build();

            // Assert
            result.Should().NotBeNull()
                .And.BeOfType<ComposedConditionNode>()
                .And.Subject.As<ComposedConditionNode>()
                .ChildConditionNodes.Should().Contain(node);
        }
    }
}
