namespace Regulae.Rql.Tests.Runtime.Types
{
    using FluentAssertions;
    using Regulae.Rql.Runtime.Types;
    using Xunit;

    public class RqlIntegerTests
    {
        [Fact]
        public void Ctor_GivenInteger_SetsValue()
        {
            // Act
            var i = new RqlInteger(10);

            // Assert
            i.Value.Should().Be(10);
            i.Type.Should().Be(RqlTypes.Integer);
            i.RuntimeValue.Should().Be(10);
            i.RuntimeType.Should().Be(typeof(int));
        }

        [Fact]
        public void Equals_SameValue_AreEqualAndHashCodesEqual()
        {
            var a = (RqlInteger)42;
            var b = (RqlInteger)42;

            a.Equals(b).Should().BeTrue();
            a.GetHashCode().Should().Be(b.GetHashCode());
        }

        [Fact]
        public void Equals_DifferentValue_AreNotEqual()
        {
            var a = (RqlInteger)1;
            var b = (RqlInteger)2;

            a.Equals(b).Should().BeFalse();
        }

        [Fact]
        public void ToString_GivenInteger_ReturnsStringRepresentation()
        {
            // Arrange
            var i = new RqlInteger(42);

            // Act
            var result = i.ToString();

            // Assert
            result.Should().Be("<integer> 42");
        }
    }
}
