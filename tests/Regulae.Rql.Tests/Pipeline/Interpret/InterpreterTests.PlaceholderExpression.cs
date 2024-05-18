namespace Regulae.Rql.Tests.Pipeline.Interpret
{
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using Regulae.Rql;
    using Regulae.Rql.Ast.Expressions;
    using Regulae.Rql.Pipeline.Interpret;
    using Regulae.Rql.Runtime;
    using Regulae.Rql.Runtime.Types;
    using Xunit;

    public partial class InterpreterTests
    {
        [Fact]
        public async Task VisitPlaceholderExpression_GivenPlaceholderExpression_ReturnsRqlStringWithPlaceholderName()
        {
            // Arrange
            var placeholderExpression = new PlaceholderExpression(NewToken("testPlaceholder", "testPlaceholder", Regulae.Rql.Tokens.TokenType.PLACEHOLDER));

            var runtime = Mock.Of<IRuntime>();
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();

            var interpreter = new Interpreter(runtime, reverseRqlBuilder);

            // Act
            var actual = await interpreter.VisitPlaceholderExpression(placeholderExpression);

            // Assert
            actual.Should().BeOfType<RqlString>();
            var actualString = (RqlString)actual;
            actualString.Value.Should().Be("testPlaceholder");
        }
    }
}