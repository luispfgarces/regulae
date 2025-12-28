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
        public async Task VisitSearchExpression_WhenRulesKeywordMissing_SuggestsRULES()
        {
            // Arrange
            var runtime = Mock.Of<IRuntime>();
            var walker = AssistAstWalker.Create(runtime, RqlSourcePosition.Empty);
            var searchKw = KeywordExpression.Create(Token.Create("SEARCH", false, null, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 6, TokenType.SEARCH));
            var expr = SearchExpression.Create(searchKw, Expression.None, Segment.None, Segment.None, Segment.None);

            // Act
            var suggestions = await walker.VisitSearchExpression(expr);

            // Assert
            suggestions.Select(s => s.Lexeme).Should().Contain(nameof(TokenType.RULES));
        }

        [Fact]
        public async Task VisitSearchExpression_WhenUntilDateFollowedByNonSemicolon_SuggestsWHEN()
        {
            // Arrange
            var runtime = Mock.Of<IRuntime>();
            var walker = AssistAstWalker.Create(runtime, RqlSourcePosition.Empty);

            var searchKw = KeywordExpression.Create(Token.Create("SEARCH", false, null, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 6, TokenType.SEARCH));
            var rulesKw = KeywordExpression.Create(Token.Create("RULES", false, null, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 5, TokenType.RULES));

            var sinceKw = KeywordExpression.Create(Token.Create("SINCE", false, null, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 5, TokenType.SINCE));
            var sinceDate = LiteralExpression.Create(LiteralType.DateTime, Token.Create("$2023-01-01$", false, DateTime.UtcNow, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 12, TokenType.DATE), DateTime.UtcNow);
            var untilKw = KeywordExpression.Create(Token.Create("UNTIL", false, null, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 5, TokenType.UNTIL));
            var untilToken = Token.Create("$2024-01-01$", false, DateTime.UtcNow, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 12, TokenType.DATE);
            untilToken.Next = Token.Create("X", false, null, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 1, TokenType.IDENTIFIER);
            var untilDate = LiteralExpression.Create(LiteralType.DateTime, untilToken, DateTime.UtcNow);
            var dates = DatesIntervalSegment.Create(sinceKw, sinceDate, untilKw, untilDate);

            var expr = SearchExpression.Create(searchKw, rulesKw, Segment.None, dates, Segment.None);

            // Act
            var suggestions = await walker.VisitSearchExpression(expr);

            // Assert
            suggestions.Select(s => s.Lexeme).Should().Contain(nameof(TokenType.WHEN));
        }

        [Fact]
        public async Task VisitSearchExpression_WhenRulesetAcceptsSuggestions_ReturnsThem()
        {
            // Arrange
            var runtime = Mock.Of<IRuntime>();
            var walker = AssistAstWalker.Create(runtime, RqlSourcePosition.Empty);

            var searchKw = KeywordExpression.Create(Token.Create("SEARCH", false, null, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 6, TokenType.SEARCH));
            var rulesKw = KeywordExpression.Create(Token.Create("RULES", false, null, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 5, TokenType.RULES));

            // Provide a ruleset segment that will cause VisitRulesetSegment to suggest FOR (ForKeyword == Expression.None)
            var rulesetSegment = RulesetSegment.Create(Expression.None, Expression.None);

            var expr = SearchExpression.Create(searchKw, rulesKw, rulesetSegment, Segment.None, Segment.None);

            // Act
            var suggestions = await walker.VisitSearchExpression(expr);

            // Assert
            suggestions.Select(s => s.Lexeme).Should().Contain(nameof(TokenType.FOR));
        }

        [Fact]
        public async Task VisitSearchExpression_WhenDatesIntervalAcceptsSuggestions_ReturnsThem()
        {
            // Arrange
            var runtime = Mock.Of<IRuntime>();
            var walker = AssistAstWalker.Create(runtime, RqlSourcePosition.Empty);

            var searchKw = KeywordExpression.Create(Token.Create("SEARCH", false, null, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 6, TokenType.SEARCH));
            var rulesKw = KeywordExpression.Create(Token.Create("RULES", false, null, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 5, TokenType.RULES));

            // ruleset does not provide suggestions
            var ruleset = Segment.None;

            // Dates interval missing since -> VisitDatesIntervalSegment suggests SINCE
            var dates = DatesIntervalSegment.Create(Expression.None, Expression.None, Expression.None, Expression.None);

            var expr = SearchExpression.Create(searchKw, rulesKw, ruleset, dates, Segment.None);

            // Act
            var suggestions = await walker.VisitSearchExpression(expr);

            // Assert
            suggestions.Select(s => s.Lexeme).Should().Contain(nameof(TokenType.SINCE));
        }

        [Fact]
        public async Task VisitSearchExpression_WhenInputConditionsProvideSuggestions_ReturnsThem()
        {
            // Arrange
            var runtime = Mock.Of<IRuntime>();
            var walker = AssistAstWalker.Create(runtime, RqlSourcePosition.Empty);

            var searchKw = KeywordExpression.Create(Token.Create("SEARCH", false, null, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 6, TokenType.SEARCH));
            var rulesKw = KeywordExpression.Create(Token.Create("RULES", false, null, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 5, TokenType.RULES));

            var ruleset = Segment.None;
            var dates = Segment.None;

            // Build an input condition segment that will suggest IS (operator == Token.None)
            var placeholder = new PlaceholderExpression(Token.Create("@p", false, "p", RqlSourcePosition.From(2, 1), RqlSourcePosition.From(2, 2), 2, TokenType.PLACEHOLDER));
            var right = LiteralExpression.Create(LiteralType.Bool, Token.Create("true", false, true, RqlSourcePosition.From(2, 3), RqlSourcePosition.From(2, 6), 4, TokenType.BOOL), true);
            var inputCondition = new InputConditionSegment(placeholder, Token.None, right);
            var begin = Token.Create("{", false, null, RqlSourcePosition.From(2, 0), RqlSourcePosition.From(2, 0), 1, TokenType.BRACE_LEFT);
            var end = Token.Create("}", false, null, RqlSourcePosition.From(2, 7), RqlSourcePosition.From(2, 7), 1, TokenType.BRACE_RIGHT);
            var inputConditions = InputConditionsSegment.Create(Expression.None, begin, [inputCondition], end);

            var expr = SearchExpression.Create(searchKw, rulesKw, ruleset, dates, inputConditions);

            // Act
            var suggestions = await walker.VisitSearchExpression(expr);

            // Assert
            suggestions.Select(s => s.Lexeme).Should().Contain(nameof(TokenType.IS));
        }

        [Fact]
        public async Task VisitSearchExpression_WhenNothingToSuggest_ReturnsEmpty()
        {
            // Arrange
            var runtime = Mock.Of<IRuntime>();
            var walker = AssistAstWalker.Create(runtime, RqlSourcePosition.Empty);

            var searchKw = KeywordExpression.Create(Token.Create("SEARCH", false, null, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 6, TokenType.SEARCH));
            var rulesKw = KeywordExpression.Create(Token.Create("RULES", false, null, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 5, TokenType.RULES));

            // All child segments are 'none' so no suggestions expected
            var expr = SearchExpression.Create(searchKw, rulesKw, Segment.None, Segment.None, Segment.None);

            // Act
            var suggestions = await walker.VisitSearchExpression(expr);

            // Assert
            suggestions.Should().BeEmpty();
        }
    }
}