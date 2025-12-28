namespace Regulae.Rql.Tests.Pipeline.Assist
{
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using Regulae.Rql.Ast.Expressions;
    using Regulae.Rql.Pipeline.Assist;
    using Regulae.Rql.Runtime;
    using Regulae.Rql.Tokens;
    using Xunit;

    public partial class AssistAstWalkerTests
    {
        [Fact]
        public async Task VisitNewArrayExpression_WhenInitializerBeginTokenNone_ReturnsEmpty()
        {
            // Arrange
            var runtime = Mock.Of<IRuntime>();
            var tokenArray = Token.Create("ARRAY", false, null, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 5, TokenType.ARRAY);
            var expr = NewArrayExpression.Create(tokenArray, Token.None, Expression.None, [], Token.None);
            var walker = AssistAstWalker.Create(runtime, RqlSourcePosition.Empty);

            // Act
            var suggestions = await walker.VisitNewArrayExpression(expr);

            // Assert
            suggestions.Should().BeEmpty();
        }

        [Fact]
        public async Task VisitNewArrayExpression_WhenInitializerBeginTokenBracketLeft_ReturnsEmpty()
        {
            // Arrange
            var runtime = Mock.Of<IRuntime>();
            var tokenArray = Token.Create("ARRAY", false, null, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 5, TokenType.ARRAY);
            var initBegin = Token.Create("[", false, null, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 1, TokenType.BRACKET_LEFT);
            var expr = NewArrayExpression.Create(tokenArray, initBegin, Expression.None, [], Token.None);
            var walker = AssistAstWalker.Create(runtime, RqlSourcePosition.Empty);

            // Act
            var suggestions = await walker.VisitNewArrayExpression(expr);

            // Assert
            suggestions.Should().BeEmpty();
        }

        [Fact]
        public async Task VisitNewArrayExpression_WhenInitializerBeginTokenBraceLeftAndEmptyValues_ReturnsEmpty()
        {
            var runtime = Mock.Of<IRuntime>();
            var tokenArray = Token.Create("ARRAY", false, null, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 5, TokenType.ARRAY);
            var initBegin = Token.Create("{", false, null, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 1, TokenType.BRACE_LEFT);
            var initEnd = Token.Create("}", false, null, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 1, TokenType.BRACE_RIGHT);
            var expr = NewArrayExpression.Create(tokenArray, initBegin, Expression.None, [], initEnd);

            var walker = AssistAstWalker.Create(runtime, RqlSourcePosition.Empty);

            // Act
            var suggestions = await walker.VisitNewArrayExpression(expr);

            // Assert
            suggestions.Should().BeEmpty();
        }

        [Fact]
        public async Task VisitNewArrayExpression_WhenLastValueIsIdentifier_ReturnsExpressionSuggestions()
        {
            // Arrange
            var runtime = Mock.Of<IRuntime>();
            var tokenArray = Token.Create("ARRAY", false, null, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 5, TokenType.ARRAY);
            var initBegin = Token.Create("{", false, null, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 1, TokenType.BRACE_LEFT);
            var initEnd = Token.Create("}", false, null, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 1, TokenType.BRACE_RIGHT);
            var lastValue = new IdentifierExpression(Token.Create("id", false, null, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 2, TokenType.IDENTIFIER));
            var expr = NewArrayExpression.Create(tokenArray, initBegin, Expression.None, [lastValue], initEnd);

            var walker = AssistAstWalker.Create(runtime, RqlSourcePosition.Empty);

            // Act
            var suggestions = await walker.VisitNewArrayExpression(expr);

            // Assert
            suggestions.Select(s => s.Lexeme).Should().Contain([nameof(TokenType.ARRAY), nameof(TokenType.MATCH), nameof(TokenType.NOTHING), nameof(TokenType.OBJECT), nameof(TokenType.SEARCH)]);
        }
    }
}