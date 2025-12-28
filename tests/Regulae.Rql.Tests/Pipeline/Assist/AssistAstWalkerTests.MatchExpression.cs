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
    using Regulae.Rql.Runtime.Types;
    using Regulae.Rql.Tokens;
    using Xunit;

    public partial class AssistAstWalkerTests
    {
        private static readonly RqlSourcePosition AnyPosition = RqlSourcePosition.From(0, 0);

        [Fact]
        public async Task VisitMatchExpression_CardinalityMissing_ReturnsAllAndOne()
        {
            // Arrange
            var runtimeMock = new Mock<IRuntime>(MockBehavior.Strict);

            var cardinalitySegment = new CardinalitySegment(Expression.None, Expression.None);

            var matchExpression = MatchExpression.Create(Expression.None, cardinalitySegment, Segment.None, Segment.None, Segment.None);

            var walker = AssistAstWalker.Create(runtimeMock.Object, AnyPosition);

            // Act
            var suggestions = await walker.VisitMatchExpression(matchExpression);

            // Assert
            suggestions.Should().NotBeNull();
            var lexemes = suggestions.Select(s => s.Lexeme).ToArray();
            lexemes.Should().Contain(nameof(TokenType.ALL));
            lexemes.Should().Contain(nameof(TokenType.ONE));
        }

        [Fact]
        public async Task VisitMatchExpression_RulesetForMissing_ReturnsForKeyword()
        {
            // Arrange
            var runtimeMock = new Mock<IRuntime>(MockBehavior.Strict);

            var rulesetSegment = new RulesetSegment(Expression.None, Expression.None);

            var matchExpression = MatchExpression.Create(Expression.None, Segment.None, rulesetSegment, Segment.None, Segment.None);

            var walker = AssistAstWalker.Create(runtimeMock.Object, AnyPosition);

            // Act
            var suggestions = await walker.VisitMatchExpression(matchExpression);

            // Assert
            suggestions.Should().NotBeNull();
            var lexemes = suggestions.Select(s => s.Lexeme).ToArray();
            lexemes.Should().Contain(nameof(TokenType.FOR));
        }

        [Fact]
        public async Task VisitMatchExpression_RulesetNameMissing_QueriesRuntimeAndReturnsNames()
        {
            // Arrange
            var ruleset = new RqlRuleset(new Ruleset("my-ruleset", DateTime.UtcNow));

            var runtimeMock = new Mock<IRuntime>();
            runtimeMock
                .Setup(r => r.GetRulesetsAsync())
                .ReturnsAsync(() =>
                {
                    var array = new RqlArray(1);
                    array.SetAtIndex(0, ruleset);
                    return array;
                });

            var rulesetSegment = new RulesetSegment(KeywordExpression.Create(NewToken(TokenType.FOR)), Expression.None);

            var matchExpression = MatchExpression.Create(Expression.None, Segment.None, rulesetSegment, Segment.None, Segment.None);

            var walker = AssistAstWalker.Create(runtimeMock.Object, AnyPosition);

            // Act
            var suggestions = await walker.VisitMatchExpression(matchExpression);

            // Assert
            suggestions.Should().NotBeNull();
            var lexemes = suggestions.Select(s => s.Lexeme).ToArray();
            lexemes.Should().Contain(@"""my-ruleset""");
        }

        [Fact]
        public async Task VisitMatchExpression_InputConditionsMissingOperator_ReturnsIs()
        {
            // Arrange
            var runtimeMock = new Mock<IRuntime>(MockBehavior.Strict);

            var identifier = new IdentifierExpression(NewToken(nameof(TokenType.IDENTIFIER), "field", TokenType.IDENTIFIER));
            var inputCondition = new InputConditionSegment(identifier, Token.None, Expression.None);

            var inputConditionsSegment = new InputConditionsSegment(Expression.None, NewToken(TokenType.BRACE_LEFT), [inputCondition], NewToken(TokenType.BRACE_RIGHT));

            var matchExpression = MatchExpression.Create(Expression.None, Segment.None, Segment.None, Segment.None, inputConditionsSegment);

            var walker = AssistAstWalker.Create(runtimeMock.Object, AnyPosition);

            // Act
            var suggestions = await walker.VisitMatchExpression(matchExpression);

            // Assert
            suggestions.Should().NotBeNull();
            var lexemes = suggestions.Select(s => s.Lexeme).ToArray();
            lexemes.Should().Contain(nameof(TokenType.IS));
        }

        [Fact]
        public async Task VisitMatchExpression_NothingMatches_ReturnsEmpty()
        {
            // Arrange
            var runtimeMock = new Mock<IRuntime>(MockBehavior.Strict);

            var matchExpression = MatchExpression.Create(Expression.None, Segment.None, Segment.None, Segment.None, Segment.None);

            var walker = AssistAstWalker.Create(runtimeMock.Object, AnyPosition);

            // Act
            var suggestions = await walker.VisitMatchExpression(matchExpression);

            // Assert
            suggestions.Should().NotBeNull().And.BeEmpty();
        }

        [Fact]
        public async Task VisitMatchExpression_OnKeywordMissing_ReturnsOnKeyword()
        {
            // Arrange
            var runtime = Mock.Of<IRuntime>();
            var walker = AssistAstWalker.Create(runtime, RqlSourcePosition.Empty);

            var matchDate = MatchDateSegment.Create(Expression.None, Expression.None);

            var matchExpr = MatchExpression.Create(
                Expression.None,
                Segment.None,
                Segment.None,
                matchDate,
                Segment.None);

            // Act
            var suggestions = await walker.VisitMatchExpression(matchExpr);

            // Assert
            suggestions.Select(s => s.Lexeme).Should().Contain("ON");
        }

        [Fact]
        public async Task VisitMatchExpression_WhenDateFollowedByNonSemicolon_SuggestsWhen()
        {
            // Arrange
            var runtime = Mock.Of<IRuntime>();
            var walker = AssistAstWalker.Create(runtime, RqlSourcePosition.Empty);

            // Create a date literal token and a following token that is not semicolon
            var date = DateTime.UtcNow;
            var dateToken = Token.Create("$dt$", false, date, RqlSourcePosition.From(1, 10), RqlSourcePosition.From(1, 20), 6, TokenType.DATE);
            var nextToken = Token.Create("ABC", false, null, RqlSourcePosition.From(1, 21), RqlSourcePosition.From(1, 24), 3, TokenType.IDENTIFIER);
            dateToken.Next = nextToken;

            var dateLiteral = LiteralExpression.Create(LiteralType.DateTime, dateToken, date);
            var onToken = Token.Create("ON", false, null, RqlSourcePosition.From(1, 6), RqlSourcePosition.From(1, 8), 2, TokenType.ON);
            var matchDate = MatchDateSegment.Create(KeywordExpression.Create(onToken), dateLiteral);

            var matchKeywordToken = Token.Create("MATCH", false, null, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 5, TokenType.MATCH);
            var matchExpr = MatchExpression.Create(
                KeywordExpression.Create(matchKeywordToken),
                Segment.None,
                Segment.None,
                matchDate,
                Segment.None);

            // Act
            var suggestions = await walker.VisitMatchExpression(matchExpr);

            // Assert
            suggestions.Select(s => s.Lexeme).Should().Contain("WHEN");
        }
    }
}
