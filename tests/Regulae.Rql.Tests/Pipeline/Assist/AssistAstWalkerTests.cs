namespace Regulae.Rql.Tests.Pipeline.Assist
{
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using Regulae.Rql.Ast.Statements;
    using Regulae.Rql.Pipeline.Assist;
    using Regulae.Rql.Runtime;
    using Regulae.Rql.Tokens;
    using Xunit;

    public partial class AssistAstWalkerTests
    {
        [Fact]
        public async Task ProvideAssistSuggestionsAsync_ReturnsSuggestionsFromStatementContainingPosition()
        {
            // Arrange
            var runtime = Mock.Of<IRuntime>();

            var begin = RqlSourcePosition.From(1, 1);
            var end = RqlSourcePosition.From(1, 10);
            var stubStatementMock = new Mock<Statement>(begin, end);
            // Setup Accept to return some assist suggestion set when visited
            var walkerPos = RqlSourcePosition.From(1, 5);
            var walker = AssistAstWalker.Create(runtime, walkerPos);

            var suggestion = AssistSuggestion.New("ARRAY");
            stubStatementMock.Setup(s => s.Accept(It.IsAny<IStatementVisitor<Task<IAssistSuggestion[]>>>()))
                .ReturnsAsync([suggestion]);

            var suggestions = await walker.ProvideAssistSuggestionsAsync([stubStatementMock.Object]);

            // Assert
            suggestions.Should().NotBeNull();
            suggestions.Should().ContainSingle();
            suggestions[0].Lexeme.Should().Be("ARRAY");
        }

        [Fact]
        public async Task ProvideAssistSuggestionsAsync_WhenNoStatements_ReturnsEmptySuggestions()
        {
            // Arrange
            var runtime = Mock.Of<IRuntime>();
            var walkerPos = RqlSourcePosition.From(1, 5);
            var walker = AssistAstWalker.Create(runtime, walkerPos);

            var suggestions = await walker.ProvideAssistSuggestionsAsync([]);

            // Assert
            suggestions.Should().NotBeNull().And.BeEmpty();
        }

        private static Token NewToken(string lexeme, object? value, TokenType type)
            => Token.Create(lexeme, false, value, RqlSourcePosition.Empty, RqlSourcePosition.Empty, (uint)lexeme.Length, type);

        private static Token NewToken(TokenType type)
            => Token.Create(type.ToString(), false, null!, RqlSourcePosition.Empty, RqlSourcePosition.Empty, (uint)type.ToString().Length, type);
    }
}