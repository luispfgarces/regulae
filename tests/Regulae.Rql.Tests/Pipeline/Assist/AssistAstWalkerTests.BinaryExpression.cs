namespace Regulae.Rql.Tests.Pipeline.Assist
{
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
        public async Task VisitBinaryExpression_ReturnsEmptySuggestions()
        {
            // Arrange
            var runtime = Mock.Of<IRuntime>();
            var walker = AssistAstWalker.Create(runtime, RqlSourcePosition.Empty);

            var left = LiteralExpression.Create(LiteralType.Integer, Token.Create("1", false, 1, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 1, TokenType.INT), 1);
            var right = LiteralExpression.Create(LiteralType.Integer, Token.Create("2", false, 2, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 1, TokenType.INT), 2);
            var opToken = Token.Create("in", false, null, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 2, TokenType.IN);
            var operatorSegment = new OperatorSegment(new[] { opToken });

            var binary = new BinaryExpression(left, operatorSegment, right);

            // Act
            var suggestions = await walker.VisitBinaryExpression(binary);

            // Assert
            suggestions.Should().BeEmpty();
        }
    }
}