namespace Regulae.Rql.Tests.Runtime.Types
{
    using System.Globalization;
    using FluentAssertions;
    using Regulae.Rql.Runtime.Types;
    using Xunit;

    public class RqlDecimalTests
    {
        [Fact]
        public void Ctor_GivenDecimal_SetsValue()
        {
            // Arrange
            var expected = 42.42m;

            // Act
            var rqlDecimal = new RqlDecimal(expected);
            var implicitRqlDecimal = (decimal)rqlDecimal;

            // Assert
            rqlDecimal.Value.Should().Be(expected);
            rqlDecimal.Type.Should().Be(RqlTypes.Decimal);
            rqlDecimal.RuntimeValue.Should().Be(expected);
            rqlDecimal.RuntimeType.Should().Be(typeof(decimal));
            implicitRqlDecimal.Should().Be(expected);
        }

        [Fact]
        public void Equals_SameValue_AreEqualAndHashCodesEqual()
        {
            var a = (RqlDecimal)1.5m;
            var b = (RqlDecimal)1.5m;

            a.Equals(b).Should().BeTrue();
            a.Equals((object)b).Should().BeTrue();
            a.GetHashCode().Should().Be(b.GetHashCode());
        }

        [Fact]
        public void Equals_DifferentValue_AreNotEqual()
        {
            var a = (RqlDecimal)1.5m;
            var b = (RqlDecimal)2.5m;

            a.Equals(b).Should().BeFalse();
        }

        [Fact]
        public void Equals_NonRqlDecimal_AreNotEqual()
        {
            var a = (RqlDecimal)1.5m;
            var b = "not a decimal";
            a.Equals(b).Should().BeFalse();
        }

        [Fact]
        public void ToString_GivenDecimalValue_ReturnsExpectedFormat()
        {
            // Arrange
            RqlDecimal d = 10.5m;
            var originalCulture = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

                // Act
                var stringValue = d.ToString();

                // Assert
                stringValue.Should().Be("<decimal> 10.5");
            }
            finally
            {
                CultureInfo.CurrentCulture = originalCulture;
            }
        }
    }
}
