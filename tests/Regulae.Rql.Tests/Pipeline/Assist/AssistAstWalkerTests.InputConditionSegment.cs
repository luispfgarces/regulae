namespace Regulae.Rql.Tests.Pipeline.Assist
{
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
        public async Task VisitInputConditionSegment_WhenOperatorMissing_SuggestsIS()
        {
            // Arrange
            var runtime = Mock.Of<IRuntime>();
            var left = new PlaceholderExpression(Token.Create("@x", false, "x", RqlSourcePosition.Empty, RqlSourcePosition.Empty, 2, TokenType.PLACEHOLDER));
            var right = LiteralExpression.Create(LiteralType.Bool, Token.Create("true", false, true, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 4, TokenType.BOOL), true);
            var segment = new InputConditionSegment(left, Token.None, right);
            var walker = AssistAstWalker.Create(runtime, RqlSourcePosition.Empty);

            // Act
            var suggestions = await walker.VisitInputConditionSegment(segment);

            // Assert
            suggestions.Select(s => s.Lexeme).Should().Contain(nameof(TokenType.IS));
        }

        [Fact]
        public async Task VisitInputConditionSegment_WhenPositionIsAtPlaceholder_DelegatesToPlaceholderAndReturnsEmpty()
        {
            // Arrange
            var runtime = Mock.Of<IRuntime>();

            // Placeholder token spans line 1 cols 1..3
            var begin = RqlSourcePosition.From(1, 1);
            var end = RqlSourcePosition.From(1, 3);
            var placeholderToken = Token.Create("@p", false, "p", begin, end, 2, TokenType.PLACEHOLDER);
            var left = new PlaceholderExpression(placeholderToken);

            var right = LiteralExpression.Create(LiteralType.String, Token.Create("\"v\"", true, "v", RqlSourcePosition.From(1, 4), RqlSourcePosition.From(1, 6), 3, TokenType.STRING), "v");
            var segment = new InputConditionSegment(left, Token.None, right);

            // Choose a walker position contained by the placeholder (line 1, column 2)
            var walker = AssistAstWalker.Create(runtime, RqlSourcePosition.From(1, 2));

            // Act
            var suggestions = await walker.VisitInputConditionSegment(segment);

            // Assert - no stored context so placeholder visitor returns empty
            suggestions.Should().BeEmpty();
        }

        [Fact]
        public async Task VisitInputConditionSegment_WhenPlaceholderAndOperatorPresent_ReturnsEmpty()
        {
            // Arrange
            var runtime = Mock.Of<IRuntime>();

            var begin = RqlSourcePosition.From(1, 1);
            var end = RqlSourcePosition.From(1, 3);
            var placeholderToken = Token.Create("@p", false, "p", begin, end, 2, TokenType.PLACEHOLDER);
            var left = new PlaceholderExpression(placeholderToken);

            // Operator present (IS)
            var operatorToken = Token.Create("is", false, null, RqlSourcePosition.From(1, 4), RqlSourcePosition.From(1, 5), 2, TokenType.IS);
            var right = LiteralExpression.Create(LiteralType.Integer, Token.Create("1", false, 1, RqlSourcePosition.From(1, 6), RqlSourcePosition.From(1, 6), 1, TokenType.INT), 1);

            var segment = new InputConditionSegment(left, operatorToken, right);

            // Walker position not inside placeholder -> should evaluate operator branch and return empty suggestions
            var walker = AssistAstWalker.Create(runtime, RqlSourcePosition.Empty);

            // Act
            var suggestions = await walker.VisitInputConditionSegment(segment);

            // Assert
            suggestions.Should().BeEmpty();
        }
    }
}
