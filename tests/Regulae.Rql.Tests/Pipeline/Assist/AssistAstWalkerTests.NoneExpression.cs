namespace Regulae.Rql.Tests.Pipeline.Assist
{
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using Regulae.Rql.Ast.Expressions;
    using Regulae.Rql.Pipeline.Assist;
    using Regulae.Rql.Runtime;
    using Xunit;

    public partial class AssistAstWalkerTests
    {
        [Fact]
        public async Task VisitNoneExpression_ReturnsEmptySuggestions()
        {
            // Arrange
            var runtime = Mock.Of<IRuntime>();
            var walker = AssistAstWalker.Create(runtime, RqlSourcePosition.Empty);

            var none = new NoneExpression();

            // Act
            var suggestions = await walker.VisitNoneExpression(none);

            // Assert
            suggestions.Should().BeEmpty();
        }
    }
}