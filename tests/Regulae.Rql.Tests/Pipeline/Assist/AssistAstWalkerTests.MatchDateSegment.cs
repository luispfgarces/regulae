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
    using Xunit;

    public partial class AssistAstWalkerTests
    {
        [Fact]
        public async Task VisitMatchDateSegment_WhenOnMissing_ReturnsON()
        {
            // Arrange
            var runtime = Mock.Of<IRuntime>();
            var walker = AssistAstWalker.Create(runtime, RqlSourcePosition.Empty);

            var segment = MatchDateSegment.Create(Expression.None, Expression.None);

            // Act
            var suggestions = await walker.VisitMatchDateSegment(segment);

            // Assert
            suggestions.Select(s => s.Lexeme).Should().Contain("ON");
        }
    }
}
