namespace Regulae.Tests.Extensions
{
    using FluentAssertions;
    using Regulae;
    using Regulae.Extensions;
    using Regulae.Generic;
    using Regulae.Tests.TestStubs;
    using Xunit;

    public class RulesEngineExtensionsTests
    {
        [Fact]
        public void RulesEngineExtensions_MakeGeneric_ReturnsGenericRulesEngine()
        {
            // Arrange
            var rulesEngineArgs = new RulesEngineArgs();
            var rulesEngine = new RulesEngine(rulesEngineArgs);

            // Act
            var genericEngine = rulesEngine.MakeGeneric<RulesetNames, ConditionNames>();

            // Assert
            genericEngine.Should().NotBeNull();
            genericEngine.GetType().Should().Be(typeof(RulesEngine<RulesetNames, ConditionNames>));
        }
    }
}