namespace Regulae.Rql.Tests.Pipeline.Assist
{
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using Regulae.Rql.Ast.Expressions;
    using Regulae.Rql.Ast.Statements;
    using Regulae.Rql.Pipeline.Assist;
    using Regulae.Rql.Runtime;
    using Regulae.Rql.Tokens;
    using Xunit;

    public partial class AssistAstWalkerTests
    {
        [Fact]
        public async Task VisitExpressionStatement_WhenExpressionIsNotSpecial_ReturnsExpressionSuggestions()
        {
            // Arrange
            var runtime = Mock.Of<IRuntime>();
            var walker = AssistAstWalker.Create(runtime, RqlSourcePosition.Empty);

            // Use a simple identifier expression inside an expression statement
            var idToken = Token.Create("abc", false, null, RqlSourcePosition.From(1, 1), RqlSourcePosition.From(1, 4), 3, TokenType.IDENTIFIER);
            var identifier = new IdentifierExpression(idToken);
            var exprStmt = ExpressionStatement.Create(identifier, RqlSourcePosition.From(1, 1), RqlSourcePosition.From(1, 4));

            // Act
            var suggestions = await walker.VisitExpressionStatement(exprStmt);

            // Assert
            suggestions.Select(s => s.Lexeme).Should().Contain(["ARRAY", "MATCH", "NOTHING", "OBJECT", "SEARCH"]);
        }

        [Fact]
        public async Task VisitExpressionStatement_WhenExpressionIsMatchExpression_ReturnsChildSuggestions()
        {
            // Arrange
            var runtime = Mock.Of<IRuntime>();
            var walker = AssistAstWalker.Create(runtime, RqlSourcePosition.Empty);

            // Use a simple identifier expression inside an expression statement
            var idToken = Token.Create("test", false, null, RqlSourcePosition.From(1, 1), RqlSourcePosition.From(1, 5), 4, TokenType.IDENTIFIER);
            var identifierMock = new Mock<KeywordExpression>(idToken);
            identifierMock.Setup(e => e.Accept(It.IsAny<IExpressionVisitor<Task<IAssistSuggestion[]>>>()))
                .ReturnsAsync(
                [
                    AssistSuggestion.New("TEST"),
                ]);
            var exprStmt = ExpressionStatement.Create(identifierMock.Object, RqlSourcePosition.From(1, 1), RqlSourcePosition.From(1, 5));

            // Act
            var suggestions = await walker.VisitExpressionStatement(exprStmt);

            // Assert
            suggestions.Select(s => s.Lexeme).Should().Contain("TEST");
        }
    }
}
