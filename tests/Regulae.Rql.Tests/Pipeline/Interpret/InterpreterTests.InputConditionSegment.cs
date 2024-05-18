namespace Regulae.Rql.Tests.Pipeline.Interpret
{
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using Regulae.Rql.Tests.TestStubs;
    using Regulae.Rql;
    using Regulae.Rql.Ast.Segments;
    using Regulae.Rql.Pipeline.Interpret;
    using Regulae.Rql.Runtime;
    using Regulae.Rql.Tokens;
    using Xunit;

    public partial class InterpreterTests
    {
        [Fact]
        public async Task VisitInputConditionSegment_GivenValidInputConditionSegment_ReturnsCondition()
        {
            // Arrange
            var expectedCondition = nameof(Conditions.IsoCountryCode);
            var expectedConditionValue = "test";
            var leftExpression = CreateMockedExpression(NewRqlString("IsoCountryCode"));
            var operatorToken = NewToken("is", null, TokenType.IS);
            var rightExpression = CreateMockedExpression(NewRqlString(expectedConditionValue));
            var inputConditionSegment = new InputConditionSegment(leftExpression, operatorToken, rightExpression);

            var runtime = Mock.Of<IRuntime>();
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();

            var interpreter = new Interpreter(runtime, reverseRqlBuilder);

            // Act
            var actual = await interpreter.VisitInputConditionSegment(inputConditionSegment);

            // Assert
            actual.Should().NotBeNull().And.BeOfType<ValueTuple<string, object>>();
            var actualCondition = (ValueTuple<string, object>)actual;
            actualCondition.Item1.Should().Be(expectedCondition);
            actualCondition.Item2.Should().Be(expectedConditionValue);
        }
    }
}