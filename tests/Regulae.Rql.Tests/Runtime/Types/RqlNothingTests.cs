namespace Regulae.Rql.Tests.Runtime.Types
{
    using FluentAssertions;
    using Regulae.Rql.Runtime.Types;
    using Xunit;

    public class RqlNothingTests
    {
        [Fact]
        public void Ctor_Default_Values()
        {
            // Act
            var n = new RqlNothing();

            // Assert
            n.Type.Should().Be(RqlTypes.Nothing);
            n.RuntimeType.Should().Be(typeof(object));
            n.RuntimeValue.Should().BeNull();
        }

        [Fact]
        public void Equals_AnyInstance_EqualsAlwaysAndHashCodesEqual()
        {
            var a = new RqlNothing();
            var b = new RqlNothing();

            a.Equals(b).Should().BeTrue();
            a.Equals((object)b).Should().BeTrue();
            a.GetHashCode().Should().Be(b.GetHashCode());
        }

        [Fact]
        public void ToString_Default_ReturnsExpectedFormat()
        {
            // Arrange
            var n = new RqlNothing();

            // Act
            var result = n.ToString();

            // Assert
            result.Should().Be("<nothing>");
        }
    }
}
