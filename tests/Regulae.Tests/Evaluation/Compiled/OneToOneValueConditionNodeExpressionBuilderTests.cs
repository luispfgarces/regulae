namespace Regulae.Tests.Evaluation.Compiled
{
    using System;
    using System.IO;
    using System.Linq.Expressions;
    using System.Reflection;
    using DiffPlex.DiffBuilder;
    using ExpressionDebugger;
    using FluentAssertions;
    using Moq;
    using Regulae;
    using Regulae.Evaluation;
    using Regulae.Evaluation.Compiled;
    using Regulae.Evaluation.Compiled.ConditionBuilders;
    using Regulae.Evaluation.Compiled.ExpressionBuilders;
    using Xunit;

    public class OneToOneValueConditionNodeExpressionBuilderTests
    {
        [Fact]
        public void Build_GivenExpressionBuilderAndArgs_BuildsExpression()
        {
            // Arrange
            string expectedScript;
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Regulae.Tests.Evaluation.Compiled.OneToOneValueConditionNodeExpressionBuilderTests.GoldenFile1.csx"))
            using (var streamReader = new StreamReader(stream))
            {
                expectedScript = streamReader.ReadToEnd();
            }
            Expression actualLeftExpression = null;
            Expression actualRightExpression = null;

            var conditionExpressionBuilder = Mock.Of<IConditionExpressionBuilder>();
            Mock.Get(conditionExpressionBuilder)
                .Setup(x => x.BuildConditionExpression(It.IsAny<IExpressionBlockBuilder>(), It.IsAny<BuildConditionExpressionArgs>()))
                .Returns<IExpressionBlockBuilder, BuildConditionExpressionArgs>((b, args) =>
                {
                    actualLeftExpression = args.LeftHandOperand;
                    actualRightExpression = args.RightHandOperand;

                    // For testing purposes, does not need to stay true to the scenario
                    return Expression.Equal(actualLeftExpression, actualRightExpression);
                });
            var conditionExpressionBuilderProvider = Mock.Of<IConditionExpressionBuilderProvider>();
            Mock.Get(conditionExpressionBuilderProvider)
                .Setup(x => x.GetConditionExpressionBuilderFor(Operators.Equal, Multiplicities.OneToOne))
                .Returns(conditionExpressionBuilder);

            var oneToOneValueConditionNodeCompiler =
                new OneToOneValueConditionNodeExpressionBuilder(conditionExpressionBuilderProvider);

            // Act
            var expressionResult = ExpressionBuilder.NewExpression("TestDummy")
                .WithParameters(p =>
                {
                    p.CreateParameter("leftOperand", typeof(Operand));
                    p.CreateParameter("rightOperand", typeof(Operand));
                })
                .HavingReturn(typeof(bool), false)
                .SetImplementation(builder =>
                {
                    var resultVariableExpression = builder.CreateVariable("result", typeof(bool));
                    var operandValueFieldInfo = typeof(Operand).GetField("Value");
                    var leftOperandVariableExpression = builder.GetParameter("leftOperand");
                    var leftOperandValueFieldAccessExpression = builder.AccessField(
                        leftOperandVariableExpression,
                        operandValueFieldInfo);
                    var rightOperandValueFieldAccessExpression = builder.AccessField(
                        builder.GetParameter("rightOperand"),
                        operandValueFieldInfo);
                    var testPresentLeftOperand = builder.AndAlso(
                        builder.NotEqual(leftOperandVariableExpression, builder.Constant<object>(value: null!)),
                        builder.NotEqual(leftOperandValueFieldAccessExpression, builder.Constant<object>(value: null!)));
                    var args = new BuildValueConditionNodeExpressionArgs
                    {
                        DataTypeConfiguration = DataTypeConfiguration.Create(DataTypes.String, typeof(string), null),
                        LeftOperandExpression = leftOperandValueFieldAccessExpression,
                        Operator = Operators.Equal,
                        ResultVariableExpression = resultVariableExpression,
                        RightOperandExpression = rightOperandValueFieldAccessExpression,
                        TestLeftOperand = testPresentLeftOperand
                    };

                    var blockExpression = builder.Block("test", innerBuilder =>
                    {
                        oneToOneValueConditionNodeCompiler.Build(innerBuilder, args);
                    });
                    builder.AddExpression(blockExpression);

                    builder.Return(resultVariableExpression);
                })
                .Build();

            // Assert
            expressionResult.Should().NotBeNull();
            expressionResult.Implementation.Should().NotBeNull();

            // Purely for the effect of comparing the generated lambda expression code with the
            // golden file.
            var lambdaExpression = Expression.Lambda<Func<Operand, Operand, bool>>(expressionResult.Implementation, expressionResult.Parameters);
            var actualScript = lambdaExpression.ToScript();
            var diffResult = SideBySideDiffBuilder.Diff(expectedScript, actualScript, ignoreWhiteSpace: true);
            diffResult.NewText.HasDifferences.Should().BeFalse();
            Func<Operand, Operand, bool> compiledLambdaExpression = null;
            FluentActions.Invoking(() => compiledLambdaExpression = lambdaExpression.Compile())
                .Should()
                .NotThrow("expression should be compilable");
            FluentActions.Invoking(() => compiledLambdaExpression.Invoke(null, "dummy"))
                .Should()
                .NotThrow("compiled expression should be executable");
            FluentActions.Invoking(() => compiledLambdaExpression.Invoke("abc", "dummy"))
                .Should()
                .NotThrow("compiled expression should be able to deal with null left operand");
            actualLeftExpression.Should().NotBeNull();
            actualLeftExpression.Type.Should().Be<string>();
            actualRightExpression.Should().NotBeNull();
            actualRightExpression.Type.Should().Be<string>();
        }
    }
}