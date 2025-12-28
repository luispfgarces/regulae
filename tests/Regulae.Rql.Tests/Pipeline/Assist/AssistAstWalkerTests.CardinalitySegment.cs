namespace Regulae.Rql.Tests.Pipeline.Assist
{
    using System;
    using System.Collections.Generic;
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
        public static IEnumerable<object[]> TestDataCardinalitySegment { get; } =
        [
            [null!, null!, new string[] { nameof(TokenType.ONE), nameof(TokenType.ALL), },],
            [nameof(TokenType.ONE), null!, new string[] { nameof(TokenType.RULE) },],
            [nameof(TokenType.ALL), null!, new string[] { nameof(TokenType.RULES) },],
            [nameof(TokenType.ALL), nameof(TokenType.RULES), Array.Empty<string>(),],
        ];

        [Theory]
        [MemberData(nameof(TestDataCardinalitySegment))]
        public async Task VisitCardinalitySegment_WhenMissingCardinality_ReturnsAllAndOne(
            string? inputCardinalityKeyword,
            string? inputRuleKeyword,
            string[] outputSuggestions)
        {
            // Arrange
            var runtime = Mock.Of<IRuntime>();
            var walker = AssistAstWalker.Create(runtime, RqlSourcePosition.Empty);
            var cardinalityKeyword = inputCardinalityKeyword is null
                ? Expression.None
                : KeywordExpression.Create(
                    Token.Create(
                        inputCardinalityKeyword,
                        false,
                        null!,
                        RqlSourcePosition.Empty,
                        RqlSourcePosition.Empty,
                        (uint)inputCardinalityKeyword.Length,
                        Enum.Parse<TokenType>(inputCardinalityKeyword)));
            var ruleKeyword = inputRuleKeyword is null
                ? Expression.None
                : KeywordExpression.Create(
                    Token.Create(
                        inputRuleKeyword,
                        false,
                        null!,
                        RqlSourcePosition.Empty,
                        RqlSourcePosition.Empty,
                        (uint)inputRuleKeyword.Length,
                        Enum.Parse<TokenType>(inputRuleKeyword)));

            var segment = CardinalitySegment.Create(cardinalityKeyword, ruleKeyword);

            // Act
            var suggestions = await walker.VisitCardinalitySegment(segment);

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
