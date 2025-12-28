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
        public static IEnumerable<object[]> TestDataRulesetSegment { get; } =
        [
            [null!, null!, new string[] { nameof(TokenType.FOR), },],
            [nameof(TokenType.FOR), null!, new string[] { "\"MyRules\"" },],
            [nameof(TokenType.FOR), "\"MyRules\"", Array.Empty<string>(),],
        ];

        [Theory]
        [MemberData(nameof(TestDataRulesetSegment))]
        public async Task VisitRulesetSegment_WhenRulesetNameMissing_ReturnsRuntimeRulesets(
            string? forKeyword,
            string? rulesetLiteral,
            string[] outputSuggestions)
        {
            // Arrange
            var runtime = Mock.Of<IRuntime>();
            var rulesetName = "MyRules";
            var rqlRuleset = new RqlRuleset(new Ruleset(rulesetName, DateTime.UtcNow));
            var rqlArray = new RqlArray(1);
            rqlArray.SetAtIndex(0, rqlRuleset);
            Mock.Get(runtime).Setup(r => r.GetRulesetsAsync()).Returns(new ValueTask<RqlArray>(rqlArray));

            var forExpression = forKeyword is null
                ? Expression.None
                : KeywordExpression.Create(
                    Token.Create(
                        forKeyword,
                        false,
                        null,
                        RqlSourcePosition.From(1, 1),
                        RqlSourcePosition.From(1, 4),
                        (uint)forKeyword.Length,
                        Enum.Parse<TokenType>(forKeyword)));
            var rulesetExpression = rulesetLiteral is null
                ? Expression.None
                : LiteralExpression.Create(
                    LiteralType.String,
                    Token.Create(
                        rulesetLiteral,
                        true,
                        null,
                        RqlSourcePosition.From(1, 5),
                        RqlSourcePosition.From(1, 13),
                        (uint)rulesetLiteral.Length,
                        TokenType.STRING),
                        rulesetLiteral);

            var segment = RulesetSegment.Create(forExpression, rulesetExpression);

            var walker = AssistAstWalker.Create(runtime, RqlSourcePosition.Empty);

            // Act
            var suggestions = await walker.VisitRulesetSegment(segment);

            // Assert
            if (outputSuggestions.Length != 0)
            {
                suggestions.Select(s => s.Lexeme).Should().Contain(outputSuggestions);
            }
            else
            {
                suggestions.Should().BeEmpty();
            }
        }
    }
}
