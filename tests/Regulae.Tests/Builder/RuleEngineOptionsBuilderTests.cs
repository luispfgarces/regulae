namespace Regulae.Tests.Builder
{
    using System;
    using FluentAssertions;
    using Regulae;
    using Regulae.Builder;
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
    }
}