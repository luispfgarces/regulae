namespace Regulae.Rql.Tests.Runtime.Types
{
    using System;
    using FluentAssertions;
    using Regulae;
    using Regulae.Rql.Runtime.Types;
    using Xunit;

    public class RqlRulesetTests
    {
        [Fact]
        public void Ctor_GivenNameAndCreationDate_SetsProperties()
        {
            // Arrange
            var creationDate = DateTime.UtcNow;
            var rs = new Ruleset("TestRuleset", creationDate);

            // Act
            var rrs = new RqlRuleset(rs);

            // Assert
            rrs.Value.Should().BeSameAs(rs);
            rrs.Type.Should().Be(RqlTypes.Ruleset);
            rrs.RuntimeValue.Should().BeSameAs(rs);
            rrs.RuntimeType.Should().Be(typeof(Ruleset));
        }

        [Fact]
        public void Equals_SameRuleset_AreEqualAndHashCodesEqual()
        {
            var rs1 = new Ruleset("RS1", DateTime.UtcNow);
            var r1 = new RqlRuleset(rs1);
            var r2 = new RqlRuleset(rs1);

            r1.Equals(r2).Should().BeTrue();
            r1.Equals((object)r2).Should().BeTrue();
            r1.GetHashCode().Should().Be(r2.GetHashCode());
        }

        [Fact]
        public void Equals_DifferentType_NotEqual()
        {
            var rs1 = new Ruleset("RS1", DateTime.UtcNow);
            var r1 = new RqlRuleset(rs1);
            var other = new RqlDecimal(10.0m);

            r1.Equals(other).Should().BeFalse();
        }

        [Fact]
        public void ToString_GivenRqlRuleset_ReturnsPrettyString()
        {
            // Arrange
            var expected = "<ruleset> TestRuleset";
            var rs = new Ruleset("TestRuleset", DateTime.UtcNow);
            var rrs = new RqlRuleset(rs);
            // Act
            var result = rrs.ToString();
            // Assert
            result.Should().Be(expected);
        }
    }
}
