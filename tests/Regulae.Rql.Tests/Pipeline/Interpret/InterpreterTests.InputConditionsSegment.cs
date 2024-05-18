namespace Regulae.Rql.Tests.Pipeline.Interpret
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using Regulae.Rql;
    using Regulae.Rql.Ast.Expressions;
    using Regulae.Rql.Ast.Segments;
    using Regulae.Rql.Pipeline.Interpret;
    using Regulae.Rql.Runtime;
    using Regulae.Rql.Tests.TestStubs;
    using Regulae.Rql.Tokens;
    using Xunit;

    public partial class InterpreterTests
    {
        [Fact]
        public async Task VisitInputConditionsSegment_GivenValidInputConditionsSegment_ReturnsConditionsCollection()
        {
            // Arrange
            var expectedCondition1 = new ValueTuple<string, object>(nameof(Conditions.IsoCountryCode), "PT");
            var expectedCondition2 = new ValueTuple<string, object>(nameof(Conditions.IsVip), true);
            var whenKeyword = Expression.None;
            var beginToken = Token.None;
            var inputConditionSegment1 = CreateMockedSegment(expectedCondition1);
            var inputConditionSegment2 = CreateMockedSegment(expectedCondition2);
            var endToken = Token.None;
            var inputConditionsSegment = new InputConditionsSegment(whenKeyword, beginToken, new[] { inputConditionSegment1, inputConditionSegment2 }, endToken);

            var runtime = Mock.Of<IRuntime>();
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();

            var interpreter = new Interpreter(runtime, reverseRqlBuilder);

            // Act
            var actual = await interpreter.VisitInputConditionsSegment(inputConditionsSegment);

            // Assert
            actual.Should().NotBeNull().And.BeAssignableTo<IDictionary<string, object>>();
            var actualConditions = actual as IDictionary<string, object>;
            actualConditions.Should()
                .Contain(expectedCondition1.Item1, expectedCondition1.Item2)
                .And
                .Contain(expectedCondition2.Item1, expectedCondition2.Item2);
        }
    }
}