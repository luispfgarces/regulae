namespace Regulae.Rql.Tests.Pipeline.Assist
{
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using Regulae.Rql.Ast.Expressions;
    using Regulae.Rql.Pipeline.Assist;
    using Regulae.Rql.Runtime;
    using Regulae.Rql.Tokens;
    using Xunit;

    public partial class AssistAstWalkerTests
    {
        [Fact]
        public async Task VisitAssignmentExpression_ReturnsEmptySuggestions()
        {
            // Arrange
            var runtime = Mock.Of<IRuntime>();
            var walker = AssistAstWalker.Create(runtime, RqlSourcePosition.Empty);

            var left = LiteralExpression.Create(LiteralType.Integer, Token.Create("1", false, 1, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 1, TokenType.INT), 1);
            var assignToken = Token.Create("=", false, null, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 1, TokenType.ASSIGN);
            var right = LiteralExpression.Create(LiteralType.Integer, Token.Create("2", false, 2, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 1, TokenType.INT), 2);

            var assignment = new AssignmentExpression(left, assignToken, right);

            // Act
            var suggestions = await walker.VisitAssignmentExpression(assignment);

            // Assert
            suggestions.Should().BeEmpty();
        }
    }
}