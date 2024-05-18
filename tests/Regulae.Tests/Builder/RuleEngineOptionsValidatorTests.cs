namespace Regulae.Tests.Builder
{
    using FluentAssertions;
    using Regulae;
    using Regulae.Builder;
    using Xunit;

    public class RuleEngineOptionsValidatorTests
    {
        [Fact]
        public void EnsureValid_GivenOptionsNullReference_ThrowsInvalidRulesEngineOptionsExceptionClaimingNullOptions()
        {
            // Arrange
            RulesEngineOptions rulesEngineOptions = null;

            var actual = Assert.Throws<InvalidRulesEngineOptionsException>(() =>
            {
                // Act
                RulesEngineOptionsValidator.Validate(rulesEngineOptions);
            });

            actual.Message.Should().Be("Specified null rulesEngineOptions.");
        }

        [Theory]
        [InlineData(DataTypes.Boolean, "abc")]
        [InlineData(DataTypes.Boolean, null)]
        [InlineData(DataTypes.Decimal, "abc")]
        [InlineData(DataTypes.Decimal, null)]
        [InlineData(DataTypes.Integer, "abc")]
        [InlineData(DataTypes.Integer, null)]
        [InlineData(DataTypes.String, 0)]
        [InlineData(DataTypes.String, null)]
        [InlineData(DataTypes.ArrayString, new[] { 0 })]
        [InlineData(DataTypes.ArrayString, null)]
        [InlineData(DataTypes.ArrayDecimal, new[] { "!a" })]
        [InlineData(DataTypes.ArrayDecimal, null)]
        [InlineData(DataTypes.ArrayBoolean, new[] { 10 })]
        [InlineData(DataTypes.ArrayBoolean, null)]
        [InlineData(DataTypes.ArrayInteger, new[] { "!a" })]
        [InlineData(DataTypes.ArrayInteger, null)]
        public void EnsureValid_GivenOptionsWithInvalidDefaultForDataType_ThrowsInvalidRulesEngineOptionsExceptionClaimingInvalidDefault(DataTypes dataType, object defaultValue)
        {
            // Arrange
            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();
            rulesEngineOptions.DataTypeDefaults[dataType] = defaultValue;

            var actual = Assert.Throws<InvalidRulesEngineOptionsException>(() =>
            {
                // Act
                RulesEngineOptionsValidator.Validate(rulesEngineOptions);
            });

            actual.Message.Should().Be($"Specified invalid default value for data type {dataType}: {defaultValue ?? "null"}.");
        }
    }
}