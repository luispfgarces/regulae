namespace Regulae.Tests.Evaluation.Compiled.ExpressionBuilders
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using FluentAssertions;
    using Moq;
    using Regulae.Evaluation.Compiled.ExpressionBuilders;
    using Xunit;

    public class ExpressionBuilderTests
    {
        [Fact]
        public void HavingReturn_GivenNotNullTypeAndDefaultValue_ReturnsExpressionImplementationBuilder()
        {
            // Arrange
            var type = typeof(object);
            object defaultValue = null;

            var expressionName = "TestExpression";
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();

            var expressionBuilder = new ExpressionBuilder(expressionName, expressionBuilderFactory);

            // Act
            var actual = expressionBuilder.HavingReturn(type, defaultValue);

            // Assert
            actual.Should().NotBeNull()
                .And.BeSameAs(expressionBuilder);
            expressionBuilder.ReturnDefaultValue.Should().Be(defaultValue);
            expressionBuilder.ReturnType.Should().Be(type);
            expressionBuilder.ReturnLabelTarget.Should().NotBeNull();
            expressionBuilder.ReturnLabelTarget.Name.Should().Contain(expressionName);
        }

        [Fact]
        public void HavingReturn_GivenNullTypeAndDefaultValue_ReturnsExpressionImplementationBuilder()
        {
            // Arrange
            var expressionName = "TestExpression";
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();

            var expressionBuilder = new ExpressionBuilder(expressionName, expressionBuilderFactory);

            // Act
            var action = FluentActions.Invoking(() => expressionBuilder.HavingReturn(null, null));

            // Assert
            action.Should().Throw<ArgumentNullException>()
                .Which.ParamName.Should().Be("type");
        }

        [Fact]
        public void HavingReturn_GivenTypeAsGenericParameterAndDefaultValue_ReturnsExpressionImplementationBuilder()
        {
            // Arrange
            object defaultValue = null;

            var expressionName = "TestExpression";
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();

            var expressionBuilder = new ExpressionBuilder(expressionName, expressionBuilderFactory);

            // Act
            var actual = expressionBuilder.HavingReturn<object>(defaultValue);

            // Assert
            actual.Should().NotBeNull()
                .And.BeSameAs(expressionBuilder);
            expressionBuilder.ReturnDefaultValue.Should().Be(defaultValue);
            expressionBuilder.ReturnType.Should().Be(typeof(object));
            expressionBuilder.ReturnLabelTarget.Should().NotBeNull();
            expressionBuilder.ReturnLabelTarget.Name.Should().Contain(expressionName);
        }

        [Fact]
        public void NewExpression_GivenNonNullEmptyOrWhitespaceName_ReturnsNewNamedExpressionBuilder()
        {
            // Arrange
            var expressionName = "TestExpression";

            // Act
            var actual = ExpressionBuilder.NewExpression(expressionName);

            // Assert
            actual.Should().NotBeNull()
                .And.BeOfType<ExpressionBuilder>();

            var expressionBuilder = actual as ExpressionBuilder;
            expressionBuilder.Name.Should().Be(expressionName);
        }

        [Fact]
        public void NewExpression_GivenNullEmptyOrWhitespaceName_ThrowsArgumentException()
        {
            // Arrange
            var expressionName = "";

            // Act
            var action = FluentActions.Invoking(() => ExpressionBuilder.NewExpression(expressionName));

            // Assert
            action.Should().ThrowExactly<ArgumentException>()
                .Which.ParamName.Should().Be("name");
        }

        [Fact]
        public void SetImplementation_GivenNonNullBuilderAction_ReturnsConfiguredExpressionBuilder()
        {
            // Arrange
            var expressionName = "TestExpression";
            var testParameter = Expression.Parameter(typeof(int), "input");
            var testReturnLabelTarget = Expression.Label(typeof(int));

            var testExpressions = new List<Expression>
            {
                Expression.Constant(1),
            };
            var testVariables = new Dictionary<string, ParameterExpression>
            {
                { "result", Expression.Variable(typeof(int), "result") },
            };
            var testLabelTargets = new Dictionary<string, LabelTarget>
            {
                { "Return", Expression.Label(typeof(int)) },
            };
            var executedBuilderAction = false;

            Action<IExpressionBlockBuilder> builderAction = b =>
            {
                executedBuilderAction = true;
            };

            var implementationExpressionBuilder = Mock.Of<IExpressionBlockBuilder>();
            Mock.Get(implementationExpressionBuilder)
                .SetupGet(x => x.LabelTargets)
                .Returns(testLabelTargets);
            Mock.Get(implementationExpressionBuilder)
                .SetupGet(x => x.Variables)
                .Returns(testVariables);
            Mock.Get(implementationExpressionBuilder)
                .SetupGet(x => x.Expressions)
                .Returns(testExpressions);

            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();
            Mock.Get(expressionBuilderFactory)
                .Setup(x => x.CreateExpressionBlockBuilder(It.IsAny<string>(), It.IsAny<IExpressionBlockBuilder>(), It.IsAny<ExpressionConfiguration>()))
                .Returns(implementationExpressionBuilder);

            var expressionBuilder = new ExpressionBuilder(
                expressionName,
                expressionBuilderFactory);

            // Act
            var actual = expressionBuilder.SetImplementation(builderAction);

            // Assert
            actual.Should().NotBeNull()
                .And.BeSameAs(expressionBuilder);
            executedBuilderAction.Should().BeTrue();
            expressionBuilder.LabelTargets.Should().NotBeNull()
                .And.BeSameAs(testLabelTargets);
            expressionBuilder.Variables.Should().NotBeNull()
                .And.BeSameAs(testVariables);
            expressionBuilder.Expressions.Should().NotBeNull()
                .And.BeSameAs(testExpressions);
        }

        [Fact]
        public void SetImplementation_GivenNullBuilderAction_ThrowsArgumentNullException()
        {
            // Arrange
            var expressionName = "TestExpression";

            Action<IExpressionBlockBuilder> builderAction = null;

            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();

            var expressionBuilder = new ExpressionBuilder(
                expressionName,
                expressionBuilderFactory);

            // Act
            var action = FluentActions.Invoking(() => expressionBuilder.SetImplementation(builderAction));

            // Assert
            action.Should().Throw<ArgumentNullException>()
                .Which.ParamName.Should().Be("builder");
        }

        [Fact]
        public void WithoutParameters_NoConditions_ReturnsExpressionReturnBuilder()
        {
            // Arrange
            var expressionName = "TestExpression";
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();

            var expressionBuilder = new ExpressionBuilder(expressionName, expressionBuilderFactory);

            // Act
            var actual = expressionBuilder.WithoutParameters();

            // Assert
            actual.Should().NotBeNull()
                .And.BeSameAs(expressionBuilder);
            expressionBuilder.Parameters.Should().NotBeNull()
                .And.BeEmpty();
        }

        [Fact]
        public void WithParameters_GivenNotNullBuilderAction_ReturnsExpressionReturnBuilder()
        {
            // Arrange
            var expressionName = "TestExpression";
            var parameterExpressions = new Dictionary<string, ParameterExpression>();
            var expressionParametersConfiguration = Mock.Of<IExpressionParametersConfiguration>();
            Mock.Get(expressionParametersConfiguration)
                .Setup(x => x.CreateParameter<It.IsAnyType>(It.IsAny<string>()))
                .Returns((IInvocation i) =>
                {
                    var parameterExpression = Expression.Parameter(i.Method.GetGenericArguments()[0], (string)i.Arguments[0]);
                    parameterExpressions.Add(parameterExpression.Name, parameterExpression);
                    return parameterExpression;
                });
            Mock.Get(expressionParametersConfiguration)
                .SetupGet(x => x.Parameters)
                .Returns(parameterExpressions);
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();
            Mock.Get(expressionBuilderFactory)
                .Setup(x => x.CreateExpressionParametersConfiguration())
                .Returns(expressionParametersConfiguration);

            var expressionBuilder = new ExpressionBuilder(expressionName, expressionBuilderFactory);

            // Act
            var actual = expressionBuilder.WithParameters(c =>
            {
                c.CreateParameter<object>("testParameter");
            });

            // Assert
            actual.Should().NotBeNull()
                .And.BeSameAs(expressionBuilder);
            expressionBuilder.Parameters.Should().ContainKey("testParameter");
        }

        [Fact]
        public void WithParameters_GivenNullBuilderAction_ThrowsArgumentNullException()
        {
            // Arrange
            var expressionName = "TestExpression";
            var expressionBuilderFactory = Mock.Of<IExpressionBuilderFactory>();

            var expressionBuilder = new ExpressionBuilder(expressionName, expressionBuilderFactory);

            // Act
            var action = FluentActions.Invoking(() => expressionBuilder.WithParameters(null));

            // Assert
            action.Should().Throw<ArgumentNullException>()
                .Which.ParamName.Should().Be("parametersConfigurationAction");
        }
    }
}