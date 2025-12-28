namespace Regulae.Rql.Tests.Runtime.Types
{
    using FluentAssertions;
    using Regulae.Rql.Runtime.Types;
    using Xunit;

    public class RqlAnyTests
    {
        [Fact]
        public void Ctor_Default_YieldsNothing()
        {
            // Act
            var any = new RqlAny();

            // Assert
            any.UnderlyingType.Should().Be(RqlTypes.Nothing);
        }

        [Fact]
        public void Ctor_GivenRuntimeValue_WrapsItCorrectly()
        {
            // Act
            var any1 = new RqlAny(new RqlInteger(1));
            var any2 = new RqlAny(new RqlString("hello"));

            // Assert
            any1.UnderlyingType.Should().Be(RqlTypes.Integer);
            any2.UnderlyingType.Should().Be(RqlTypes.String);
            any1.Value.Should().Be(1);
            any1.RuntimeValue.Should().Be(1);
            any1.RuntimeType.Should().Be(typeof(int));
            any2.Value.Should().Be("hello");
            any2.RuntimeValue.Should().Be("hello");
            any2.RuntimeType.Should().Be(typeof(string));
        }

        [Fact]
        public void Ctor_GivenRqlDecimalWrappedAsRqlAny_UnwrapsAndWrapsItCorrectly()
        {
            // Arrange
            var any1 = new RqlAny(new RqlDecimal(1.5m));

            // Act
            var any2 = new RqlAny(any1);

            // Assert
            any2.UnderlyingType.Should().Be(RqlTypes.Decimal);
            any1.RuntimeValue.Should().Be(1.5m);
            any1.RuntimeType.Should().Be(typeof(decimal));
        }

        [Fact]
        public void Equals_SameUnderlying_AreEqual()
        {
            var underlying = (RqlInteger)7;
            var a = (RqlAny)underlying;
            var b = (RqlAny)underlying;

            a.Equals(b).Should().BeTrue();
            a.GetHashCode().Should().Be(b.GetHashCode());
        }

        [Fact]
        public void Equals_DifferentUnderlying_AreNotEqual()
        {
            var a = (RqlAny)((RqlInteger)1);
            var b = (RqlAny)((RqlInteger)2);

            a.Equals(b).Should().BeFalse();
        }

        [Fact]
        public void Equals_DifferentTypes_AreNotEqual()
        {
            var a = (RqlAny)((RqlInteger)1);
            var b = (RqlDecimal)2;

            a.Equals((object)b).Should().BeFalse();
        }

        [Fact]
        public void Equals_WrappedAny_UnwrapsAndCompares()
        {
            var inner = (RqlInteger)5;
            var a = (RqlAny)inner;
            var wrapped = new RqlAny(a); // internal ctor used via same assembly access
            var direct = (RqlAny)inner;

            wrapped.Equals(direct).Should().BeTrue();
            wrapped.Equals((object)wrapped).Should().BeTrue();
        }

        [Fact]
        public void ToString_GivenWrappedValues_ReturnsPrettyString()
        {
            // Arrange
            var any1 = new RqlAny(new RqlInteger(1));

            // Act
            var str1 = any1.ToString();

            // Assert
            str1.Should().Be("<any> (<integer> 1)");
        }

        [Fact]
        public void Unwrap_GivenWrappedRqlInteger_UnwrapsCorrectly()
        {
            // Arrange
            RqlAny any1 = new RqlInteger(3);

            // Act
            var unwrapped = any1.Unwrap<RqlInteger>();

            // Assert
            unwrapped.Value.Should().Be(3);
        }
    }
}
