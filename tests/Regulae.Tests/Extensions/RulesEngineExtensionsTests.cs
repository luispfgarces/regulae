namespace Regulae.Tests.Extensions
{
    using FluentAssertions;
    using Moq;
    using Regulae;
    using Regulae.Evaluation;
    using Regulae.Extensions;
    using Regulae.Generic;
    using Regulae.Source;
    using Regulae.Tests.TestStubs;
    using Regulae.Validation;
    using Xunit;

    public class RulesEngineExtensionsTests
    {
        [Fact]
        public void RulesEngineExtensions_MakeGeneric_ReturnsGenericRulesEngine()
        {
            // Arrange
            var rulesEngine = new RulesEngine(
                Mock.Of<IConditionsEvalEngine>(),
                Mock.Of<IRulesSource>(),
                Mock.Of<IValidatorProvider>(),
                RulesEngineOptions.NewWithDefaults(),
                Mock.Of<IRuleConditionsExtractor>());

            // Act
            var genericEngine = rulesEngine.MakeGeneric<RulesetNames, ConditionNames>();

            // Assert
            genericEngine.Should().NotBeNull();
            genericEngine.GetType().Should().Be(typeof(RulesEngine<RulesetNames, ConditionNames>));
        }
    }
}