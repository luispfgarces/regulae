namespace Regulae.Rql.Tests.Pipeline.Interpret
{
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using Regulae.Rql;
    using Regulae.Rql.Ast.Expressions;
    using Regulae.Rql.Ast.Segments;
    using Regulae.Rql.Ast.Statements;
    using Regulae.Rql.Pipeline.Interpret;
    using Regulae.Rql.Runtime;
    using Regulae.Rql.Runtime.Types;
    using Xunit;

    public partial class InterpreterTests
    {
        [Fact]
        public async Task VisitNoneExpression_GivenNoneExpression_ReturnsRqlNothing()
        {
            // Arrange
            var noneExpression = new NoneExpression();

            var runtime = Mock.Of<IRuntime>();
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();

            var interpreter = new Interpreter(runtime, reverseRqlBuilder);

            // Act
            var actual = await interpreter.VisitNoneExpression(noneExpression);

            // Assert
            actual.Should().NotBeNull()
                .And.BeOfType<RqlNothing>();
        }

        [Fact]
        public async Task VisitNoneSegment_GivenNoneSegment_ReturnsNull()
        {
            // Arrange
            var noneSegment = new NoneSegment();

            var runtime = Mock.Of<IRuntime>();
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();

            var interpreter = new Interpreter(runtime, reverseRqlBuilder);

            // Act
            var actual = await interpreter.VisitNoneSegment(noneSegment);

            // Assert
            actual.Should().BeNull();
        }

        [Fact]
        public async Task VisitNoneStatement_GivenNoneStatement_ReturnsExpressionStatementWithRqlNothing()
        {
            // Arrange
            var noneStatement = new NoneStatement();

            var runtime = Mock.Of<IRuntime>();
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();

            var interpreter = new Interpreter(runtime, reverseRqlBuilder);

            // Act
            var actual = await interpreter.VisitNoneStatement(noneStatement);

            // Assert
            actual.Should().NotBeNull()
                .And.BeOfType<ExpressionStatementResult>();
            var actualExpressionStatementResult = (ExpressionStatementResult)actual;
            actualExpressionStatementResult.Rql.Should().BeEmpty();
            actualExpressionStatementResult.Result.Should().BeOfType<RqlNothing>();
        }
    }
}