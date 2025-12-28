namespace Regulae.Tests.Builder.RulesBuilder
{
    using FluentAssertions;
    using Regulae.Builder;
    using Regulae.Builder.RulesBuilder;
    using Regulae.ConditionNodes;
    using Xunit;

    public class RootConditionNodeBuilderTests
    {
        [Fact]
        public void Value_CreatesValueConditionNode()
        {
            var builder = new RootConditionNodeBuilder();
            var node = builder.Value("Test", Operators.Equal, 5);

            node.Should().BeOfType<ValueConditionNode>();
            var valueNode = (ValueConditionNode)node;
            valueNode.Condition.Should().Be("Test");
            valueNode.Operator.Should().Be(Operators.Equal);
            valueNode.RightOperand.Value.Should().Be(5);
            valueNode.RightOperand.Cardinality.Should().Be(Cardinalities.One);
            valueNode.RightOperand.DataType.Should().Be(DataTypes.Integer);
        }

        [Fact]
        public void And_CreatesComposedConditionNode()
        {
            var builder = new RootConditionNodeBuilder();
            var node = builder.And(b => b.Value("A", Operators.Equal, 1));

            node.Should().BeOfType<ComposedConditionNode>();
            node.LogicalOperator.Should().Be(LogicalOperators.And);
        }

        [Fact]
        public void Or_CreatesComposedConditionNode()
        {
            var builder = new RootConditionNodeBuilder();
            var node = builder.Or(b => b.Value("B", Operators.NotEqual, 2));

            node.Should().BeOfType<ComposedConditionNode>();
            node.LogicalOperator.Should().Be(LogicalOperators.Or);
        }

        [Fact]
        public void Xor_CreatesComposedConditionNode()
        {
            var builder = new RootConditionNodeBuilder();
            var node = builder.Xor(b => b.Value("C", Operators.GreaterThan, 3));

            node.Should().BeOfType<ComposedConditionNode>();
            node.LogicalOperator.Should().Be(LogicalOperators.Xor);
        }

        [Fact]
        public void Condition_ReturnsGivenNode()
        {
            var builder = new RootConditionNodeBuilder();
            var valueNode = ConditionNodeFactory.CreateValueNode("D", Operators.LesserThan, 4);
            var node = builder.Condition(valueNode);

            node.Should().BeSameAs(valueNode);
        }
    }
}