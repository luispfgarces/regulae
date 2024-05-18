namespace Regulae.Tests.Evaluation.Compiled
{
    using System.Collections.Generic;
    using FluentAssertions;
    using Regulae.Evaluation.Compiled;
    using Regulae.Tests.TestStubs;
    using Xunit;

    public class ConditionsValueLookupExtensionTests
    {
        [Fact]
        public void GetValueOrDefault_GivenConditionsDictionaryAndCondition_ReturnsNull()
        {
            // Arrange
            const string expected = "EUR";
            var conditions = new Dictionary<string, object>
            {
                { ConditionNames.IsoCurrency.ToString(), expected }
            };
            var condition = ConditionNames.IsoCurrency.ToString();

            // Act
            var result = ConditionsValueLookupExtension.GetValueOrDefault(conditions, condition);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void GetValueOrDefault_GivenEmptyConditionsDictionaryAndCondition_ReturnsNull()
        {
            // Arrange
            var conditions = new Dictionary<string, object>();
            var condition = ConditionNames.IsoCurrency.ToString();

            // Act
            var result = ConditionsValueLookupExtension.GetValueOrDefault(conditions, condition);

            // Assert
            result.Should().BeNull();
        }
    }
}