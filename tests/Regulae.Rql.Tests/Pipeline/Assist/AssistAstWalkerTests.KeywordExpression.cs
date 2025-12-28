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
        public async Task VisitKeywordExpression_ReturnsEmptySuggestions()
        {
            // Arrange
            var runtime = Mock.Of<IRuntime>();
            var walker = AssistAstWalker.Create(runtime, RqlSourcePosition.Empty);

            var token = Token.Create("CREATE", false, null, RqlSourcePosition.Empty, RqlSourcePosition.Empty, 6, TokenType.CREATE);
            var keyword = KeywordExpression.Create(token);

            // Act
            var suggestions = await walker.VisitKeywordExpression(keyword);

            // Assert
            suggestions.Should().BeEmpty();
        }
    }
}