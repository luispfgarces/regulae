namespace Regulae.Tests.Evaluation.Interpreted.ValueEvaluation.Dispatchers
{
    using FluentAssertions;
    using Moq;
    using Regulae;
    using Regulae.Evaluation;
    using Regulae.Evaluation.Interpreted.ValueEvaluation;
    using Regulae.Evaluation.Interpreted.ValueEvaluation.Dispatchers;
    using Xunit;

    public class OneToOneConditionEvalDispatcherTests
    {
        [Fact]
        public void EvalDispatch_GivenStringDataTypeLeftOperandAsIntegerAndRightOperandAsInteger_ConvertsIntegersToStringsEvalsOperatorAndReturnsBoolean()
        {
            // Arrange
            var dataType = DataTypes.String;
            var leftOperand = 1;
            var rightOperand = "2";
            var @operator = Operators.In;

            var oneToOneOperatorEvalStrategy = Mock.Of<IOneToOneOperatorEvalStrategy>();
            Mock.Get(oneToOneOperatorEvalStrategy).Setup(x => x.Eval(It.IsAny<object>(), It.IsAny<object>())).Returns(false);
            var operatorEvalStrategyFactory = Mock.Of<IOperatorEvalStrategyFactory>();
            Mock.Get(operatorEvalStrategyFactory).Setup(x => x.GetOneToOneOperatorEvalStrategy(It.Is<Operators>(v => v == @operator))).Returns(oneToOneOperatorEvalStrategy);
            var dataTypesConfigurationProvider = Mock.Of<IDataTypesConfigurationProvider>();
            Mock.Get(dataTypesConfigurationProvider).Setup(x => x.GetDataTypeConfiguration(It.Is<DataTypes>(v => v == dataType))).Returns(DataTypeConfiguration.Create(dataType, typeof(string), string.Empty));

            var oneToOneConditionEvalDispatcher = new OneToOneConditionEvalDispatcher(operatorEvalStrategyFactory, dataTypesConfigurationProvider);

            // Act
            var result = oneToOneConditionEvalDispatcher.EvalDispatch(leftOperand, @operator, rightOperand);

            // Assert
            result.Should().BeFalse();
        }
    }
}