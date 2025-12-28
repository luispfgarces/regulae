namespace Regulae.Tests.Cache
{
    using System;
    using FluentAssertions;
    using Regulae.Cache;
    using Xunit;

    public class InMemoryCacheTests
    {
        private static InMemoryCache CreateCache() => new InMemoryCache(Guid.NewGuid().ToString());

        [Fact]
        public void Set_And_TryGet_ShouldStoreAndRetrieveValue()
        {
            // Arrange
            var cache = CreateCache();
            var key = "test-key";
            var value = "test-value";

            // Act
            cache.Set(key, value);
            var result = cache.TryGet(key, out var retrieved);

            // Assert
            result.Should().BeTrue();
            retrieved.Should().Be(value);
        }

        [Fact]
        public void TryGet_WithNonexistentKey_ShouldReturnFalse()
        {
            // Arrange
            var cache = CreateCache();

            // Act
            var result = cache.TryGet("nonexistent", out var value);

            // Assert
            result.Should().BeFalse();
            value.Should().BeNull();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Set_WithNullOrEmptyKey_ShouldThrowArgumentException(string inputKey)
        {
            // Arrange
            var cache = CreateCache();

            // Act
            Action act = () => cache.Set(inputKey, "value");

            // Assert
            act.Should().Throw<ArgumentException>().WithParameterName("key");
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void TryGet_WithNullOrEmptyKey_ShouldThrowArgumentException(string inputKey)
        {
            // Arrange
            var cache = CreateCache();

            // Act
            Action act = () => cache.TryGet(inputKey, out _);

            // Assert
            act.Should().Throw<ArgumentException>().WithParameterName("key");
        }

        [Fact]
        public void Evict_RemovesKey()
        {
            // Arrange
            var cache = CreateCache();
            var key = "evict-key";
            cache.Set(key, 123);

            // Act
            cache.Evict(key);

            // Assert
            cache.TryGet(key, out var value).Should().BeFalse();
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Evict_WithNullOrEmptyKey_ShouldThrowArgumentException(string inputKey)
        {
            // Arrange
            var cache = CreateCache();

            // Act
            Action act = () => cache.Evict(inputKey);

            // Assert
            act.Should().Throw<ArgumentException>().WithParameterName("key");
        }

        [Fact]
        public void EvictMany_RemovesAllMatchingPrefix()
        {
            // Arrange
            var cache = CreateCache();
            cache.Set("prefix-1", 1);
            cache.Set("prefix-2", 2);
            cache.Set("other-1", 3);

            // Act
            cache.EvictMany("prefix-");

            // Assert
            cache.TryGet("prefix-1", out _).Should().BeFalse();
            cache.TryGet("prefix-2", out _).Should().BeFalse();
            cache.TryGet("other-1", out var value).Should().BeTrue();
            value.Should().Be(3);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void EvictMany_WithNullOrEmptyPrefix_ShouldThrowArgumentException(string inputKeyPrefix)
        {
            // Arrange
            var cache = CreateCache();

            // Act
            Action act = () => cache.EvictMany(inputKeyPrefix);

            // Assert
            act.Should().Throw<ArgumentException>().WithParameterName("keyPrefix");
        }
    }
}