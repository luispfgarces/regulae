namespace Regulae.Tests
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Regulae;
    using Regulae.ConditionNodes;
    using Regulae.Tests.TestStubs;
    using Xunit;

    public class RuleConditionsExtractorTests
    {
        [Fact]
        public void GetConditions_ReturnsCorrectExtraction()
        {
            // Arrange

            var dateBegin = new DateTime(2018, 01, 01);
            var dateEnd = new DateTime(2019, 01, 01);

            var rule1 = new Rule("Rule 1", "Test ruleset", dateBegin, dateEnd, new ObjectContentContainer(new object()))
            {
                Priority = 3,
                RootCondition = new ValueConditionNode(ConditionNames.IsoCountryCode.ToString(), Operators.Equal, "USA"),
            };

            var rule2 = new Rule("Rule 2", "Test ruleset", new DateTime(2020, 01, 01), new DateTime(2021, 01, 01), new ObjectContentContainer(new object()))
            {
                Priority = 200,
                RootCondition = new ValueConditionNode(ConditionNames.IsoCountryCode.ToString(), Operators.Equal, "USA"),
            };

            var rule3 = new Rule("Rule 3", "Test ruleset", dateBegin, dateEnd, new ObjectContentContainer(new object()))
            {
                Priority = 1,
                RootCondition = new ValueConditionNode(ConditionNames.IsoCurrency.ToString(), Operators.Equal, "EUR"),
            };

            var rule4 = new Rule("Rule 4", "Test ruleset", dateBegin, dateEnd, new ObjectContentContainer(new object()))
            {
                Priority = 1,
                RootCondition = new ComposedConditionNode(
                    LogicalOperators.And,
                    [
                        new ValueConditionNode(ConditionNames.IsVip.ToString(), Operators.Equal, "true"),
                        new ValueConditionNode(ConditionNames.PluviosityRate.ToString(), Operators.Equal, "15"),
                        new ValueConditionNode(ConditionNames.IsoCurrency.ToString(), Operators.Equal, "JPY")
                    ]),
            };

            var matchRules = new[]
            {
                rule1,
                rule2,
                rule3,
                rule4
            };

            var expectedConditionList = new List<string>
            {
                ConditionNames.IsoCurrency.ToString(),
                ConditionNames.IsoCountryCode.ToString(),
                ConditionNames.IsVip.ToString(),
                ConditionNames.PluviosityRate.ToString(),
            };

            var ruleConditionsExtractor = new RuleConditionsExtractor();

            // Act
            var actual = ruleConditionsExtractor.GetConditions(matchRules);

            // Assert
            actual.Should().BeEquivalentTo(expectedConditionList);
        }

        [Fact]
        public void GetConditions_WithEmptyMatchRules_ReturnsEmptyListConditions()
        {
            // Arrange

            var matchRules = new List<Rule>();

            var expectedConditionList = new List<string>();

            var ruleConditionsExtractor = new RuleConditionsExtractor();

            // Act
            var actual = ruleConditionsExtractor.GetConditions(matchRules);

            // Assert
            actual.Should().BeEquivalentTo(expectedConditionList);
        }

        [Fact]
        public void GetConditions_WithNullRootCondition_ReturnsEmptyListConditions()
        {
            // Arrange

            var dateBegin = new DateTime(2018, 01, 01);
            var dateEnd = new DateTime(2019, 01, 01);

            var matchRules = new List<Rule>
            {
                new("Rule 1", "Test ruleset", dateBegin, dateEnd, new ObjectContentContainer(new object()))
                {
                    Priority = 1,
                    RootCondition = null,
                }
            };

            var expectedConditionList = new List<string>();

            var ruleConditionsExtractor = new RuleConditionsExtractor();

            // Act
            var actual = ruleConditionsExtractor.GetConditions(matchRules);

            // Assert
            actual.Should().BeEquivalentTo(expectedConditionList);
        }
    }
}