namespace Regulae.Providers.MongoDb.Tests.Serialization
{
    using FluentAssertions;
    using Regulae.Providers.MongoDb.Serialization;
    using Regulae.Providers.MongoDb.Tests.TestStubs;
    using Xunit;

    public class DynamicToStrongTypeContentSerializationProviderTests
    {
        [Fact]
        public void GetContentSerializer_GivenAnyRulesetValue_ReturnsDynamicToStrongTypeContentSerializer()
        {
            // Arrange
            var dynamicToStrongTypeContentSerializationProvider = new DynamicToStrongTypeContentSerializationProvider();

            // Act
            var contentSerializer = dynamicToStrongTypeContentSerializationProvider.GetContentSerializer(RulesetNames.RulesetSample.ToString());

            // Assert
            contentSerializer.Should().NotBeNull()
                .And.BeOfType<DynamicToStrongTypeContentSerializer>();
        }
    }
}