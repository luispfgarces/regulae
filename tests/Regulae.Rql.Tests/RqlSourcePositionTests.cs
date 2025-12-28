namespace Regulae.Rql.Tests
{
    using FluentAssertions;
    using Regulae.Rql;
    using Xunit;

    public class RqlSourcePositionTests
    {
        [Fact]
        public void From_CreatesPositionWithGivenLineAndColumn()
        {
            // Act
            var pos = RqlSourcePosition.From(1, 2);

            // Assert
            pos.Line.Should().Be(1u);
            pos.Column.Should().Be(2u);
            pos.ToString().Should().Be("{1:2}");
        }

        [Fact]
        public void Empty_IsZeroZero()
        {
            // Act
            var empty = RqlSourcePosition.Empty;

            // Assert
            empty.Line.Should().Be(0u);
            empty.Column.Should().Be(0u);
        }

        [Fact]
        public void ComparisonOperators_CompareByLineThenColumn()
        {
            // Arrange
            var a = RqlSourcePosition.From(1, 1);
            var b = RqlSourcePosition.From(2, 0);
            var c = RqlSourcePosition.From(2, 5);
            var d = RqlSourcePosition.From(2, 5);

            // Act & Assert
            // line comparison
            (b > a).Should().BeTrue();
            (b >= a).Should().BeTrue();
            (a < b).Should().BeTrue();
            (a <= b).Should().BeTrue();

            // same line, column comparison
            (c > b).Should().BeTrue();
            (b < c).Should().BeTrue();

            // different lines
            (a > b).Should().BeFalse();
            (a >= b).Should().BeFalse();
            (b <= a).Should().BeFalse();
            (b < a).Should().BeFalse();

            // equality behavior
            (c >= d).Should().BeTrue();
            (c <= d).Should().BeTrue();
            (c == d).Should().BeTrue();
            (c != d).Should().BeFalse();
        }

        [Fact]
        public void EqualsAndGetHashCode_BehaveConsistently()
        {
            // Arrange
            var p1 = RqlSourcePosition.From(3, 4);
            var p2 = RqlSourcePosition.From(3, 4);
            var p3 = RqlSourcePosition.From(4, 3);

            // Act & Assert
            p1.Equals(p2).Should().BeTrue();
            p1.Equals((object)p2).Should().BeTrue();
            p1.GetHashCode().Should().Be(p2.GetHashCode());

            p1.Equals(p3).Should().BeFalse();
            p1.Equals("not a position").Should().BeFalse();
            p1.Equals(null).Should().BeFalse();
        }
    }
}
