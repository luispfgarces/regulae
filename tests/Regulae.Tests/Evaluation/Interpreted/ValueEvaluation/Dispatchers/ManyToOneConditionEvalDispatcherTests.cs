namespace Regulae.Tests.Evaluation.Interpreted.ValueEvaluation.Dispatchers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using FluentAssertions;
    using Moq;
    using Regulae;
    using Regulae.Evaluation;
    using Regulae.Evaluation.Interpreted.ValueEvaluation;
    using Regulae.Evaluation.Interpreted.ValueEvaluation.Dispatchers;
    using Xunit;

    public class ManyToOneConditionEvalDispatcherTests
    {
        [Fact]
        public void EvalDispatch_GivenStringDataTypeLeftOperandAsIntegerAndRightOperandAsStringCollection_ThrowsArgumentExceptionForLeftOperand()
        {
            // Arrange
            var dataType = DataTypes.String;
            object leftOperand = 1;
            object rightOperand = new[] { "str3", "str4" };
            var @operator = Operators.In;

            var manyToOneOperatorEvalStrategy = Mock.Of<IManyToOneOperatorEvalStrategy>();
            Mock.Get(manyToOneOperatorEvalStrategy).Setup(x => x.Eval(It.IsAny<IEnumerable<object>>(), It.IsAny<object>())).Returns(false);
            var operatorEvalStrategyFactory = Mock.Of<IOperatorEvalStrategyFactory>();
            Mock.Get(operatorEvalStrategyFactory).Setup(x => x.GetManyToOneOperatorEvalStrategy(It.Is<Operators>(v => v == @operator))).Returns(manyToOneOperatorEvalStrategy);
            var dataTypesConfigurationProvider = Mock.Of<IDataTypesConfigurationProvider>();
            Mock.Get(dataTypesConfigurationProvider).Setup(x => x.GetDataTypeConfiguration(It.Is<DataTypes>(v => v == dataType))).Returns(DataTypeConfiguration.Create(dataType, typeof(string), string.Empty));

            var manyToOneConditionEvalDispatcher = new ManyToOneConditionEvalDispatcher(operatorEvalStrategyFactory, dataTypesConfigurationProvider);

            // Act
            var argumentException = Assert.Throws<ArgumentException>(() => manyToOneConditionEvalDispatcher.EvalDispatch(dataType, leftOperand, @operator, rightOperand));

            // Assert
            argumentException.Should().NotBeNull();
            argumentException.ParamName.Should().Be(nameof(leftOperand));
            argumentException.Message.Should().Contain(nameof(IEnumerable));
        }

        [Fact]
        public void EvalDispatch_GivenStringDataTypeLeftOperandAsStringCollectionAndRightOperandAsInteger_ConvertsIntegerToStringEvalsOperatorAndReturnsBoolean()
        {
            // Arrange
            var dataType = DataTypes.String;
            object leftOperand = new[] { "str1", "str2" };
            object rightOperand = 1;
            var @operator = Operators.In;

            var manyToOneOperatorEvalStrategy = Mock.Of<IManyToOneOperatorEvalStrategy>();
            Mock.Get(manyToOneOperatorEvalStrategy).Setup(x => x.Eval(It.IsAny<IEnumerable<object>>(), It.IsAny<object>())).Returns(false);
            var operatorEvalStrategyFactory = Mock.Of<IOperatorEvalStrategyFactory>();
            Mock.Get(operatorEvalStrategyFactory).Setup(x => x.GetManyToOneOperatorEvalStrategy(It.Is<Operators>(v => v == @operator))).Returns(manyToOneOperatorEvalStrategy);
            var dataTypesConfigurationProvider = Mock.Of<IDataTypesConfigurationProvider>();
            Mock.Get(dataTypesConfigurationProvider).Setup(x => x.GetDataTypeConfiguration(It.Is<DataTypes>(v => v == dataType))).Returns(DataTypeConfiguration.Create(dataType, typeof(string), string.Empty));

            var manyToOneConditionEvalDispatcher = new ManyToOneConditionEvalDispatcher(operatorEvalStrategyFactory, dataTypesConfigurationProvider);

            // Act
            var result = manyToOneConditionEvalDispatcher.EvalDispatch(dataType, leftOperand, @operator, rightOperand);

            // Assert
            result.Should().BeFalse();
        }
    }
}