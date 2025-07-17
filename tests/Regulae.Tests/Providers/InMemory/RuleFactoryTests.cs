namespace Regulae.Tests.Providers.InMemory
{
    using System;
    using System.Dynamic;
    using System.Linq;
    using FluentAssertions;
    using Regulae;
    using Regulae.Builder;
    using Regulae.ConditionNodes;
    using Regulae.Core;
    using Regulae.Providers.InMemory;
    using Regulae.Providers.InMemory.DataModel;
    using Regulae.Tests.Providers.InMemory.TestStubs;
    using Xunit;

    public class RuleFactoryTests
    {
        [Fact]
        public void CreateRule_GivenNullRule_ThrowsArgumentNullException()
        {
            // Arrange
            Rule rule = null;

            var ruleFactory = new RuleFactory();

            // Act
            var argumentNullException = Assert.Throws<ArgumentNullException>(() => ruleFactory.CreateRule(rule));

            // Assert
            argumentNullException.Should().NotBeNull();
            argumentNullException.ParamName.Should().Be(nameof(rule));
        }

        [Fact]
        public void CreateRule_GivenNullRuleDataModel_ThrowsArgumentNullException()
        {
            // Arrange
            RuleDataModel ruleDataModel = null;

            var ruleFactory = new RuleFactory();

            // Act
            var argumentNullException = Assert.Throws<ArgumentNullException>(() => ruleFactory.CreateRule(ruleDataModel));

            // Assert
            argumentNullException.Should().NotBeNull();
            argumentNullException.ParamName.Should().Be(nameof(ruleDataModel));
        }

        [Fact]
        public void CreateRule_GivenRuleDataModelWithComposedNodeAndChildNodesOfEachDataType_ReturnsRuleInstance()
        {
            // Arrange
            dynamic content = new ExpandoObject();
            content.Prop1 = 123;
            content.Prop2 = "Sample string";
            content.Prop3 = 500.34m;

            var integerConditionNodeDataModel = new ValueConditionNodeDataModel
            {
                Condition = ConditionNames.SampleIntegerCondition.ToString(),
                LogicalOperator = LogicalOperators.Eval,
                RightOperand = 20,
                Operator = Operators.GreaterThan,
                Properties = new PropertiesDictionary(2),
            };

            var stringConditionNodeDataModel = new ValueConditionNodeDataModel
            {
                Condition = ConditionNames.SampleStringCondition.ToString(),
                LogicalOperator = LogicalOperators.Eval,
                RightOperand = "TEST",
                Operator = Operators.Equal,
                Properties = new PropertiesDictionary(2),
            };

            var decimalConditionNodeDataModel = new ValueConditionNodeDataModel
            {
                Condition = ConditionNames.SampleDecimalCondition.ToString(),
                LogicalOperator = LogicalOperators.Eval,
                RightOperand = 50.3m,
                Operator = Operators.LesserThanOrEqual,
                Properties = new PropertiesDictionary(2),
            };

            var booleanConditionNodeDataModel = new ValueConditionNodeDataModel
            {
                Condition = ConditionNames.SampleBooleanCondition.ToString(),
                LogicalOperator = LogicalOperators.Eval,
                RightOperand = true,
                Operator = Operators.NotEqual,
                Properties = new PropertiesDictionary(2),
            };

            var ruleDataModel = new RuleDataModel
            {
                Content = content,
                Ruleset = RulesetNames.RulesetSample.ToString(),
                DateBegin = new DateTime(2020, 1, 1),
                DateEnd = null,
                Name = "My rule used for testing purposes",
                Priority = 1,
                RootCondition = new ComposedConditionNodeDataModel
                {
                    LogicalOperator = LogicalOperators.And,
                    ChildConditionNodes = new ConditionNodeDataModel[]
                    {
                        integerConditionNodeDataModel,
                        stringConditionNodeDataModel,
                        decimalConditionNodeDataModel,
                        booleanConditionNodeDataModel
                    },
                    Properties = new PropertiesDictionary(2),
                }
            };

            var ruleFactory = new RuleFactory();

            // Act
            var rule = ruleFactory.CreateRule(ruleDataModel);

            // Assert
            rule.Should().NotBeNull();
            rule.ContentContainer.Should().NotBeNull()
                .And.BeOfType<ObjectContentContainer>();
            rule.DateBegin.Should().Be(ruleDataModel.DateBegin);
            rule.DateEnd.Should().BeNull();
            rule.Name.Should().Be(ruleDataModel.Name);
            rule.Priority.Should().Be(ruleDataModel.Priority);
            rule.Ruleset.Should().Be(ruleDataModel.Ruleset);
            rule.RootCondition.Should().BeOfType<ComposedConditionNode>();

            var composedConditionNode = rule.RootCondition.As<ComposedConditionNode>();
            composedConditionNode.LogicalOperator.Should().Be(LogicalOperators.And);
            composedConditionNode.ChildConditionNodes.Should().HaveCount(4);

            var valueConditionNodes = composedConditionNode.ChildConditionNodes.OfType<ValueConditionNode>();
            valueConditionNodes.Should().HaveCount(4);
            var integerConditionNode = valueConditionNodes.First(x => x.RightOperand.DataType == DataTypes.Integer);
            integerConditionNode.Should().NotBeNull();
            integerConditionNode.Condition.Should().Match(x => x == integerConditionNodeDataModel.Condition);
            integerConditionNode.LogicalOperator.Should().Be(integerConditionNodeDataModel.LogicalOperator);
            integerConditionNode.RightOperand.Should().BeEquivalentTo(integerConditionNodeDataModel.RightOperand);
            integerConditionNode.Operator.Should().Be(integerConditionNodeDataModel.Operator);

            var stringConditionNode = valueConditionNodes.First(x => x.RightOperand.DataType == DataTypes.String);
            stringConditionNode.Should().NotBeNull();
            stringConditionNode.Condition.Should().Match(x => x == stringConditionNodeDataModel.Condition);
            stringConditionNode.LogicalOperator.Should().Be(stringConditionNodeDataModel.LogicalOperator);
            stringConditionNode.RightOperand.Should().BeEquivalentTo(stringConditionNodeDataModel.RightOperand);
            stringConditionNode.Operator.Should().Be(stringConditionNodeDataModel.Operator);

            var decimalConditionNode = valueConditionNodes.First(x => x.RightOperand.DataType == DataTypes.Decimal);
            decimalConditionNode.Should().NotBeNull();
            decimalConditionNode.Condition.Should().Match(x => x == decimalConditionNodeDataModel.Condition);
            decimalConditionNode.LogicalOperator.Should().Be(decimalConditionNodeDataModel.LogicalOperator);
            decimalConditionNode.RightOperand.Should().BeEquivalentTo(decimalConditionNodeDataModel.RightOperand);
            decimalConditionNode.Operator.Should().Be(decimalConditionNodeDataModel.Operator);

            var booleanConditionNode = valueConditionNodes.First(x => x.RightOperand.DataType == DataTypes.Boolean);
            booleanConditionNode.Should().NotBeNull();
            booleanConditionNode.Condition.Should().Match(x => x == booleanConditionNodeDataModel.Condition);
            booleanConditionNode.LogicalOperator.Should().Be(booleanConditionNodeDataModel.LogicalOperator);
            booleanConditionNode.RightOperand.Should().BeEquivalentTo(booleanConditionNodeDataModel.RightOperand);
            booleanConditionNode.Operator.Should().Be(booleanConditionNodeDataModel.Operator);
        }

        [Fact]
        public void CreateRule_GivenRuleWithComposedNodeAndChildNodesOfEachDataType_ReturnsRuleDataModelInstance()
        {
            // Arrange
            dynamic content = new ExpandoObject();
            content.Prop1 = 123;
            content.Prop2 = "Sample string";
            content.Prop3 = 500.34m;

            var booleanConditionNode = ConditionNodeFactory
                .CreateValueNode(ConditionNames.SampleBooleanCondition.ToString(), Operators.NotEqual, true) as ValueConditionNode;
            var decimalConditionNode = ConditionNodeFactory
                .CreateValueNode(ConditionNames.SampleDecimalCondition.ToString(), Operators.LesserThanOrEqual, 50.3m) as ValueConditionNode;
            var integerConditionNode = ConditionNodeFactory
                .CreateValueNode(ConditionNames.SampleIntegerCondition.ToString(), Operators.GreaterThan, 20) as ValueConditionNode;
            var stringConditionNode = ConditionNodeFactory
                .CreateValueNode(ConditionNames.SampleStringCondition.ToString(), Operators.Equal, "TEST") as ValueConditionNode;

            var rule1 = Rule.Create<RulesetNames, ConditionNames>("My rule used for testing purposes")
                .InRuleset(RulesetNames.RulesetSample)
                .SetContent((object)content)
                .Since(new DateTime(2020, 1, 1))
                .ApplyWhen(c => c
                    .And(a => a
                        .Condition(booleanConditionNode)
                        .Condition(decimalConditionNode)
                        .Condition(integerConditionNode)
                        .Condition(stringConditionNode)
                    )
                )
                .Build().Rule;

            var ruleFactory = new RuleFactory();

            // Act
            var rule = ruleFactory.CreateRule(rule1);

            // Assert
            rule.Should().NotBeNull();
            var content1 = rule.Content;
            content1.Should().NotBeNull()
                .And.BeSameAs(content);
            rule.DateBegin.Should().Be(rule.DateBegin);
            rule.DateEnd.Should().BeNull();
            rule.Name.Should().Be(rule.Name);
            rule.Priority.Should().Be(rule.Priority);
            rule.Ruleset.Should().Be(rule.Ruleset);
            rule.RootCondition.Should().BeOfType<ComposedConditionNodeDataModel>();

            var composedConditionNodeDataModel = rule.RootCondition.As<ComposedConditionNodeDataModel>();
            composedConditionNodeDataModel.LogicalOperator.Should().Be(LogicalOperators.And);
            composedConditionNodeDataModel.ChildConditionNodes.Should().HaveCount(4);

            var valueConditionNodeDataModels = composedConditionNodeDataModel.ChildConditionNodes.OfType<ValueConditionNodeDataModel>();
            valueConditionNodeDataModels.Should().HaveCount(4);
            var integerConditionNodeDataModel = valueConditionNodeDataModels.First(v => v.RightOperand.DataType == DataTypes.Integer);
            integerConditionNodeDataModel.Should().NotBeNull();
            integerConditionNodeDataModel.Condition.Should().Match(x => integerConditionNode.Condition == x);
            integerConditionNodeDataModel.LogicalOperator.Should().Be(integerConditionNode.LogicalOperator);
            integerConditionNodeDataModel.RightOperand.Should().BeEquivalentTo(integerConditionNode.RightOperand);
            integerConditionNodeDataModel.Operator.Should().Be(integerConditionNode.Operator);

            var stringConditionNodeDataModel = valueConditionNodeDataModels.First(v => v.RightOperand.DataType == DataTypes.String);
            stringConditionNodeDataModel.Should().NotBeNull();
            stringConditionNodeDataModel.Condition.Should().Match(x => stringConditionNode.Condition == x);
            stringConditionNodeDataModel.LogicalOperator.Should().Be(stringConditionNode.LogicalOperator);
            stringConditionNodeDataModel.RightOperand.Should().BeEquivalentTo(stringConditionNode.RightOperand);
            stringConditionNodeDataModel.Operator.Should().Be(stringConditionNode.Operator);

            var decimalConditionNodeDataModel = valueConditionNodeDataModels.First(v => v.RightOperand.DataType == DataTypes.Decimal);
            decimalConditionNodeDataModel.Should().NotBeNull();
            decimalConditionNodeDataModel.Condition.Should().Match(x => decimalConditionNode.Condition == x);
            decimalConditionNodeDataModel.LogicalOperator.Should().Be(decimalConditionNode.LogicalOperator);
            decimalConditionNodeDataModel.RightOperand.Should().BeEquivalentTo(decimalConditionNode.RightOperand);
            decimalConditionNodeDataModel.Operator.Should().Be(decimalConditionNode.Operator);

            var booleanConditionNodeDataModel = valueConditionNodeDataModels.First(v => v.RightOperand.DataType == DataTypes.Boolean);
            booleanConditionNodeDataModel.Should().NotBeNull();
            booleanConditionNodeDataModel.Condition.Should().Match(x => booleanConditionNode.Condition == x);
            booleanConditionNodeDataModel.LogicalOperator.Should().Be(booleanConditionNode.LogicalOperator);
            booleanConditionNodeDataModel.RightOperand.Should().BeEquivalentTo(booleanConditionNode.RightOperand);
            booleanConditionNodeDataModel.Operator.Should().Be(booleanConditionNode.Operator);
        }
    }
}