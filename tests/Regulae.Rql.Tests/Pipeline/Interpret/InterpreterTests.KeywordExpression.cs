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
        public async Task VisitKeywordExpression_GivenValidKeywordExpression_ReturnsLexeme()
        {
            // Arrange
            var expected = NewRqlString("var");
            var keywordToken = NewToken("var", null, TokenType.VAR);
            var keywordExpression = KeywordExpression.Create(keywordToken);

            var runtime = Mock.Of<IRuntime>();
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();

            var interpreter = new Interpreter(runtime, reverseRqlBuilder);

            // Act
            var actual = await interpreter.VisitKeywordExpression(keywordExpression);

            // Assert
            actual.Should().BeEquivalentTo(expected);
        }
    }
}