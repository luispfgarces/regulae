namespace Regulae.Rql.Tests.Pipeline.Interpret
{
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using Regulae.Rql;
    using Regulae.Rql.Ast.Statements;
    using Regulae.Rql.Pipeline.Interpret;
    using Regulae.Rql.Runtime;
    using Xunit;

    public partial class InterpreterTests
    {
        [Fact]
        public async Task VisitExpressionStatemet_GivenValidExpressionStatement_ReturnsExpressionResultWithRql()
        {
            // Arrange
            var expectedValue = NewRqlString("test");
            var expectedRql = "test rql";
            var expression = CreateMockedExpression(expectedValue);
            var expressionStatement = ExpressionStatement.Create(expression, RqlSourcePosition.Empty, RqlSourcePosition.Empty);

            var runtime = Mock.Of<IRuntime>();
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();
            Mock.Get(reverseRqlBuilder)
                .Setup(x => x.BuildRql(It.IsIn(expressionStatement)))
                .Returns(expectedRql);

            var interpreter = new Interpreter(runtime, reverseRqlBuilder);

            // Act
            var actual = await interpreter.VisitExpressionStatement(expressionStatement);

            // Assert
            actual.Should().NotBeNull().And.BeOfType<ExpressionStatementResult>();
            actual.Rql.Should().Be(expectedRql);
            actual.Success.Should().BeTrue();
            var actualExpressionStatementResult = (ExpressionStatementResult)actual;
            actualExpressionStatementResult.Result.Should().BeEquivalentTo(expectedValue);
        }
    }
}