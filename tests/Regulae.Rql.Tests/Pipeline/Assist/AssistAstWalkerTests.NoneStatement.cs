namespace Regulae.Rql.Tests.Pipeline.Assist
{
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using Regulae.Rql.Ast.Statements;
    using Regulae.Rql.Pipeline.Assist;
    using Regulae.Rql.Runtime;
    using Xunit;

    public partial class AssistAstWalkerTests
    {
        [Fact]
        public async Task VisitNoneStatement_ReturnsEmptySuggestions()
        {
            // Arrange
            var runtime = Mock.Of<IRuntime>();
            var walker = AssistAstWalker.Create(runtime, RqlSourcePosition.Empty);

            var noneStmt = new NoneStatement();

            // Act
            var suggestions = await walker.VisitNoneStatement(noneStmt);

            // Assert
            suggestions.Should().BeEmpty();
        }
    }
}