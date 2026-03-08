namespace Regulae.Tests.Extensions
{
    using System;
    using FluentAssertions;
    using Regulae;
    using Regulae.Builder;
    using Xunit;

    public class RulesEngineConfigurationExtensionsTests
    {
        [Fact]
        public void EnableAutoCreateRulesets_SetsOptionTrue()
        {
            // Arrange
            var builder = new RulesEngineOptionsBuilder();

            // Act
            builder.EnableAutoCreateRulesets();

            // Assert
            var options = builder.Build();
            options.AutoCreateRulesets.Should().BeTrue();
        }

        [Fact]
        public void DisableAutoCreateRulesets_SetsOptionFalse()
        {
            // Arrange
            var builder = new RulesEngineOptionsBuilder();

            // Act
            builder.DisableAutoCreateRulesets();

            // Assert
            var options = builder.Build();
            options.AutoCreateRulesets.Should().BeFalse();
        }

        [Fact]
        public void UseCompiledEvaluationStrategy_SetsCompiled()
        {
            // Arrange
            var builder = new RulesEngineOptionsBuilder();

            // Act
            builder.UseCompiledEvaluationStrategy();

            // Assert
            var options = builder.Build();
            options.EvaluationStrategy.Should().Be(EvaluationStrategies.Compiled);
        }

        [Fact]
        public void UseInterpretedEvaluationStrategy_SetsInterpreted()
        {
            // Arrange
            var builder = new RulesEngineOptionsBuilder();

            // Act
            builder.UseInterpretedEvaluationStrategy();

            // Assert
            var options = builder.Build();
            options.EvaluationStrategy.Should().Be(EvaluationStrategies.Interpreted);
        }

        [Fact]
        public void UseLargestNumberPriorityCriteria_SetsLargest()
        {
            var builder = new RulesEngineOptionsBuilder();

            builder.UseLargestNumberPriorityCriteria();

            var options = builder.Build();
            options.PriorityCriteria.Should().Be(PriorityCriterias.PrioritizeLargestNumber);
        }

        [Fact]
        public void UseSmallestNumberPriorityCriteria_SetsSmallest()
        {
            // Arrange
            var builder = new RulesEngineOptionsBuilder();

            // Act
            builder.UseSmallestNumberPriorityCriteria();

            // Assert
            var options = builder.Build();
            options.PriorityCriteria.Should().Be(PriorityCriterias.PrioritizeSmallestNumber);
        }

        [Fact]
        public void UseInMemoryCache_DefaultOverload_SetsCache()
        {
            // Arrange
            var builder = new RulesEngineOptionsBuilder();

            // Act
            builder.UseInMemoryCache();

            // Assert
            var options = builder.Build();
            options.Cache.Should().NotBeNull();
        }

        [Fact]
        public void UseInMemoryCache_WithName_SetsCache()
        {
            // Arrange
            var builder = new RulesEngineOptionsBuilder();

            // Act
            builder.UseInMemoryCache("mycache");

            // Assert
            var options = builder.Build();
            options.Cache.Should().NotBeNull();
        }

        [Fact]
        public void ExtensionMethods_ThrowOnNullArgument()
        {
            // Arrange
            IRulesEngineConfiguration nullAsInterface = null;

            // Act
            Action a1 = () => RulesEngineConfigurationExtensions.EnableAutoCreateRulesets(nullAsInterface!);
            Action a2 = () => RulesEngineConfigurationExtensions.DisableAutoCreateRulesets(nullAsInterface!);
            Action a3 = () => RulesEngineConfigurationExtensions.UseCompiledEvaluationStrategy(nullAsInterface!);
            Action a4 = () => RulesEngineConfigurationExtensions.UseInterpretedEvaluationStrategy(nullAsInterface!);
            Action a5 = () => RulesEngineConfigurationExtensions.UseInMemoryCache(nullAsInterface!);
            Action a6 = () => RulesEngineConfigurationExtensions.UseInMemoryCache(nullAsInterface!, "name");
            Action a7 = () => RulesEngineConfigurationExtensions.UseLargestNumberPriorityCriteria(nullAsInterface!);
            Action a8 = () => RulesEngineConfigurationExtensions.UseSmallestNumberPriorityCriteria(nullAsInterface!);

            // Assert
            a1.Should().Throw<ArgumentNullException>();
            a2.Should().Throw<ArgumentNullException>();
            a3.Should().Throw<ArgumentNullException>();
            a4.Should().Throw<ArgumentNullException>();
            a5.Should().Throw<ArgumentNullException>();
            a6.Should().Throw<ArgumentNullException>();
            a7.Should().Throw<ArgumentNullException>();
            a8.Should().Throw<ArgumentNullException>();
        }
    }
}
