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
        public async Task VisitInputConditionsSegment_WhenBeginTokenNone_ReturnsEmpty()
        {
            // Arrange
            var runtime = Mock.Of<IRuntime>();
            var seg = InputConditionsSegment.Create(Expression.None, Token.None, Array.Empty<Segment>(), Token.None);
            var walker = AssistAstWalker.Create(runtime, RqlSourcePosition.Empty);

            // Act
            var suggestions = await walker.VisitInputConditionsSegment(seg);

            // Assert
            suggestions.Should().BeEmpty();
        }

        [Fact]
        public async Task VisitInputConditionsSegment_WhenChildProvidesSuggestion_ReturnsFirstNonEmpty()
        {
            // Arrange
            var runtime = Mock.Of<IRuntime>();
            var begin = Token.Create("{", false, null, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 1, TokenType.BRACE_LEFT);

            var left = new PlaceholderExpression(Token.Create("@p", false, "p", RqlSourcePosition.Empty, RqlSourcePosition.Empty, 2, TokenType.PLACEHOLDER));
            var right = LiteralExpression.Create(LiteralType.Integer, Token.Create("1", false, 1, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 1, TokenType.INT), 1);
            var inputCondition = new InputConditionSegment(left, Token.None, right);

            var end = Token.Create("}", false, null, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 1, TokenType.BRACE_RIGHT);
            var seg = InputConditionsSegment.Create(Expression.None, begin, new Segment[] { inputCondition }, end);
            var walker = AssistAstWalker.Create(runtime, RqlSourcePosition.Empty);

            // Act
            var suggestions = await walker.VisitInputConditionsSegment(seg);

            // Assert
            suggestions.Select(s => s.Lexeme).Should().Contain(nameof(TokenType.IS));
        }
    }
}