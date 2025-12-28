namespace Regulae.Rql.Tests.Pipeline.Assist
{
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
        [Fact]
        public async Task VisitPlaceholderExpression_WithStoredRulesetAndMatchDate_ReturnsUniqueConditions()
        {
            // Arrange
            var runtime = Mock.Of<IRuntime>();
            var walker = AssistAstWalker.Create(runtime, RqlSourcePosition.Empty);

            // Prepare ruleset segment to store ruleset name in context
            var rulesetName = "RS1";
            var forToken = Token.Create("FOR", false, null, RqlSourcePosition.From(1, 1), RqlSourcePosition.From(1, 3), 3, TokenType.FOR);
            var rulesetToken = Token.Create("\"RS1\"", false, rulesetName, RqlSourcePosition.From(1, 4), RqlSourcePosition.From(1, 9), 6, TokenType.STRING);
            var rulesetLiteral = LiteralExpression.Create(LiteralType.String, rulesetToken, rulesetName);
            var rulesetSegment = RulesetSegment.Create(KeywordExpression.Create(forToken), rulesetLiteral);

            // Call VisitRulesetSegment to populate storedContext["Ruleset-Name"]
            await walker.VisitRulesetSegment(rulesetSegment);

            // Prepare match date to populate storedContext["Match-Date"]
            var dt = DateTime.Parse("2024-01-01Z").ToUniversalTime();
            var dateToken = Token.Create("$2024-01-01Z$", false, dt, RqlSourcePosition.From(1, 10), RqlSourcePosition.From(1, 32), 22, TokenType.DATE);
            var dateLiteral = LiteralExpression.Create(LiteralType.DateTime, dateToken, dt);
            var onToken = Token.Create("ON", false, null, RqlSourcePosition.From(1, 8), RqlSourcePosition.From(1, 9), 2, TokenType.ON);
            var matchDateSegment = MatchDateSegment.Create(KeywordExpression.Create(onToken), dateLiteral);

            // Call VisitMatchDateSegment to populate storedContext["Match-Date"]
            await walker.VisitMatchDateSegment(matchDateSegment);

            // Mock runtime to return two unique conditions
            var conds = new RqlArray(2);
            conds.SetAtIndex(0, new RqlString("CondA"));
            conds.SetAtIndex(1, new RqlString("CondB"));
            Mock.Get(runtime).Setup(r => r.GetUniqueConditionsAsync(rulesetName, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(new ValueTask<RqlArray>(conds));

            // Create placeholder expression
            var placeholderToken = Token.Create("@p", false, "p", RqlSourcePosition.From(2, 1), RqlSourcePosition.From(2, 2), 2, TokenType.PLACEHOLDER);
            var placeholder = new PlaceholderExpression(placeholderToken);

            // Create a new walker bound to the same runtime/position but reusing stored context is not possible across instances.
            // Instead reuse the same walker variable (we populated it above).
            // Act
            var suggestions = await walker.VisitPlaceholderExpression(placeholder);

            // Assert
            suggestions.Select(s => s.Lexeme).Should().Contain(["@CondA", "@CondB"]);
        }

        [Fact]
        public async Task VisitPlaceholderExpression_WithStoredRulesetAndDatesInterval_ReturnsUniqueConditions()
        {
            // Arrange
            var runtime = Mock.Of<IRuntime>();
            var walker = AssistAstWalker.Create(runtime, RqlSourcePosition.Empty);

            // Prepare ruleset segment to store ruleset name in context
            var rulesetName = "RS1";
            var forToken = Token.Create("FOR", false, null, RqlSourcePosition.From(1, 1), RqlSourcePosition.From(1, 3), 3, TokenType.FOR);
            var rulesetToken = Token.Create("\"RS1\"", false, rulesetName, RqlSourcePosition.From(1, 4), RqlSourcePosition.From(1, 9), 6, TokenType.STRING);
            var rulesetLiteral = LiteralExpression.Create(LiteralType.String, rulesetToken, rulesetName);
            var rulesetSegment = RulesetSegment.Create(KeywordExpression.Create(forToken), rulesetLiteral);

            // Call VisitRulesetSegment to populate storedContext["Ruleset-Name"]
            await walker.VisitRulesetSegment(rulesetSegment);

            // Prepare match date to populate storedContext["Since-Date"] and storedContext["Until-Date"]
            var sinceDate = DateTime.Parse("2024-01-01Z").ToUniversalTime();
            var sinceDateToken = Token.Create("$2024-01-01Z$", false, sinceDate, RqlSourcePosition.From(1, 10), RqlSourcePosition.From(1, 32), 22, TokenType.DATE);
            var sinceDateLiteral = LiteralExpression.Create(LiteralType.DateTime, sinceDateToken, sinceDate);
            var sinceToken = NewToken(TokenType.SINCE);
            var untilDate = DateTime.Parse("2025-01-01Z").ToUniversalTime();
            var untilDateToken = Token.Create("$2025-01-01Z$", false, sinceDate, RqlSourcePosition.From(1, 10), RqlSourcePosition.From(1, 32), 22, TokenType.DATE);
            var untilDateLiteral = LiteralExpression.Create(LiteralType.DateTime, sinceDateToken, sinceDate);
            var untilToken = NewToken(TokenType.UNTIL);
            var datesIntervalSegment = DatesIntervalSegment.Create(KeywordExpression.Create(sinceToken), sinceDateLiteral, KeywordExpression.Create(untilToken), untilDateLiteral);

            // Call VisitDatesIntervalSegment to populate storedContext["Since-Date"] and storedContext["Until-Date"]
            await walker.VisitDatesIntervalSegment(datesIntervalSegment);

            // Mock runtime to return two unique conditions
            var conds = new RqlArray(2);
            conds.SetAtIndex(0, new RqlString("CondA"));
            conds.SetAtIndex(1, new RqlString("CondB"));
            Mock.Get(runtime).Setup(r => r.GetUniqueConditionsAsync(rulesetName, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(new ValueTask<RqlArray>(conds));

            // Create placeholder expression
            var placeholderToken = Token.Create("@p", false, "p", RqlSourcePosition.From(2, 1), RqlSourcePosition.From(2, 2), 2, TokenType.PLACEHOLDER);
            var placeholder = new PlaceholderExpression(placeholderToken);

            // Create a new walker bound to the same runtime/position but reusing stored context is not possible across instances.
            // Instead reuse the same walker variable (we populated it above).
            // Act
            var suggestions = await walker.VisitPlaceholderExpression(placeholder);

            // Assert
            suggestions.Select(s => s.Lexeme).Should().Contain(["@CondA", "@CondB"]);
        }

        [Fact]
        public async Task VisitPlaceholderExpression_WithoutStoredRuleset_ReturnsEmpty()
        {
            // Arrange
            var runtime = Mock.Of<IRuntime>();
            var walker = AssistAstWalker.Create(runtime, RqlSourcePosition.Empty);

            // Create placeholder expression
            var placeholderToken = Token.Create("@p", false, "p", RqlSourcePosition.From(2, 1), RqlSourcePosition.From(2, 2), 2, TokenType.PLACEHOLDER);
            var placeholder = new PlaceholderExpression(placeholderToken);

            // Act
            var suggestions = await walker.VisitPlaceholderExpression(placeholder);

            // Assert
            suggestions.Select(s => s.Lexeme).Should().BeEmpty();
        }
    }
}