
namespace Regulae.Rql.Tests.Pipeline.Assist
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using Regulae.Rql;
    using Regulae.Rql.Ast.Statements;
    using Regulae.Rql.Pipeline.Assist;
    using Regulae.Rql.Runtime;
    using Regulae.Rql.Tokens;
    using Xunit;

    public class AssistEngineTests
    {
        [Fact]
        public async Task ProcessAssistAsync_WhenNoTokens_ReturnsEmpty()
        {
            // Arrange
            var runtime = Mock.Of<IRuntime>();
            var engine = new AssistEngine(runtime);
            var tokens = Array.Empty<Token>();
            var statements = Array.Empty<Statement>();
            var position = RqlSourcePosition.From(1, 1);

            // Act
            var actual = await engine.ProcessAssistAsync(tokens, statements, position);

            // Assert
            actual.Should().NotBeNull().And.BeEmpty();
        }

        [Fact]
        public async Task ProcessAssistAsync_WhenTokenNotFoundAtPosition_ReturnsEmpty()
        {
            // Arrange
            var runtime = Mock.Of<IRuntime>();
            var engine = new AssistEngine(runtime);

            var token = Token.Create("abc", false, null, RqlSourcePosition.From(1, 1), RqlSourcePosition.From(1, 2), 3, TokenType.IDENTIFIER);
            var tokens = new[] { token };
            var statements = Array.Empty<Statement>();

            var position = RqlSourcePosition.From(1, 5);

            // Act
            var actual = await engine.ProcessAssistAsync(tokens, statements, position);

            // Assert
            actual.Should().NotBeNull().And.BeEmpty();
        }

        [Fact]
        public async Task ProcessAssistAsync_WhenTokenBeginCharNotAllowed_ReturnsEmpty()
        {
            // Arrange
            var runtime = Mock.Of<IRuntime>();
            var engine = new AssistEngine(runtime);

            var token = Token.Create("1abc", false, null, RqlSourcePosition.From(1, 1), RqlSourcePosition.From(1, 5), 4, TokenType.IDENTIFIER);
            var tokens = new[] { token };
            var statements = Array.Empty<Statement>();
            var position = RqlSourcePosition.From(1, 3);

            // Act
            var actual = await engine.ProcessAssistAsync(tokens, statements, position);

            // Assert
            actual.Should().NotBeNull().And.BeEmpty();
        }

        [Fact]
        public async Task ProcessAssistAsync_WhenAstProvidesSuggestions_FiltersByTokenLexeme()
        {
            // Arrange
            var runtime = Mock.Of<IRuntime>();
            var engine = new AssistEngine(runtime);

            var token = Token.Create("a", false, null, RqlSourcePosition.From(1, 1), RqlSourcePosition.From(1, 1), 1, TokenType.IDENTIFIER);
            var tokens = new[] { token };
            var position = RqlSourcePosition.From(1, 1);

            var suggestion1 = AssistSuggestion.New("\"Apple\"");
            var suggestion2 = AssistSuggestion.New("Banana");
            var suggestion3 = AssistSuggestion.New("apricot");

            var statementMock = new Mock<Statement>(RqlSourcePosition.From(1, 1), RqlSourcePosition.From(1, 1));
            statementMock
                .Setup(s => s.Accept(It.IsAny<IStatementVisitor<Task<IAssistSuggestion[]>>>()))
                .ReturnsAsync([suggestion1, suggestion2, suggestion3]);

            var statements = new[] { statementMock.Object };

            // Act
            var actual = await engine.ProcessAssistAsync(tokens, statements, position);

            // Assert
            var lexemes = actual.Select(s => s.Lexeme).ToArray();
            lexemes.Should().Contain("\"Apple\"");
            lexemes.Should().Contain("apricot");
            lexemes.Should().NotContain("Banana");
            actual.Count.Should().Be(2);
        }

        [Fact]
        public async Task ProcessAssistAsync_WhenAstReturnsNoSuggestions_ReturnsEmpty()
        {
            // Arrange
            var runtime = Mock.Of<IRuntime>();
            var engine = new AssistEngine(runtime);

            var token = Token.Create("a", false, null, RqlSourcePosition.From(1, 1), RqlSourcePosition.From(1, 1), 1, TokenType.IDENTIFIER);
            var tokens = new[] { token };
            var position = RqlSourcePosition.From(1, 1);

            var statementMock = new Mock<Statement>(RqlSourcePosition.Empty, RqlSourcePosition.Empty);
            statementMock
                .Setup(s => s.Accept(It.IsAny<IStatementVisitor<Task<IAssistSuggestion[]>>>()))
                .ReturnsAsync([]);

            var statements = new[] { statementMock.Object };

            // Act
            var actual = await engine.ProcessAssistAsync(tokens, statements, position);

            // Assert
            actual.Should().BeEmpty();
        }
    }
}