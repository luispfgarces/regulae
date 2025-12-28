namespace Regulae.Tests.Builder
{
    using System;
    using FluentAssertions;
    using Moq;
    using Regulae;
    using Regulae.Builder;
    using Regulae.Cache;
    using Xunit;

    public class RuleEngineOptionsBuilderTests
    {
        [Theory]
        [InlineData(DataTypes.Boolean, "abc")]
        [InlineData(DataTypes.Boolean, null)]
        [InlineData(DataTypes.Decimal, "abc")]
        [InlineData(DataTypes.Decimal, null)]
        [InlineData(DataTypes.Integer, "abc")]
        [InlineData(DataTypes.Integer, null)]
        [InlineData(DataTypes.String, 0)]
        [InlineData(DataTypes.String, null)]
        public void SetDataTypeDefault_GivenOptionsWithInvalidDefaultForDataType_ThrowsInvalidRulesEngineOptionsExceptionClaimingInvalidDefault(DataTypes dataType, object defaultValue)
        {
            // Arrange
            var rulesEngineOptionsBuilder = new RulesEngineOptionsBuilder();

            var actual = Assert.Throws<ArgumentException>(() =>
            {
                // Act
                rulesEngineOptionsBuilder.SetDataTypeDefault(dataType, defaultValue);
            });

            actual.Message.Should().Be($"Specified invalid default value for data type {dataType}: {defaultValue ?? "null"}. (Parameter 'defaultValue')");
        }

        [Fact]
        public void SetDataTypeDefault_WithValidNativeValues_SetsDefault()
        {
            var builder = new RulesEngineOptionsBuilder();
            builder.SetDataTypeDefault(DataTypes.Boolean, true)
                   .SetDataTypeDefault(DataTypes.Decimal, 1.23m)
                   .SetDataTypeDefault(DataTypes.Integer, 42)
                   .SetDataTypeDefault(DataTypes.String, "test");

            var options = builder.Build();

            options.DataTypeDefaults[DataTypes.Boolean].Should().Be(true);
            options.DataTypeDefaults[DataTypes.Decimal].Should().Be(1.23m);
            options.DataTypeDefaults[DataTypes.Integer].Should().Be(42);
            options.DataTypeDefaults[DataTypes.String].Should().Be("test");
        }

        [Fact]
        public void SetDataTypeDefault_WithValidStringRepresentationValues_SetsDefault()
        {
            var builder = new RulesEngineOptionsBuilder();
            builder.SetDataTypeDefault(DataTypes.Boolean, "true")
                   .SetDataTypeDefault(DataTypes.Decimal, "1.23")
                   .SetDataTypeDefault(DataTypes.Integer, "42")
                   .SetDataTypeDefault(DataTypes.String, "test");

            var options = builder.Build();

            options.DataTypeDefaults[DataTypes.Boolean].Should().Be(true);
            options.DataTypeDefaults[DataTypes.Decimal].Should().Be(1.23m);
            options.DataTypeDefaults[DataTypes.Integer].Should().Be(42);
            options.DataTypeDefaults[DataTypes.String].Should().Be("test");
        }

        [Fact]
        public void SetAutoCreateRulesets_SetsOption()
        {
            var builder = new RulesEngineOptionsBuilder();
            builder.SetAutoCreateRulesets(true);

            var options = builder.Build();

            options.AutoCreateRulesets.Should().BeTrue();
        }

        [Fact]
        public void UseCache_SetsCache()
        {
            var cache = Mock.Of<ICache>();
            var builder = new RulesEngineOptionsBuilder();
            builder.UseCache(cache);

            var options = builder.Build();

            options.Cache.Should().BeSameAs(cache);
        }

        [Fact]
        public void UseEvaluationStrategy_WithValidValue_SetsStrategy()
        {
            var builder = new RulesEngineOptionsBuilder();
            builder.UseEvaluationStrategy(EvaluationStrategies.Interpreted);

            var options = builder.Build();

            options.EvaluationStrategy.Should().Be(EvaluationStrategies.Interpreted);
        }

        [Fact]
        public void UseEvaluationStrategy_WithInvalidValue_Throws()
        {
            var builder = new RulesEngineOptionsBuilder();
            var invalidValue = (EvaluationStrategies)999;

            Action act = () => builder.UseEvaluationStrategy(invalidValue);

            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithMessage("*Invalid EvaluationStrategies value specified.*");
        }

        [Fact]
        public void SetMissingConditionBehavior_WithValidValue_SetsBehavior()
        {
            var builder = new RulesEngineOptionsBuilder();
            builder.SetMissingConditionBehavior(MissingConditionBehaviors.UseDataTypeDefault);

            var options = builder.Build();

            options.MissingConditionBehavior.Should().Be(MissingConditionBehaviors.UseDataTypeDefault);
        }

        [Fact]
        public void SetMissingConditionBehavior_WithInvalidValue_Throws()
        {
            var builder = new RulesEngineOptionsBuilder();
            var invalidValue = (MissingConditionBehaviors)999;

            Action act = () => builder.SetMissingConditionBehavior(invalidValue);

            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithMessage("*Invalid MissingConditionBehaviors value specified.*");
        }

        [Fact]
        public void UsePriorityCriteria_WithValidValue_SetsCriteria()
        {
            var builder = new RulesEngineOptionsBuilder();
            builder.UsePriorityCriteria(PriorityCriterias.SmallestNumber);

            var options = builder.Build();

            options.PriorityCriteria.Should().Be(PriorityCriterias.SmallestNumber);
        }

        [Fact]
        public void UsePriorityCriteria_WithInvalidValue_Throws()
        {
            var builder = new RulesEngineOptionsBuilder();
            var invalidValue = (PriorityCriterias)100;

            Action act = () => builder.UsePriorityCriteria(invalidValue);

            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithMessage("*Invalid PriorityCriterias value specified.*");
        }

        [Fact]
        public void SetDataTypeDefault_WithUnsupportedDataType_ThrowsArgumentOutOfRangeException()
        {
            var builder = new RulesEngineOptionsBuilder();
            var unsupportedDataType = (DataTypes)999;

            Action act = () => builder.SetDataTypeDefault(unsupportedDataType, "any");

            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithMessage($"Invalid DataTypes value specified: {unsupportedDataType}.*")
                .And.ParamName.Should().Be("dataType");
        }
    }
}