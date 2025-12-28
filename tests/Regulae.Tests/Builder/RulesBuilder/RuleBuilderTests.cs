namespace Regulae.Tests.Builder.RulesBuilder
{
    using System;
    using FluentAssertions;
    using Regulae;
    using Regulae.Builder;
    using Regulae.Builder.RulesBuilder;
    using Regulae.ConditionNodes;
    using Xunit;

    public class RuleBuilderTests
    {
        [Fact]
        public void Build_ReturnsSuccess_WhenValid()
        {
            var builder = new RuleBuilder("TestRule")
                .InRuleset("TestRuleset")
                .SetContent("TestContent")
                .Since(DateTime.UtcNow)
                .ApplyWhen("Cond", Operators.Equal, 1);

            var result = builder.Build();

            result.IsSuccess.Should().BeTrue();
            result.Rule.Should().NotBeNull();
            result.Rule.Name.Should().Be("TestRule");
            result.Rule.Ruleset.Should().Be("TestRuleset");
        }

        [Fact]
        public void WithActive_SetsActiveFlag()
        {
            var builder = new RuleBuilder("TestRule")
                .InRuleset("TestRuleset")
                .SetContent("TestContent")
                .Since(DateTime.UtcNow)
                .WithActive(false)
                .ApplyWhen("Cond", Operators.Equal, 1);

            var result = builder.Build();

            result.Rule.Active.Should().BeFalse();
        }

        [Fact]
        public void Until_SetsDateEnd()
        {
            var dateEnd = DateTime.UtcNow.AddDays(1);
            var builder = new RuleBuilder("TestRule")
                .InRuleset("TestRuleset")
                .SetContent("TestContent")
                .Since(DateTime.UtcNow)
                .Until(dateEnd)
                .ApplyWhen("Cond", Operators.Equal, 1);

            var result = builder.Build();

            result.Rule.DateEnd.Should().Be(dateEnd);
        }

        [Fact]
        public void ApplyWhen_WithConditionNode_SetsRootCondition()
        {
            var builder = new RuleBuilder("TestRule")
                .InRuleset("TestRuleset")
                .SetContent("TestContent")
                .Since(DateTime.UtcNow);

            var condition = ConditionNodeFactory.CreateValueNode("Cond", Operators.Equal, 1);
            builder.ApplyWhen(condition);

            var result = builder.Build();

            result.Rule.RootCondition.Should().NotBeNull()
                .And.BeOfType<ValueConditionNode>();
            var valueNode = (ValueConditionNode)result.Rule.RootCondition;
            valueNode.Condition.Should().Be("Cond");
            valueNode.Operator.Should().Be(Operators.Equal);
            valueNode.RightOperand.Value.Should().Be(1);
            valueNode.RightOperand.Cardinality.Should().Be(Cardinalities.One);
            valueNode.RightOperand.DataType.Should().Be(DataTypes.Integer);
        }
    }
}