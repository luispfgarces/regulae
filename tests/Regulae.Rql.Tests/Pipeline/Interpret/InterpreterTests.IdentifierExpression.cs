namespace Regulae.Rql.Tests.Pipeline.Interpret
{
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using Regulae.Rql;
    using Regulae.Rql.Ast.Expressions;
    using Regulae.Rql.Pipeline.Interpret;
    using Regulae.Rql.Runtime;
    using Regulae.Rql.Tokens;
    using Xunit;

    public partial class InterpreterTests
    {
        [Fact]
        public async Task VisitIdentifierExpression_GivenValidIdentifierExpression_ReturnsIdentifierLexeme()
        {
            // Arrange
            var expected = NewRqlString("test");
            var identifierToken = NewToken("test", null, TokenType.IDENTIFIER);
            var identifierExpression = new IdentifierExpression(identifierToken);

            var runtime = Mock.Of<IRuntime>();
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();

            var interpreter = new Interpreter(runtime, reverseRqlBuilder);

            // Act
            var actual = await interpreter.VisitIdentifierExpression(identifierExpression);

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }
    }
}