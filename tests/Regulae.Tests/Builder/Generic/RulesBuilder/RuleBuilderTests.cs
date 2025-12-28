namespace Regulae.Tests.Builder.Generic.RulesBuilder
{
    using System;
    using FluentAssertions;
    using Moq;
    using Regulae;
    using Regulae.Builder.Generic.RulesBuilder;
    using Regulae.ConditionNodes;
    using Regulae.Generic.ConditionNodes;
    using Regulae.Serialization;
    using Regulae.Tests.TestStubs;
    using Xunit;

    public class RuleBuilderTests
    {
        [Fact]
        public void NewRule_GivenRuleWithComposedCondition_BuildsAndReturnsRule()
        {
            // Arrange
            var ruleName = "Rule 1";
            var dateBegin = DateTime.Parse("2021-01-01");
            var ruleset = RulesetNames.Type1;
            var content = "Content";
            var rootCondition = new ValueConditionNode(ConditionNames.IsoCurrency.ToString(), Operators.Equal, "EUR");

            // Act
            var ruleBuilderResult = Rule.Create<RulesetNames, ConditionNames>(ruleName)
                .InRuleset(ruleset)
                .SetContent(content)
                .Since(dateBegin)
                .ApplyWhen(c =>
                {
                    return rootCondition;
                })
                .Build();

            // Assert
            ruleBuilderResult.Should().NotBeNull();
            ruleBuilderResult.IsSuccess.Should().BeTrue();

            var rule = ruleBuilderResult.Rule;
            rule.Name.Should().Be(ruleName);
            rule.DateBegin.Should().Be(dateBegin);
            rule.DateEnd.Should().BeNull();
            rule.ContentContainer.Should().NotBeNull();
            rule.RootCondition.Should().NotBeNull()
                .And.BeOfType<ValueConditionNode<ConditionNames>>();

            var valueConditionNode = rule.RootCondition as ValueConditionNode<ConditionNames>;
            valueConditionNode.Condition.Should().Be(ConditionNames.IsoCurrency);
            valueConditionNode.Operator.Should().Be(Operators.Equal);
            var rightOperand = valueConditionNode.RightOperand;
            rightOperand.Value.Should().Be("EUR");
            rightOperand.DataType.Should().Be(DataTypes.String);
            rightOperand.Cardinality.Should().Be(Cardinalities.One);
        }

        [Theory]
        [InlineData(nameof(ConditionNames.NumberOfSales), Operators.Contains, 40)]
        [InlineData(nameof(ConditionNames.NumberOfSales), Operators.NotContains, 40)]
        [InlineData("", Operators.Equal, 40)]
        public void NewRule_GivenRuleWithIntegerConditionAndContainsOperator_ReturnsInvalidRuleResult(
            string conditionName,
            Operators containsOperator,
            object rightOperand)
        {
            // Arrange
            var ruleName = "Rule 1";
            var dateBegin = DateTime.Parse("2021-01-01");
            var ruleset = RulesetNames.Type1;
            var content = "Content";
            var conditionOperator = containsOperator;
            var condition = string.IsNullOrEmpty(conditionName) ? 0 : Enum.Parse<ConditionNames>(conditionName);

            // Act
            var ruleBuilderResult = Rule.Create<RulesetNames, ConditionNames>(ruleName)
                .InRuleset(ruleset)
                .SetContent(content)
                .Since(dateBegin)
                .ApplyWhen(condition, conditionOperator, rightOperand)
                .Build();

            // Assert
            ruleBuilderResult.Should().NotBeNull();
            ruleBuilderResult.IsSuccess.Should().BeFalse();
            ruleBuilderResult.Rule.Should().BeNull();

            ruleBuilderResult.Errors.Should().NotBeNull().And.NotBeEmpty();
        }

        [Theory]
        [InlineData(Operators.Contains)]
        [InlineData(Operators.NotContains)]
        public void NewRule_GivenRuleWithStringConditionAndContainsOperator_BuildsAndReturnsRule(Operators containsOperator)
        {
            // Arrange
            var ruleName = "Rule 1";
            var dateBegin = DateTime.Parse("2021-01-01");
            var ruleset = RulesetNames.Type1;
            var content = "Content";
            const ConditionNames condition = ConditionNames.IsoCountryCode;
            const string conditionValue = "PT";
            var conditionOperator = containsOperator;
            const LogicalOperators logicalOperator = LogicalOperators.Eval;
            const DataTypes dataType = DataTypes.String;
            const Cardinalities cardinality = Cardinalities.One;

            // Act
            var ruleBuilderResult = Rule.Create<RulesetNames, ConditionNames>(ruleName)
                .InRuleset(ruleset)
                .SetContent(content)
                .Since(dateBegin)
                .ApplyWhen(c => c.Value(condition, conditionOperator, conditionValue))
                .Build();

            // Assert
            ruleBuilderResult.Should().NotBeNull();
            ruleBuilderResult.IsSuccess.Should().BeTrue();
            ruleBuilderResult.Rule.Should().NotBeNull();

            var rule = ruleBuilderResult.Rule;

            rule.Name.Should().Be(ruleName);
            rule.DateBegin.Should().Be(dateBegin);
            rule.DateEnd.Should().BeNull();
            rule.ContentContainer.Should().NotBeNull();
            rule.RootCondition.Should().NotBeNull();
            rule.RootCondition.Should().BeAssignableTo<IValueConditionNode<ConditionNames>>();

            var rootCondition = rule.RootCondition as IValueConditionNode<ConditionNames>;
            rootCondition.Condition.Should().Be(condition);
            rootCondition.RightOperand.DataType.Should().Be(dataType);
            rootCondition.RightOperand.Cardinality.Should().Be(cardinality);
            rootCondition.LogicalOperator.Should().Be(logicalOperator);
            rootCondition.Operator.Should().Be(conditionOperator);
        }

        // TODO create test for WithCondition() with composed condition

        [Fact]
        public void NewRule_WithSerializedContent_GivenNullContentSerializationProvider_ThrowsArgumentNullException()
        {
            // Arrange
            var ruleBuilder = new RuleBuilder<RulesetNames, ConditionNames>("My rule used for testing purposes");
            IContentSerializationProvider contentSerializationProvider = null;

            // Act
            var argumentNullException = Assert
                .Throws<ArgumentNullException>(() => ruleBuilder.SetContent(new object(), contentSerializationProvider));

            // Assert
            argumentNullException.Should().NotBeNull();
            argumentNullException.ParamName.Should().Be(nameof(contentSerializationProvider));
        }

        [Fact]
        public void NewRule_WithSerializedContent_SetsContentAsSerializedContent()
        {
            // Arrange
            var ruleName = "Rule 1";
            var dateBegin = DateTime.Parse("2021-01-01");
            var ruleset = RulesetNames.Type1;
            var content = "TEST";

            var contentSerializer = Mock.Of<IContentSerializer>();
            Mock.Get(contentSerializer)
                .Setup(x => x.Deserialize(It.IsAny<object>(), It.IsAny<Type>()))
                .Returns(content);

            var contentSerializationProvider = Mock.Of<IContentSerializationProvider>();
            Mock.Get(contentSerializationProvider)
                .Setup(x => x.GetContentSerializer(ruleset.ToString()))
                .Returns(contentSerializer);

            // Act
            var ruleBuilderResult = Rule.Create<RulesetNames, ConditionNames>(ruleName)
                .InRuleset(ruleset)
                .SetContent(content, contentSerializationProvider)
                .Since(dateBegin)
                .Build();

            // Assert
            ruleBuilderResult.Rule.Ruleset.Should().Be(ruleset);
            var ruleContent = ruleBuilderResult.Rule.ContentContainer;
            ruleContent.Should().NotBeNull().And.BeOfType<SerializedContentContainer>();
            ruleContent.GetContentAs<string>().Should().Be(content);
        }

        [Fact]
        public void WithActive_SetsActiveFlagOnRule()
        {
            // Arrange
            var builder = new RuleBuilder<RulesetNames, ConditionNames>("TestRule")
                .InRuleset(RulesetNames.Type1)
                .SetContent("test content")
                .Since(DateTime.UtcNow);

            // Act
            var result = builder.WithActive(false).Build();

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Rule.Should().NotBeNull();
            result.Rule!.Active.Should().BeFalse();
        }
    }
}