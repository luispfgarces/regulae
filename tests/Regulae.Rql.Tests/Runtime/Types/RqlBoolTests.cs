namespace Regulae.Rql.Tests.Runtime.Types
{
    using FluentAssertions;
    using Regulae.Rql.Runtime.Types;
    using Xunit;

    public class RqlBoolTests
    {
        [Fact]
        public void Ctor_GivenTrue_SetsValue()
        {
            // Act
            var b = new RqlBool(true);
            var implicitB = (bool)b;

            // Assert
            b.Value.Should().BeTrue();
            b.Type.Should().Be(RqlTypes.Bool);
            b.RuntimeType.Should().Be(typeof(bool));
            b.RuntimeValue.Should().Be(true);
            implicitB.Should().BeTrue();
        }

        [Fact]
        public void Equals_SameValue_AreEqualAndHashCodesEqual()
        {
            var a = (RqlBool)true;
            var b = (RqlBool)true;

            a.Equals(b).Should().BeTrue();
            a.Equals((object)b).Should().BeTrue();
            a.GetHashCode().Should().Be(b.GetHashCode());
        }

        [Fact]
        public void Equals_DifferentValue_AreNotEqual()
        {
            var a = (RqlBool)true;
            var b = (RqlBool)false;

            a.Equals(b).Should().BeFalse();
        }

        [Fact]
        public void Equals_NonRqlBool_AreNotEqual()
        {
            var a = (RqlBool)true;
            var b = "not a bool";
            a.Equals(b).Should().BeFalse();
        }

        [Fact]
        public void ToString_GivenTrueValue_ReturnsExpectedFormat()
        {
            // Arrange
            var b = new RqlBool(true);

            // Act
            var result = b.ToString();

            // Assert
            result.Should().Be("<bool> True");
        }
    }
}
