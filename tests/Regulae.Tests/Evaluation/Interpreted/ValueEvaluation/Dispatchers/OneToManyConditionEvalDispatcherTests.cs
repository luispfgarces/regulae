namespace Regulae.Tests.Evaluation.Interpreted.ValueEvaluation.Dispatchers
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Moq;
    using Regulae;
    using Regulae.Evaluation;
    using Regulae.Evaluation.Interpreted.ValueEvaluation;
    using Regulae.Evaluation.Interpreted.ValueEvaluation.Dispatchers;
    using Xunit;

    public class OneToManyConditionEvalDispatcherTests
    {
        [Fact]
        public void EvalDispatch_GivenStringDataTypeLeftOperandAsIntegerAndRightOperandAsStringCollection_ConvertsIntegerToStringEvalsOperatorAndReturnsBoolean()
        {
            // Arrange
            var dataType = DataTypes.String;
            object leftOperand = 1;
            object rightOperand = new[] { "str3", "str4" };
            var @operator = Operators.In;

            var manyToOneOperatorEvalStrategy = Mock.Of<IOneToManyOperatorEvalStrategy>();
            Mock.Get(manyToOneOperatorEvalStrategy).Setup(x => x.Eval(It.IsAny<object>(), It.IsAny<IEnumerable<object>>())).Returns(false);
            var operatorEvalStrategyFactory = Mock.Of<IOperatorEvalStrategyFactory>();
            Mock.Get(operatorEvalStrategyFactory).Setup(x => x.GetOneToManyOperatorEvalStrategy(It.Is<Operators>(v => v == @operator))).Returns(manyToOneOperatorEvalStrategy);
            var dataTypesConfigurationProvider = Mock.Of<IDataTypesConfigurationProvider>();
            Mock.Get(dataTypesConfigurationProvider).Setup(x => x.GetDataTypeConfiguration(It.Is<DataTypes>(v => v == dataType))).Returns(DataTypeConfiguration.Create(dataType, typeof(string), string.Empty));

            var oneToManyConditionEvalDispatcher = new OneToManyConditionEvalDispatcher(operatorEvalStrategyFactory, dataTypesConfigurationProvider);

            // Act
            var result = oneToManyConditionEvalDispatcher.EvalDispatch(dataType, leftOperand, @operator, rightOperand);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void EvalDispatch_GivenStringDataTypeLeftOperandAsStringCollectionAndRightOperandAsInteger_ThrowsArgumentExceptionForLeftOperand()
        {
            // Arrange
            var dataType = DataTypes.String;
            object leftOperand = new[] { "str1", "str2" };
            object rightOperand = 1;
            var @operator = Operators.In;

            var oneToManyOperatorEvalStrategy = Mock.Of<IOneToManyOperatorEvalStrategy>();
            Mock.Get(oneToManyOperatorEvalStrategy).Setup(x => x.Eval(It.IsAny<object>(), It.IsAny<IEnumerable<object>>())).Returns(false);
            var operatorEvalStrategyFactory = Mock.Of<IOperatorEvalStrategyFactory>();
            Mock.Get(operatorEvalStrategyFactory).Setup(x => x.GetOneToManyOperatorEvalStrategy(It.Is<Operators>(v => v == @operator))).Returns(oneToManyOperatorEvalStrategy);
            var dataTypesConfigurationProvider = Mock.Of<IDataTypesConfigurationProvider>();
            Mock.Get(dataTypesConfigurationProvider).Setup(x => x.GetDataTypeConfiguration(It.Is<DataTypes>(v => v == dataType))).Returns(DataTypeConfiguration.Create(dataType, typeof(string), string.Empty));

            var oneToManyConditionEvalDispatcher = new OneToManyConditionEvalDispatcher(operatorEvalStrategyFactory, dataTypesConfigurationProvider);

            // Act
            var argumentException = Assert.Throws<ArgumentException>(() => oneToManyConditionEvalDispatcher.EvalDispatch(dataType, leftOperand, @operator, rightOperand));

            // Assert
            argumentException.Should().NotBeNull();
            argumentException.ParamName.Should().Be(nameof(leftOperand));
            argumentException.Message.Should().Contain(nameof(String));
        }
    }
}