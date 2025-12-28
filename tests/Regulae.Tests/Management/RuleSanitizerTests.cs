namespace Regulae.Tests.Management
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using Regulae;
    using Regulae.ConditionNodes;
    using Regulae.Evaluation;
    using Regulae.Management;
    using Regulae.Source;
    using Xunit;

    public class RuleSanitizerTests
    {
        [Fact]
        public async Task SanitizeAsync_ReturnsSuccess_WhenNoRootCondition()
        {
            // Arrange
            var rulesSource = new Mock<IRulesSource>();
            var dataTypeProvider = new DataTypesConfigurationProvider(RulesEngineOptions.NewWithDefaults());
            var sanitizer = new RuleSanitizer(rulesSource.Object, dataTypeProvider);

            var rule = Rule.Create("r").InRuleset("rs").SetContent("c").Since(DateTime.UtcNow).Build().Rule;
            rule.RootCondition = null;

            // Act
            var result = await sanitizer.SanitizeAsync(rule);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task SanitizeAsync_Throws_WhenConditionNotFound()
        {
            // Arrange
            var rulesSource = new Mock<IRulesSource>();
            rulesSource.Setup(x => x.GetConditionsAsync(It.IsAny<GetConditionsArgs>())).ReturnsAsync(new Dictionary<string, Condition>());
            var dataTypeProvider = new DataTypesConfigurationProvider(RulesEngineOptions.NewWithDefaults());
            var sanitizer = new RuleSanitizer(rulesSource.Object, dataTypeProvider);

            var rule = Rule.Create("r").InRuleset("rs").SetContent("c").Since(DateTime.UtcNow).ApplyWhen("missing", Operators.Equal, "val").Build().Rule;

            // Act
            Func<Task> act = async () => await sanitizer.SanitizeAsync(rule);

            // Assert
            var exception = await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
            exception.WithMessage("The given condition with name 'missing' does not exist. Please create the condition before using it to evaluate rules.*");
            exception.Which.ParamName.Should().Be("conditions");
        }

        [Fact]
        public async Task SanitizeAsync_ConvertsStringToTargetDataType()
        {
            // Arrange
            var rulesSource = new Mock<IRulesSource>();
            var conditionName = "cond1";
            var conditions = new Dictionary<string, Condition>
            {
                { conditionName, new Condition(conditionName, DateTime.UtcNow, DataTypes.Integer) }
            };
            rulesSource.Setup(x => x.GetConditionsAsync(It.IsAny<GetConditionsArgs>())).ReturnsAsync(conditions);

            var dataTypeProvider = new DataTypesConfigurationProvider(RulesEngineOptions.NewWithDefaults());
            var sanitizer = new RuleSanitizer(rulesSource.Object, dataTypeProvider);

            var rule = Rule.Create("r").InRuleset("rs").SetContent("c").Since(DateTime.UtcNow).ApplyWhen(conditionName, Operators.Equal, "123").Build().Rule;

            // Act
            var result = await sanitizer.SanitizeAsync(rule);

            // Assert
            result.IsSuccess.Should().BeTrue();
            var valueCond = (ValueConditionNode)rule.RootCondition!;
            valueCond.RightOperand.DataType.Should().Be(DataTypes.Integer);
            valueCond.RightOperand.Value.Should().Be(123);
        }

        [Fact]
        public async Task SanitizeAsync_TraversesComposedCondition_ConvertsChildren()
        {
            // Arrange
            var rulesSource = new Mock<IRulesSource>();
            var condA = "a";
            var condB = "b";
            var conditions = new Dictionary<string, Condition>
            {
                { condA, new Condition(condA, DateTime.UtcNow, DataTypes.Integer) },
                { condB, new Condition(condB, DateTime.UtcNow, DataTypes.Integer) },
            };
            rulesSource.Setup(x => x.GetConditionsAsync(It.IsAny<GetConditionsArgs>())).ReturnsAsync(conditions);

            var dataTypeProvider = new DataTypesConfigurationProvider(RulesEngineOptions.NewWithDefaults());
            var sanitizer = new RuleSanitizer(rulesSource.Object, dataTypeProvider);

            var child1 = new ValueConditionNode(condA, Operators.Equal, new Operand("1", DataTypes.String, Cardinalities.One));
            var child2 = new ValueConditionNode(condB, Operators.Equal, new Operand("2", DataTypes.String, Cardinalities.One));
            var composed = new ComposedConditionNode(LogicalOperators.And, [child1, child2]);

            var rule = Rule.Create("r").InRuleset("rs").SetContent("c").Since(DateTime.UtcNow).Build().Rule;
            rule.RootCondition = composed;

            // Act
            var result = await sanitizer.SanitizeAsync(rule);

            // Assert
            result.IsSuccess.Should().BeTrue();
            var composedAfter = (ComposedConditionNode)rule.RootCondition!;
            var first = (ValueConditionNode)composedAfter.ChildConditionNodes.First();
            var second = (ValueConditionNode)composedAfter.ChildConditionNodes.Skip(1).First();
            first.RightOperand.DataType.Should().Be(DataTypes.Integer);
            first.RightOperand.Value.Should().Be(1);
            second.RightOperand.DataType.Should().Be(DataTypes.Integer);
            second.RightOperand.Value.Should().Be(2);
        }

        [Fact]
        public async Task SanitizeAsync_ValueOneCardinality_PatternMismatch_AddsErrorR0026()
        {
            // Arrange
            var rulesSource = new Mock<IRulesSource>();
            var conditionName = "condInt";
            var conditions = new Dictionary<string, Condition>
            {
                { conditionName, new Condition(conditionName, DateTime.UtcNow, DataTypes.Integer) }
            };
            rulesSource.Setup(x => x.GetConditionsAsync(It.IsAny<GetConditionsArgs>())).ReturnsAsync(conditions);

            var dataTypeProvider = new DataTypesConfigurationProvider(RulesEngineOptions.NewWithDefaults());
            var sanitizer = new RuleSanitizer(rulesSource.Object, dataTypeProvider);

            // Right operand is a string that doesn't match integer pattern
            var rule = Rule.Create("r").InRuleset("rs").SetContent("c").Since(DateTime.UtcNow)
                .ApplyWhen(conditionName, Operators.Equal, "abc").Build().Rule;

            // Act
            var result = await sanitizer.SanitizeAsync(rule);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.Code == Constants.ErrorCodes.R0026);
        }

        [Fact]
        public async Task SanitizeAsync_ValueOneCardinality_InvalidCast_AddsErrorR0027()
        {
            // Arrange
            var rulesSource = new Mock<IRulesSource>();
            var conditionName = "condInt";
            var conditions = new Dictionary<string, Condition>
            {
                { conditionName, new Condition(conditionName, DateTime.UtcNow, DataTypes.Integer) }
            };
            rulesSource.Setup(x => x.GetConditionsAsync(It.IsAny<GetConditionsArgs>())).ReturnsAsync(conditions);

            var dataTypeProvider = new DataTypesConfigurationProvider(RulesEngineOptions.NewWithDefaults());
            var sanitizer = new RuleSanitizer(rulesSource.Object, dataTypeProvider);

            // Create a right operand with a non-convertible object; use explicit ctor to avoid Operand runtime checks
            var valueOperand = new Operand(new object(), DataTypes.Boolean, Cardinalities.One);
            var valueCondition = new ValueConditionNode(conditionName, Operators.Equal, valueOperand);
            var rule = Rule.Create("r").InRuleset("rs").SetContent("c").Since(DateTime.UtcNow).Build().Rule;
            rule.RootCondition = valueCondition;

            // Act
            var result = await sanitizer.SanitizeAsync(rule);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.Code == Constants.ErrorCodes.R0027);
        }

        [Fact]
        public async Task SanitizeAsync_ValueManyCardinality_RightOperandIsEnumerable_ConvertsAndAddsR0028()
        {
            // Arrange
            var rulesSource = new Mock<IRulesSource>();
            var conditionName = "manyInt";
            var conditions = new Dictionary<string, Condition>
            {
                { conditionName, new Condition(conditionName, DateTime.UtcNow, DataTypes.Integer) }
            };
            rulesSource.Setup(x => x.GetConditionsAsync(It.IsAny<GetConditionsArgs>())).ReturnsAsync(conditions);

            var dataTypeProvider = new DataTypesConfigurationProvider(RulesEngineOptions.NewWithDefaults());
            var sanitizer = new RuleSanitizer(rulesSource.Object, dataTypeProvider);

            // Provide an IEnumerable<string> as right operand value with cardinality Many
            var rightValue = new[] { "1", "2" };
            var operand = new Operand(rightValue, DataTypes.String, Cardinalities.Many);
            var valueCondition = new ValueConditionNode(conditionName, Operators.In, operand);

            var rule = Rule.Create("r").InRuleset("rs").SetContent("c").Since(DateTime.UtcNow).Build().Rule;
            rule.RootCondition = valueCondition;

            // Act
            var result = await sanitizer.SanitizeAsync(rule);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.Code == Constants.ErrorCodes.R0028);

            var vc = (ValueConditionNode)rule.RootCondition!;
            vc.RightOperand.Cardinality.Should().Be(Cardinalities.One);
            vc.RightOperand.DataType.Should().Be(DataTypes.Integer);
            // Value should be an IEnumerable<object>
            vc.RightOperand.Value.Should().BeAssignableTo<IEnumerable>();
        }

        [Fact]
        public async Task SanitizeAsync_ValueManyCardinality_RightOperandNotEnumerable_AddsR0028()
        {
            // Arrange
            var rulesSource = new Mock<IRulesSource>();
            var conditionName = "manyInt";
            var conditions = new Dictionary<string, Condition>
            {
                { conditionName, new Condition(conditionName, DateTime.UtcNow, DataTypes.Integer) }
            };
            rulesSource.Setup(x => x.GetConditionsAsync(It.IsAny<GetConditionsArgs>())).ReturnsAsync(conditions);

            var dataTypeProvider = new DataTypesConfigurationProvider(RulesEngineOptions.NewWithDefaults());
            var sanitizer = new RuleSanitizer(rulesSource.Object, dataTypeProvider);

            // Provide a non-enumerable object but set cardinality to Many
            var operand = new Operand(new object(), DataTypes.String, Cardinalities.Many);
            var valueCondition = new ValueConditionNode(conditionName, Operators.In, operand);

            var rule = Rule.Create("r").InRuleset("rs").SetContent("c").Since(DateTime.UtcNow).Build().Rule;
            rule.RootCondition = valueCondition;

            // Act
            var result = await sanitizer.SanitizeAsync(rule);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.Code == Constants.ErrorCodes.R0028);
        }
    }
}
