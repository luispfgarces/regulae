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

    public class ManyToManyConditionEvalDispatcherTests
    {
        [Fact]
        public void EvalDispatch_GivenStringDataTypeLeftOperandAsIntegerAndRightOperandAsStringCollection_ThrowsArgumentExceptionForLeftOperand()
        {
            // Arrange
            var dataType = DataTypes.String;
            object leftOperand = 1;
            object rightOperand = new[] { "str3", "str4" };
            var @operator = Operators.In;

            var manyToManyOperatorEvalStrategy = Mock.Of<IManyToManyOperatorEvalStrategy>();
            Mock.Get(manyToManyOperatorEvalStrategy).Setup(x => x.Eval(It.IsAny<IEnumerable<object>>(), It.IsAny<IEnumerable<object>>())).Returns(false);
            var operatorEvalStrategyFactory = Mock.Of<IOperatorEvalStrategyFactory>();
            Mock.Get(operatorEvalStrategyFactory).Setup(x => x.GetManyToManyOperatorEvalStrategy(It.Is<Operators>(v => v == @operator))).Returns(manyToManyOperatorEvalStrategy);
            var dataTypesConfigurationProvider = Mock.Of<IDataTypesConfigurationProvider>();
            Mock.Get(dataTypesConfigurationProvider).Setup(x => x.GetDataTypeConfiguration(It.Is<DataTypes>(v => v == dataType))).Returns(DataTypeConfiguration.Create(dataType, typeof(string), string.Empty));

            var manyToManyConditionEvalDispatcher = new ManyToManyConditionEvalDispatcher(operatorEvalStrategyFactory, dataTypesConfigurationProvider);

            // Act
            var argumentException = Assert.Throws<ArgumentException>(() => manyToManyConditionEvalDispatcher.EvalDispatch(dataType, leftOperand, @operator, rightOperand));

            // Assert
            argumentException.Should().NotBeNull();
            argumentException.ParamName.Should().Be(nameof(leftOperand));
            argumentException.Message.Should().Contain(nameof(IEnumerable));
        }

        [Fact]
        public void EvalDispatch_GivenStringDataTypeLeftOperandAsStringCollectionAndRightOperandAsInteger_ThrowsArgumentExceptionForRightOperand()
        {
            // Arrange
            var dataType = DataTypes.String;
            object leftOperand = new[] { "str1", "str2" };
            object rightOperand = 1;
            var @operator = Operators.In;

            var manyToManyOperatorEvalStrategy = Mock.Of<IManyToManyOperatorEvalStrategy>();
            Mock.Get(manyToManyOperatorEvalStrategy).Setup(x => x.Eval(It.IsAny<IEnumerable<object>>(), It.IsAny<IEnumerable<object>>())).Returns(false);
            var operatorEvalStrategyFactory = Mock.Of<IOperatorEvalStrategyFactory>();
            Mock.Get(operatorEvalStrategyFactory).Setup(x => x.GetManyToManyOperatorEvalStrategy(It.Is<Operators>(v => v == @operator))).Returns(manyToManyOperatorEvalStrategy);
            var dataTypesConfigurationProvider = Mock.Of<IDataTypesConfigurationProvider>();
            Mock.Get(dataTypesConfigurationProvider).Setup(x => x.GetDataTypeConfiguration(It.Is<DataTypes>(v => v == dataType))).Returns(DataTypeConfiguration.Create(dataType, typeof(string), string.Empty));

            var manyToManyConditionEvalDispatcher = new ManyToManyConditionEvalDispatcher(operatorEvalStrategyFactory, dataTypesConfigurationProvider);

            // Act
            var argumentException = Assert.Throws<ArgumentException>(() => manyToManyConditionEvalDispatcher.EvalDispatch(dataType, leftOperand, @operator, rightOperand));

            // Assert
            argumentException.Should().NotBeNull();
            argumentException.ParamName.Should().Be(nameof(rightOperand));
            argumentException.Message.Should().Contain(nameof(IEnumerable));
        }

        [Fact]
        public void EvalDispatch_GivenStringDataTypeLeftOperandAsStringCollectionAndRightOperandAsStringCollection_EvalsOperatorAndReturnsBoolean()
        {
            // Arrange
            var dataType = DataTypes.String;
            object leftOperand = new[] { "str1", "str2" };
            object rightOperand = new[] { "str3", "str4" };
            var @operator = Operators.In;

            var manyToManyOperatorEvalStrategy = Mock.Of<IManyToManyOperatorEvalStrategy>();
            Mock.Get(manyToManyOperatorEvalStrategy).Setup(x => x.Eval(It.IsAny<IEnumerable<object>>(), It.IsAny<IEnumerable<object>>())).Returns(false);
            var operatorEvalStrategyFactory = Mock.Of<IOperatorEvalStrategyFactory>();
            Mock.Get(operatorEvalStrategyFactory).Setup(x => x.GetManyToManyOperatorEvalStrategy(It.Is<Operators>(v => v == @operator))).Returns(manyToManyOperatorEvalStrategy);
            var dataTypesConfigurationProvider = Mock.Of<IDataTypesConfigurationProvider>();
            Mock.Get(dataTypesConfigurationProvider).Setup(x => x.GetDataTypeConfiguration(It.Is<DataTypes>(v => v == dataType))).Returns(DataTypeConfiguration.Create(dataType, typeof(string), string.Empty));

            var manyToManyConditionEvalDispatcher = new ManyToManyConditionEvalDispatcher(operatorEvalStrategyFactory, dataTypesConfigurationProvider);

            // Act
            var result = manyToManyConditionEvalDispatcher.EvalDispatch(dataType, leftOperand, @operator, rightOperand);

            // Assert
            result.Should().BeFalse();
        }
    }
}