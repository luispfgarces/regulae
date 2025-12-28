namespace Regulae.Rql.Tests.Runtime.Types
{
    using FluentAssertions;
    using Regulae.Rql.Runtime.Types;
    using Xunit;

    public class RqlStringTests
    {
        [Fact]
        public void Ctor_GivenStringValue_SetsValue()
        {
            // Act
            var rqlString = new RqlString("hello");

            // Assert
            rqlString.Value.Should().Be("hello");
            rqlString.Type.Should().Be(RqlTypes.String);
            rqlString.RuntimeType.Should().Be(typeof(string));
            rqlString.RuntimeValue.Should().Be("hello");
        }

        [Fact]
        public void Equals_SameValue_AreEqualAndHashCodesEqual()
        {
            var a = (RqlString)"abc";
            var b = (RqlString)"abc";

            a.Equals(b).Should().BeTrue();
            a.Equals((object)b).Should().BeTrue();
            (a == b).Should().BeTrue(); // uses Equals override
            a.GetHashCode().Should().Be(b.GetHashCode());
        }

        [Fact]
        public void Equals_DifferentValue_AreNotEqual()
        {
            var a = (RqlString)"abc";
            var b = (RqlString)"def";

            a.Equals(b).Should().BeFalse();
            a.GetHashCode().Should().NotBe(b.GetHashCode());
        }

        [Fact]
        public void ToString_GivenRqlString_ReturnsPrettyString()
        {
            // Arrange
            var rqlString = new RqlString("example");

            // Act
            var result = rqlString.ToString();

            // Assert
            result.Should().Be("<string> \"example\"");
        }
    }
}
