namespace Regulae.Rql.Tests.Pipeline.Interpret
{
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using Regulae.Rql;
    using Regulae.Rql.Ast.Segments;
    using Regulae.Rql.Pipeline.Interpret;
    using Regulae.Rql.Runtime;
    using Xunit;

    public partial class InterpreterTests
    {
        [Fact]
        public async Task VisitCardinalitySegment_GivenValidCardinalitySegment_ReturnsCardinalityValue()
        {
            // Arrange
            var expected = NewRqlString("ONE");
            var cardinalityExpression = CreateMockedExpression(expected);
            var ruleExpression = CreateMockedExpression(NewRqlString("rule"));
            var cardinalitySegment = CardinalitySegment.Create(cardinalityExpression, ruleExpression);

            var runtime = Mock.Of<IRuntime>();
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();

            var interpreter = new Interpreter(runtime, reverseRqlBuilder);

            // Act
            var actual = await interpreter.VisitCardinalitySegment(cardinalitySegment);

            // Assert
            actual.Should().NotBeNull().And.BeEquivalentTo(expected);
        }
    }
}