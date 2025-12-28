namespace Regulae.Tests.Builder.RulesBuilder
{
    using System.Linq;
    using FluentAssertions;
    using Regulae.Builder;
    using Regulae.Builder.RulesBuilder;
    using Regulae.ConditionNodes;
    using Xunit;

    public class FluentConditionNodeBuilderTests
    {
        [Fact]
        public void Value_AddsValueConditionNode()
        {
            var builder = new FluentConditionNodeBuilder(LogicalOperators.And);
            builder.Value("TestCondition", Operators.Equal, 42);
            var node = builder.Build();

            node.Should().BeOfType<ComposedConditionNode>();
            var composedNode = (ComposedConditionNode)node;
            composedNode.ChildConditionNodes.Should().ContainSingle(c => c is ValueConditionNode);
            var valueNode = (ValueConditionNode)composedNode.ChildConditionNodes.First();
            valueNode.Condition.Should().Be("TestCondition");
            valueNode.Operator.Should().Be(Operators.Equal);
            valueNode.RightOperand.Value.Should().Be(42);
            valueNode.RightOperand.Cardinality.Should().Be(Cardinalities.One);
            valueNode.RightOperand.DataType.Should().Be(DataTypes.Integer);
        }

        [Fact]
        public void And_AddsComposedConditionNode()
        {
            var builder = new FluentConditionNodeBuilder(LogicalOperators.And);
            builder.And(b => b.Value("A", Operators.Equal, 1));
            var node = builder.Build();

            node.Should().BeOfType<ComposedConditionNode>();
            var composed = (ComposedConditionNode)node;
            composed.ChildConditionNodes.Should().ContainSingle()
                .Which.Should().BeOfType<ComposedConditionNode>();
        }

        [Fact]
        public void Or_AddsComposedConditionNode()
        {
            var builder = new FluentConditionNodeBuilder(LogicalOperators.Or);
            builder.Or(b => b.Value("B", Operators.NotEqual, 2));
            var node = builder.Build();

            node.Should().BeOfType<ComposedConditionNode>();
            var composed = (ComposedConditionNode)node;
            composed.ChildConditionNodes.Should().ContainSingle()
                .Which.Should().BeOfType<ComposedConditionNode>();
        }

        [Fact]
        public void Xor_AddsComposedConditionNode()
        {
            var builder = new FluentConditionNodeBuilder(LogicalOperators.Xor);
            builder.Xor(b => b.Value("C", Operators.GreaterThan, 3));
            var node = builder.Build();

            node.Should().BeOfType<ComposedConditionNode>();
            var composed = (ComposedConditionNode)node;
            composed.ChildConditionNodes.Should().ContainSingle()
                .Which.Should().BeOfType<ComposedConditionNode>();
        }

        [Fact]
        public void Condition_AddsGivenConditionNode()
        {
            var builder = new FluentConditionNodeBuilder(LogicalOperators.And);
            var valueNode = ConditionNodeFactory.CreateValueNode("D", Operators.LesserThan, 4);
            builder.Condition(valueNode);
            var node = builder.Build();

            node.Should().BeOfType<ComposedConditionNode>();
            var composed = (ComposedConditionNode)node;
            composed.ChildConditionNodes.Should().ContainSingle()
                .Which.Should().BeSameAs(valueNode);
        }
    }
}