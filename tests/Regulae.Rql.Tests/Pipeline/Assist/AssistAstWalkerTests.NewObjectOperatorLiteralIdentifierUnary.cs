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
        public async Task VisitNewObjectExpression_ReturnsEmpty()
        {
            // Arrange
            var runtime = Mock.Of<IRuntime>();
            var objToken = Token.Create("OBJECT", false, null, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 6, TokenType.OBJECT);
            var expr = new NewObjectExpression(objToken, new Expression[0]);
            var walker = AssistAstWalker.Create(runtime, RqlSourcePosition.Empty);

            // Act
            var suggestions = await walker.VisitNewObjectExpression(expr);

            // Assert
            suggestions.Should().BeEmpty();
        }

        [Fact]
        public async Task VisitOperatorSegment_ReturnsEmpty()
        {
            // Arrange
            var runtime = Mock.Of<IRuntime>();
            var token = Token.Create("=", false, null, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 1, TokenType.EQUAL);
            var opSeg = new OperatorSegment(new[] { token });
            var walker = AssistAstWalker.Create(runtime, RqlSourcePosition.Empty);

            // Act
            var suggestions = await walker.VisitOperatorSegment(opSeg);

            // Assert
            suggestions.Should().BeEmpty();
        }

        [Fact]
        public async Task VisitLiteralExpression_ReturnsEmpty()
        {
            // Arrange
            var runtime = Mock.Of<IRuntime>();
            var token = Token.Create("1", false, 1, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 1, TokenType.INT);
            var literal = LiteralExpression.Create(LiteralType.Integer, token, 1);
            var walker = AssistAstWalker.Create(runtime, RqlSourcePosition.Empty);

            // Act
            var suggestions = await walker.VisitLiteralExpression(literal);

            // Assert
            suggestions.Should().BeEmpty();
        }

        [Fact]
        public async Task VisitIdentifierExpression_ReturnsEmpty()
        {
            // Arrange
            var runtime = Mock.Of<IRuntime>();
            var token = Token.Create("abc", false, null, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 3, TokenType.IDENTIFIER);
            var id = new IdentifierExpression(token);
            var walker = AssistAstWalker.Create(runtime, RqlSourcePosition.Empty);

            // Act
            var suggestions = await walker.VisitIdentifierExpression(id);

            // Assert
            suggestions.Should().BeEmpty();
        }

        [Fact]
        public async Task VisitUnaryExpression_DelegatesToRight()
        {
            // Arrange
            var runtime = Mock.Of<IRuntime>();
            var right = new IdentifierExpression(Token.Create("abc", false, null, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 3, TokenType.IDENTIFIER));
            var unary = new UnaryExpression(Token.Create("-", false, null, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 1, TokenType.MINUS), right);
            var walker = AssistAstWalker.Create(runtime, RqlSourcePosition.Empty);

            // Act
            var suggestions = await walker.VisitUnaryExpression(unary);

            // Assert - right is identifier -> VisitIdentifierExpression returns empty
            suggestions.Should().BeEmpty();
        }
    }
}