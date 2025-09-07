namespace Regulae.Tests.Builder
{
    using FluentAssertions;
    using Moq;
    using Regulae;
    using Regulae.Builder;
    using Regulae.Cache;
    using Xunit;

    public class ConfiguredRulesEngineBuilderTests
    {
        [Fact]
        public void Build_WhenCacheIsSpecified_ReturnsRulesEngineWithRulesSourceCaching()
        {
            // Arrange
            var rulesDataSource = Mock.Of<IRulesDataSource>();
            var cache = Mock.Of<ICache>();
            var configuredRulesEngineBuilder = new ConfiguredRulesEngineBuilder(rulesDataSource);

            configuredRulesEngineBuilder.Configure(opt =>
            {
                opt.UseCache(cache);
            });

            // Act
            var actual = configuredRulesEngineBuilder.Build();

            // Assert
            actual.Should().NotBeNull();
        }

        [Fact]
        public void Build_WhenCompiledEvaluationStrategy_ReturnsRulesEngineWithCompiledEvaluation()
        {
            // Arrange
            var rulesDataSource = Mock.Of<IRulesDataSource>();
            var configuredRulesEngineBuilder = new ConfiguredRulesEngineBuilder(rulesDataSource);

            configuredRulesEngineBuilder.Configure(opt =>
            {
                opt.UseEvaluationStrategy(EvaluationStrategies.Compiled);
            });

            // Act
            var actual = configuredRulesEngineBuilder.Build();

            // Assert
            actual.Should().NotBeNull();
            actual.Options.EvaluationStrategy.Should().Be(EvaluationStrategies.Compiled);
        }

        [Fact]
        public void Build_WhenInterpretedEvaluationStrategy_ReturnsRulesEngineWithInterpretedEvaluation()
        {
            // Arrange
            var rulesDataSource = Mock.Of<IRulesDataSource>();
            var configuredRulesEngineBuilder = new ConfiguredRulesEngineBuilder(rulesDataSource);

            configuredRulesEngineBuilder.Configure(opt =>
            {
                opt.UseEvaluationStrategy(EvaluationStrategies.Interpreted);
            });

            // Act
            var actual = configuredRulesEngineBuilder.Build();

            // Assert
            actual.Should().NotBeNull();
            actual.Options.EvaluationStrategy.Should().Be(EvaluationStrategies.Interpreted);
        }
    }
}