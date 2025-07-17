namespace Regulae.IntegrationTests.Features.RulesEngine.RulesMatching
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Regulae;
    using Regulae.Generic;
    using Regulae.IntegrationTests.Common.Features;
    using Regulae.IntegrationTests.Common.Features.Stubs;
    using Regulae.IntegrationTests.Features.RulesEngine;
    using Xunit;

    public class OperatorContainsManyToOneTests : RulesEngineTestsBase
    {
        private static readonly RulesetNames testRuleset = RulesetNames.Sample1;
        private readonly Rule<RulesetNames, ConditionNames> expectedMatchRule;
        private readonly Rule<RulesetNames, ConditionNames> otherRule;

        public OperatorContainsManyToOneTests()
            : base(testRuleset)
        {
            this.expectedMatchRule = Rule.Create<RulesetNames, ConditionNames>("Expected rule")
                .InRuleset(testRuleset)
                .SetContent("Just as expected!")
                .Since(UtcDate("2020-01-01Z"))
                .ApplyWhen(ConditionNames.Condition1, Operators.Contains, "Cat")
                .Build()
                .Rule;

            this.otherRule = Rule.Create<RulesetNames, ConditionNames>("Other rule")
                .InRuleset(testRuleset)
                .SetContent("Oops! Not expected to be matched.")
                .Since(UtcDate("2020-01-01Z"))
                .Build()
                .Rule;

            this.AddRules(this.CreateTestRules());
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task RulesEngine_GivenCondition1WithArrayOfStringsContainingCat_MatchesExpectedRule(bool compiled)
        {
            // Arrange
            var emptyConditions = new Dictionary<ConditionNames, object>
            {
                {  ConditionNames.Condition1, new[]{ "Dog", "Fish", "Cat", "Spider", "Mockingbird", } },
            };
            var matchDate = UtcDate("2020-01-02Z");

            // Act
            var actualMatch = await this.MatchOneAsync(matchDate, emptyConditions, compiled);

            // Assert
            actualMatch.Should().BeEquivalentTo(expectedMatchRule);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task RulesEngine_GivenCondition1WithArrayOfStringsNotContainingCat_MatchesOtherRule(bool compiled)
        {
            // Arrange
            var emptyConditions = new Dictionary<ConditionNames, object>
            {
                { ConditionNames.Condition1, new[]{ "Dog", "Fish", "Bat", "Spider", "Mockingbird", } },
            };
            var matchDate = UtcDate("2020-01-02Z");

            // Act
            var actualMatch = await this.MatchOneAsync(matchDate, emptyConditions, compiled);

            // Assert
            actualMatch.Should().BeEquivalentTo(otherRule);
        }

        private IEnumerable<RuleSpecification> CreateTestRules()
        {
            var ruleSpecs = new List<RuleSpecification>
            {
                new RuleSpecification(expectedMatchRule, RuleAddPriorityOption.AtNumber(1)),
                new RuleSpecification(otherRule, RuleAddPriorityOption.AtNumber(2))
            };

            return ruleSpecs;
        }
    }
}