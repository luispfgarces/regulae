namespace Regulae.Tests.Evaluation.Interpreted
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Moq;
    using Regulae;
    using Regulae.ConditionNodes;
    using Regulae.Evaluation;
    using Regulae.Evaluation.Interpreted;
    using Regulae.Evaluation.Interpreted.ValueEvaluation.Dispatchers;
    using Regulae.Tests.TestStubs;
    using Xunit;

    public class InterpretedConditionsEvalEngineTests
    {
        [Fact]
        public void Eval_GivenComposedConditionNodeWithAndOperatorAndMissingConditionWithSearchMode_EvalsAndReturnsResult()
        {
            // Arrange
            var condition1 = new ValueConditionNode(ConditionNames.IsVip.ToString(), Operators.Equal, true);
            var condition2 = new ValueConditionNode(ConditionNames.IsoCurrency.ToString(), Operators.NotEqual, "SGD");

            var composedConditionNode = new ComposedConditionNode(
                LogicalOperators.And,
                new IConditionNode[] { condition1, condition2 });

            var conditions = new Dictionary<string, Operand>
            {
                {
                    ConditionNames.IsoCurrency.ToString(),
                    "SGD"
                },
                {
                    ConditionNames.IsVip.ToString(),
                    true
                }
            };

            var evaluationOptions = new EvaluationOptions
            {
                MatchMode = MatchModes.Search,
                ExcludeRulesWithoutSearchConditions = true
            };

            var mockConditionEvalDispatcher = new Mock<IConditionEvalDispatcher>();
            mockConditionEvalDispatcher.Setup(x => x.EvalDispatch(It.IsAny<Operand>(), It.IsAny<Operators>(), It.IsAny<Operand>()))
                .Returns(false);

            var mockConditionEvalDispatchProvider = new Mock<IConditionEvalDispatcherProvider>();
            mockConditionEvalDispatchProvider.Setup(x => x.GetEvalDispatcher(It.IsAny<Operand>(), It.IsAny<Operators>(), It.IsAny<Operand>()))
                .Returns(() =>
                {
                    return mockConditionEvalDispatcher.Object;
                });
            var conditionsTreeAnalyzer = Mock.Of<IConditionsTreeAnalyzer>();
            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            var sut = new InterpretedConditionsEvalEngine(mockConditionEvalDispatchProvider.Object, conditionsTreeAnalyzer, rulesEngineOptions);

            // Act
            var actual = sut.Eval(composedConditionNode, conditions, evaluationOptions);

            // Assert
            actual.Should().BeFalse();
        }

        [Fact]
        public void Eval_GivenComposedConditionNodeWithAndOperatorWithExactMatch_EvalsAndReturnsResult()
        {
            // Arrange
            var condition1 = new ValueConditionNode(ConditionNames.IsVip.ToString(), Operators.Equal, true);
            var condition2 = new ValueConditionNode(ConditionNames.IsoCurrency.ToString(), Operators.NotEqual, "SGD");

            var composedConditionNode = new ComposedConditionNode(
                LogicalOperators.Eval,
                new IConditionNode[] { condition1, condition2 });

            var conditions = new Dictionary<string, Operand>
            {
                {
                    ConditionNames.IsoCurrency.ToString(),
                    "SGD"
                },
                {
                    ConditionNames.IsVip.ToString(),
                    true
                }
            };

            var evaluationOptions = new EvaluationOptions
            {
                MatchMode = MatchModes.Exact
            };

            var mockConditionEvalDispatcher = new Mock<IConditionEvalDispatcher>();
            mockConditionEvalDispatcher.Setup(x => x.EvalDispatch(It.IsAny<Operand>(), It.IsAny<Operators>(), It.IsAny<Operand>()))
                .Returns(false);

            var mockConditionEvalDispatchProvider = new Mock<IConditionEvalDispatcherProvider>();
            mockConditionEvalDispatchProvider.Setup(x => x.GetEvalDispatcher(It.IsAny<Operand>(), It.IsAny<Operators>(), It.IsAny<Operand>()))
                .Returns(() =>
                {
                    return mockConditionEvalDispatcher.Object;
                });
            var conditionsTreeAnalyzer = Mock.Of<IConditionsTreeAnalyzer>();
            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            var sut = new InterpretedConditionsEvalEngine(mockConditionEvalDispatchProvider.Object, conditionsTreeAnalyzer, rulesEngineOptions);

            // Act
            var notSupportedException = Assert.Throws<NotSupportedException>(() => sut.Eval(composedConditionNode, conditions, evaluationOptions));

            // Assert
            notSupportedException.Should().NotBeNull();
            notSupportedException.Message.Should().Be("Unsupported logical operator: 'Eval'.");
        }

        [Fact]
        public void Eval_GivenComposedConditionNodeWithEvalOperator_ThrowsNotSupportedException()
        {
            // Arrange
            var condition1 = new ValueConditionNode(ConditionNames.IsVip.ToString(), Operators.Equal, true);
            var condition2 = new ValueConditionNode(ConditionNames.IsoCurrency.ToString(), Operators.NotEqual, "SGD");

            var composedConditionNode = new ComposedConditionNode(
                LogicalOperators.Eval,
                new IConditionNode[] { condition1, condition2 });

            var conditions = new Dictionary<string, Operand>
            {
                {
                    ConditionNames.IsoCurrency.ToString(),
                    "SGD"
                },
                {
                    ConditionNames.IsVip.ToString(),
                    true
                }
            };

            var evaluationOptions = new EvaluationOptions
            {
                MatchMode = MatchModes.Exact
            };

            var mockConditionEvalDispatcher = new Mock<IConditionEvalDispatcher>();
            mockConditionEvalDispatcher.Setup(x => x.EvalDispatch(It.IsAny<Operand>(), It.IsAny<Operators>(), It.IsAny<Operand>()))
                .Returns(false);

            var mockConditionEvalDispatchProvider = new Mock<IConditionEvalDispatcherProvider>();
            mockConditionEvalDispatchProvider.Setup(x => x.GetEvalDispatcher(It.IsAny<Operand>(), It.IsAny<Operators>(), It.IsAny<Operand>()))
                .Returns(() =>
                {
                    return mockConditionEvalDispatcher.Object;
                });
            var conditionsTreeAnalyzer = Mock.Of<IConditionsTreeAnalyzer>();
            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            var sut = new InterpretedConditionsEvalEngine(mockConditionEvalDispatchProvider.Object, conditionsTreeAnalyzer, rulesEngineOptions);

            // Act
            var notSupportedException = Assert.Throws<NotSupportedException>(() => sut.Eval(composedConditionNode, conditions, evaluationOptions));

            // Assert
            notSupportedException.Should().NotBeNull();
            notSupportedException.Message.Should().Be("Unsupported logical operator: 'Eval'.");
        }

        [Fact]
        public void Eval_GivenComposedConditionNodeWithOrOperatorWithExactMatch_EvalsAndReturnsResult()
        {
            // Arrange
            var condition1 = new ValueConditionNode(ConditionNames.IsVip.ToString(), Operators.Equal, true);
            var condition2 = new ValueConditionNode(ConditionNames.IsoCurrency.ToString(), Operators.NotEqual, "SGD");

            var composedConditionNode = new ComposedConditionNode(
                LogicalOperators.Or,
                new IConditionNode[] { condition1, condition2 });

            var conditions = new Dictionary<string, Operand>
            {
                {
                    ConditionNames.IsoCurrency.ToString(),
                    "SGD"
                },
                {
                    ConditionNames.IsVip.ToString(),
                    true
                }
            };

            var evaluationOptions = new EvaluationOptions
            {
                MatchMode = MatchModes.Exact
            };

            var mockConditionEvalDispatcher = new Mock<IConditionEvalDispatcher>();
            mockConditionEvalDispatcher.Setup(x => x.EvalDispatch(It.IsAny<Operand>(), It.IsAny<Operators>(), It.IsAny<Operand>()))
                .Returns(true);

            var mockConditionEvalDispatchProvider = new Mock<IConditionEvalDispatcherProvider>();
            mockConditionEvalDispatchProvider.Setup(x => x.GetEvalDispatcher(It.IsAny<Operand>(), It.IsAny<Operators>(), It.IsAny<Operand>()))
                .Returns(() =>
                {
                    return mockConditionEvalDispatcher.Object;
                });
            var conditionsTreeAnalyzer = Mock.Of<IConditionsTreeAnalyzer>();
            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            var sut = new InterpretedConditionsEvalEngine(mockConditionEvalDispatchProvider.Object, conditionsTreeAnalyzer, rulesEngineOptions);

            // Act
            var actual = sut.Eval(composedConditionNode, conditions, evaluationOptions);

            // Assert
            actual.Should().BeTrue();
        }
    }
}