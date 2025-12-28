namespace Regulae.Rql.Tests.Pipeline.Assist
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using Regulae.Rql.Ast.Expressions;
    using Regulae.Rql.Ast.Segments;
    using Regulae.Rql.Pipeline.Assist;
    using Regulae.Rql.Runtime;
    using Regulae.Rql.Tokens;
    using Xunit;

    public partial class AssistAstWalkerTests
    {
        [Fact]
        public async Task VisitDatesIntervalSegment_WhenSinceMissing_SuggestsSince()
        {
            // Arrange
            var runtime = Mock.Of<IRuntime>();
            var segment = DatesIntervalSegment.Create(Expression.None, Expression.None, Expression.None, Expression.None);
            var walker = AssistAstWalker.Create(runtime, RqlSourcePosition.Empty);

            // Act
            var suggestions = await walker.VisitDatesIntervalSegment(segment);

            // Assert
            suggestions.Select(s => s.Lexeme).Should().Contain(nameof(TokenType.SINCE));
        }

        [Fact]
        public async Task VisitDatesIntervalSegment_WhenSincePresentWithoutDate_ReturnsEmpty()
        {
            // Arrange
            var runtime = Mock.Of<IRuntime>();
            var sinceDate = Expression.None;
            var untilDate = Expression.None;
            var sinceKw = KeywordExpression.Create(Token.Create("SINCE", false, null, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 5, TokenType.SINCE));
            var untilKw = Expression.None;

            var segment = DatesIntervalSegment.Create(sinceKw, sinceDate, untilKw, untilDate);
            var walker = AssistAstWalker.Create(runtime, RqlSourcePosition.Empty);

            // Act
            var suggestions = await walker.VisitDatesIntervalSegment(segment);

            // Assert
            suggestions.Should().BeEmpty();
        }

        [Fact]
        public async Task VisitDatesIntervalSegment_WhenSincePresentAndUntilMissing_SuggestsUntil()
        {
            // Arrange
            var runtime = Mock.Of<IRuntime>();
            var sinceDate = LiteralExpression.Create(LiteralType.DateTime, Token.Create("$2023-01-01$", false, DateTime.UtcNow, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 12, TokenType.DATE), DateTime.UtcNow);
            var untilDate = Expression.None;
            var sinceKw = KeywordExpression.Create(Token.Create("SINCE", false, null, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 5, TokenType.SINCE));
            var untilKw = Expression.None;

            var segment = DatesIntervalSegment.Create(sinceKw, sinceDate, untilKw, untilDate);
            var walker = AssistAstWalker.Create(runtime, RqlSourcePosition.Empty);

            // Act
            var suggestions = await walker.VisitDatesIntervalSegment(segment);

            // Assert
            suggestions.Select(s => s.Lexeme).Should().Contain(nameof(TokenType.UNTIL));
        }

        [Fact]
        public async Task VisitDatesIntervalSegment_WhenSinceAndUntilPresentWithoutDate_ReturnsEmpty()
        {
            // Arrange
            var runtime = Mock.Of<IRuntime>();
            var sinceDate = LiteralExpression.Create(LiteralType.DateTime, Token.Create("$2023-01-01$", false, DateTime.UtcNow, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 12, TokenType.DATE), DateTime.UtcNow);
            var untilDate = Expression.None;
            var sinceKw = KeywordExpression.Create(Token.Create("SINCE", false, null, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 5, TokenType.SINCE));
            var untilKw = KeywordExpression.Create(Token.Create("UNTIL", false, null, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 5, TokenType.UNTIL));

            var segment = DatesIntervalSegment.Create(sinceKw, sinceDate, untilKw, untilDate);
            var walker = AssistAstWalker.Create(runtime, RqlSourcePosition.Empty);

            // Act
            var suggestions = await walker.VisitDatesIntervalSegment(segment);

            // Assert
            suggestions.Should().BeEmpty();
        }

        [Fact]
        public async Task VisitDatesIntervalSegment_WhenSinceAndUntilPresent_StoresDatesAndReturnsEmpty()
        {
            // Arrange
            var runtime = Mock.Of<IRuntime>();
            var sinceDate = LiteralExpression.Create(LiteralType.DateTime, Token.Create("$2023-01-01$", false, DateTime.UtcNow, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 12, TokenType.DATE), DateTime.UtcNow);
            var untilDate = LiteralExpression.Create(LiteralType.DateTime, Token.Create("$2023-02-01$", false, DateTime.UtcNow, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 12, TokenType.DATE), DateTime.UtcNow);
            var sinceKw = KeywordExpression.Create(Token.Create("SINCE", false, null, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 5, TokenType.SINCE));
            var untilKw = KeywordExpression.Create(Token.Create("UNTIL", false, null, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 5, TokenType.UNTIL));

            var segment = DatesIntervalSegment.Create(sinceKw, sinceDate, untilKw, untilDate);
            var walker = AssistAstWalker.Create(runtime, RqlSourcePosition.Empty);

            // Act
            var suggestions = await walker.VisitDatesIntervalSegment(segment);

            // Assert
            suggestions.Should().BeEmpty();
        }
    }
}