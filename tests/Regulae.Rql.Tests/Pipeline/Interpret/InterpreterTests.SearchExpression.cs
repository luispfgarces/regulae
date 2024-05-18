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
        public static IEnumerable<object[]> ValidCasesSearchExpression => new[]
        {
            new object[] { true },
            new object[] { false },
        };

        [Fact]
        public async Task VisitSearchExpression_GivenSearchExpressionFailingRuntimeEvaluation_ThrowsInterpreterException()
        {
            // Arrange
            var conditions = new Dictionary<string, object>(StringComparer.Ordinal)
            {
                { nameof(Conditions.IsVip), false },
            };

            var searchKeyword = Expression.None;
            var rulesKeyword = Expression.None;
            var rulesetExpression = CreateMockedSegment(NewRqlString("Type1"));
            var datesInterval = CreateMockedSegment(new Tuple<RqlDate, RqlDate>(NewRqlDate(new DateTime(2024, 1, 1)), NewRqlDate(new DateTime(2024, 12, 31))));
            var inputConditionsSegment = CreateMockedSegment(conditions);
            var searchExpression = new SearchExpression(searchKeyword, rulesKeyword, rulesetExpression, datesInterval, inputConditionsSegment);

            var runtime = Mock.Of<IRuntime>();
            Mock.Get(runtime)
                .Setup(x => x.GetRulesetsAsync())
                .ReturnsAsync(NewRqlArray(new RqlRuleset(new Ruleset("Type1", DateTime.UtcNow))));
            Mock.Get(runtime)
                .Setup(x => x.SearchRulesAsync(It.IsAny<SearchRulesArgs>()))
                .Throws(new RuntimeException("test"));
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();

            var interpreter = new Interpreter(runtime, reverseRqlBuilder);

            // Act
            var actual = await Assert.ThrowsAsync<InterpreterException>(async () => await interpreter.VisitSearchExpression(searchExpression));

            // Act
            actual.Message.Should().Contain("test");
        }

        [Theory]
        [MemberData(nameof(ValidCasesSearchExpression))]
        public async Task VisitSearchExpression_GivenValidSearchExpressionForOneCardinality_ReturnsRqlArrayWithOneRule(bool hasConditions)
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

            var searchKeyword = Expression.None;
            var rulesKeyword = Expression.None;
            var rulesetExpression = CreateMockedSegment(new RqlString("Type1"));
            var datesInterval = CreateMockedSegment(new Tuple<RqlDate, RqlDate>(NewRqlDate(new DateTime(2024, 1, 1)), NewRqlDate(new DateTime(2024, 12, 31))));
            var inputConditionsSegment = CreateMockedSegment(conditions!);
            var searchExpression = new SearchExpression(searchKeyword, rulesKeyword, rulesetExpression, datesInterval, inputConditionsSegment);

            var runtime = Mock.Of<IRuntime>();
            Mock.Get(runtime)
                .Setup(x => x.GetRulesetsAsync())
                .ReturnsAsync(NewRqlArray(new RqlRuleset(new Ruleset("Type1", DateTime.UtcNow))));
            Mock.Get(runtime)
                .Setup(x => x.SearchRulesAsync(It.IsAny<SearchRulesArgs>()))
                .Returns(new ValueTask<RqlArray>(expected));
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();

            var interpreter = new Interpreter(runtime, reverseRqlBuilder);

            // Act
            var actual = await interpreter.VisitSearchExpression(searchExpression);

            // Act
            actual.Should().NotBeNull()
                .And.BeEquivalentTo(expected);
        }
    }
}