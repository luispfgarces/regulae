namespace Regulae.Rql.Tests.Pipeline.Interpret
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using Regulae.Rql.Ast.Expressions;
    using Regulae.Rql.Ast.Segments;
    using Regulae.Rql.Pipeline.Interpret;
    using Regulae.Rql.Runtime;
    using Regulae.Rql.Runtime.Types;
    using Regulae.Rql.Tests.TestStubs;
    using Xunit;

    public partial class InterpreterTests
    {
        [Fact]
        public async Task VisitRulesetSegment_GivenInvalidRulesetSegmentWithInvalidRuleset_ThrowsInterpreterException()
        {
            // Arrange
            var conditions = new Dictionary<string, object>(StringComparer.Ordinal)
            {
                { nameof(Conditions.IsVip), false },
            };

            var forKeyword = Expression.None;
            var rulesetExpression = CreateMockedExpression(NewRqlDecimal(1m));
            var rulesetSegment = RulesetSegment.Create(forKeyword, rulesetExpression);

            var runtime = Mock.Of<IRuntime>();
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();

            var interpreter = new Interpreter(runtime, reverseRqlBuilder);

            // Act
            var actual = await Assert.ThrowsAsync<InterpreterException>(async () => await interpreter.VisitRulesetSegment(rulesetSegment));

            // Act
            actual.Message.Should().Contain("Expected a ruleset value of type 'string' but found 'decimal' instead");
        }

        [Fact]
        public async Task VisitRulesetSegment_GivenInvalidRulesetSegmentWithUnknownRuleset_ThrowsInterpreterException()
        {
            // Arrange
            var conditions = new Dictionary<string, object>(StringComparer.Ordinal)
            {
                { nameof(Conditions.IsVip), false },
            };

            var forKeyword = Expression.None;
            var rulesetExpression = CreateMockedExpression(NewRqlString("dummy"));
            var rulesetSegment = RulesetSegment.Create(forKeyword, rulesetExpression);

            var runtime = Mock.Of<IRuntime>();
            Mock.Get(runtime)
                .Setup(x => x.GetRulesetsAsync())
                .ReturnsAsync(NewRqlArray(new RqlRuleset(new Ruleset("other", DateTime.UtcNow))));
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();

            var interpreter = new Interpreter(runtime, reverseRqlBuilder);

            // Act
            var actual = await Assert.ThrowsAsync<InterpreterException>(async () => await interpreter.VisitRulesetSegment(rulesetSegment));

            // Act
            actual.Message.Should().Contain("The ruleset 'dummy' was not found");
        }

        [Fact]
        public async Task VisitRulesetSegment_GivenValidRulesetSegment_ReturnsRuleset()
        {
            // Arrange
            var expected = new RqlString("Type1");
            var expectedRuleset = new RqlRuleset(new Ruleset(expected, DateTime.UtcNow));

            var forKeyword = Expression.None;
            var rulesetExpression = CreateMockedExpression(expected);
            var rulesetSegment = RulesetSegment.Create(forKeyword, rulesetExpression);

            var runtime = Mock.Of<IRuntime>();
            Mock.Get(runtime)
                .Setup(x => x.GetRulesetsAsync())
                .ReturnsAsync(NewRqlArray(expectedRuleset));
            var reverseRqlBuilder = Mock.Of<IReverseRqlBuilder>();

            var interpreter = new Interpreter(runtime, reverseRqlBuilder);

            // Act
            var actual = await interpreter.VisitRulesetSegment(rulesetSegment);

            // Act
            actual.Should().NotBeNull()
                .And.BeEquivalentTo(expected);
        }
    }
}