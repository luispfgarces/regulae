namespace Regulae.Rql.Tests.Runtime.Types
{
    using System;
    using FluentAssertions;
    using Regulae.Rql.Runtime.Types;
    using Xunit;

    public class RqlDateTests
    {
        [Fact]
        public void Ctor_GivenDate_SetsValue()
        {
            // Act
            var value = DateTime.Parse("2025-12-31Z");
            var b = new RqlDate(value);
            var implicitB = (DateTime)b;

            // Assert
            b.Value.Should().Be(value);
            b.Type.Should().Be(RqlTypes.Date);
            b.RuntimeType.Should().Be(typeof(DateTime));
            b.RuntimeValue.Should().Be(value);
            implicitB.Should().Be(value);
        }

        [Fact]
        public void Equals_SameValue_AreEqualAndHashCodesEqual()
        {
            var dt = DateTime.Parse("2024-01-01Z");
            var a = (RqlDate)dt;
            var b = (RqlDate)dt;

            a.Equals(b).Should().BeTrue();
            a.Equals((object)b).Should().BeTrue();
            a.GetHashCode().Should().Be(b.GetHashCode());
        }

        [Fact]
        public void Equals_DifferentValue_AreNotEqual()
        {
            var a = (RqlDate)DateTime.Parse("2024-01-01Z");
            var b = (RqlDate)DateTime.Parse("2025-01-01Z");

            a.Equals(b).Should().BeFalse();
        }

        [Fact]
        public void Equals_DifferentType_AreNotEqual()
        {
            var a = (RqlDate)DateTime.Parse("2024-01-01Z");
            var b = (RqlString)"2024-01-01";
            a.Equals(b).Should().BeFalse();
        }
    }
}
