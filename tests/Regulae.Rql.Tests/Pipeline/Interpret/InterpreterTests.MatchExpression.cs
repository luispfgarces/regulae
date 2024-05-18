namespace Regulae.Rql.Tests.Pipeline.Interpret
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using Regulae;
    using Regulae.Rql;
    using Regulae.Rql.Ast.Expressions;
    using Regulae.Rql.Pipeline.Interpret;
    using Regulae.Rql.Runtime;
    using Regulae.Rql.Runtime.Types;
    using Regulae.Rql.Tests.TestStubs;
    using Xunit;

    public partial class InterpreterTests
    {
        public static IEnumerable<object[]> ValidCasesMatchExpression => new[]
        {
            new object[] { "one", true },
            new object[] { "one", false },
            new object[] { "all", true },
            new object[] { "all", false },
        };

        [Fact]
        public async Task VisitMatchExpression_GivenMatchExpressionFailingRuntimeEvaluation_ThrowsInterpreterException()
        {
            // Arrange
            var conditions = new Dictionary<string, object>(StringComparer.Ordinal)
            {
                { nameof(Conditions.IsVip), false },
            };

            var matchKeyword = Expression.None;
            var cardinalitySegment = CreateMockedSegment(NewRqlString("one"));
            var rulesetSegment = CreateMockedSegment(NewRqlString("Type1"));
            var matchDateSegment = CreateMockedSegment(NewRqlDate(new DateTime(2024, 1, 1)));
            var inputConditionsSegment = CreateMockedSegment(conditions);
            var matchExpression = MatchExpression.Create(matchKeyword, cardinalitySegment, rulesetSegment, matchDateSegment, inputConditionsSegment);

            var runtime = Mock.Of<IRuntime>();
            Mock.Get(runtime)
                .Setup(x => x.GetRulesetsAsync())
                .ReturnsAsync(NewRqlArray(new RqlRuleset(new Ruleset("Type1", DateTime.UtcNow))));
            Mock.Get(runtime)
                .Setup(x => x.MatchRulesAsync(It.IsAny<MatchRulesArgs>()))
                .Throws(new RuntimeException("test"));
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();

            var interpreter = new Interpreter(runtime, reverseRqlBuilder);

            // Act
            var actual = await Assert.ThrowsAsync<InterpreterException>(async () => await interpreter.VisitMatchExpression(matchExpression));

            // Act
            actual.Message.Should().Contain("test");
        }

        [Theory]
        [MemberData(nameof(ValidCasesMatchExpression))]
        public async Task VisitMatchExpression_GivenValidMatchExpressionForOneCardinality_ReturnsOneRule(
            string cardinalityName,
            bool hasConditions)
        {
            // Arrange
            var ruleResult = Rule.Create<Rulesets, Conditions>("Dummy rule")
                .InRuleset(Rulesets.Type1)
                .SetContent("test")
                .Since(DateTime.Now)
                .ApplyWhen(x => x.Value(Conditions.IsVip, Operators.Equal, false))
                .Build();
            var conditions = hasConditions
                ? new Dictionary<string, object>(StringComparer.Ordinal)
                {
                    { nameof(Conditions.IsVip), false },
                }
                : null;

            var expected = new RqlArray(1);
            expected.SetAtIndex(0, new RqlRule(ruleResult.Rule));

            var matchKeyword = Expression.None;
            var cardinalitySegment = CreateMockedSegment(NewRqlString(cardinalityName));
            var rulesetSegment = CreateMockedSegment(new RqlString("Type1"));
            var matchDateSegment = CreateMockedSegment(NewRqlDate(new DateTime(2024, 1, 1)));
            var inputConditionsSegment = CreateMockedSegment(conditions!);
            var matchExpression = MatchExpression.Create(matchKeyword, cardinalitySegment, rulesetSegment, matchDateSegment, inputConditionsSegment);

            var runtime = Mock.Of<IRuntime>();
            Mock.Get(runtime)
                .Setup(x => x.GetRulesetsAsync())
                .ReturnsAsync(NewRqlArray(new RqlRuleset(new Ruleset("Type1", DateTime.UtcNow))));
            Mock.Get(runtime)
                .Setup(x => x.MatchRulesAsync(It.IsAny<MatchRulesArgs>()))
                .Returns(new ValueTask<RqlArray>(expected));
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();

            var interpreter = new Interpreter(runtime, reverseRqlBuilder);

            // Act
            var actual = await interpreter.VisitMatchExpression(matchExpression);

            // Act
            actual.Should().NotBeNull()
                .And.BeEquivalentTo(expected);
        }
    }
}