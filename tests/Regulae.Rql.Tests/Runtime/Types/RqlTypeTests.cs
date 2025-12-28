namespace Regulae.Rql.Tests.Runtime.Types
{
    using System;
    using FluentAssertions;
    using Regulae.Rql.Runtime.Types;
    using Xunit;

    public class RqlTypeTests
    {
        [Fact]
        public void Ctor_InvalidName_ThrowsArgumentException()
        {
            // Act
            Action a = () => _ = new RqlType(" ");

            // Assert
            a.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Ctor_ValidName_SetsName()
        {
            // Act
            var t = new RqlType("myType");

            // Assert
            t.Name.Should().Be("myType");
        }

        [Fact]
        public void Equals_SameName_AreEqualAndOperatorWorks()
        {
            var assignableType = new RqlType("othertype");
            var t1 = new RqlType("mytype");
            t1.AddAssignableType(assignableType);
            var t2 = new RqlType("mytype");
            t2.AddAssignableType(assignableType);

            t1.Equals(t2).Should().BeTrue();
            (t1 == t2).Should().BeTrue();
            (t1 != t2).Should().BeFalse();
            t1.GetHashCode().Should().Be(t2.GetHashCode());
        }

        [Fact]
        public void Equals_DifferentName_AreNotEqual()
        {
            var t1 = new RqlType("a");
            var t2 = new RqlType("b");

            t1.Equals(t2).Should().BeFalse();
            (t1 == t2).Should().BeFalse();
            (t1 != t2).Should().BeTrue();
        }

        [Fact]
        public void Equals_DifferentTypeAndNull_AreNotEqual()
        {
            var t1 = new RqlType("a");
            var other = new object();

            t1.Equals(other).Should().BeFalse();
            t1.Equals(null).Should().BeFalse();
        }

        [Fact]
        public void AddAssignableType_PreventsSelfAndDuplicates()
        {
            var a = new RqlType("a");
            var b = new RqlType("b");

            a.AddAssignableType(b);
            a.IsAssignableTo(b).Should().BeTrue();
            b.IsAssignableTo(a).Should().BeFalse();
            a.AssignableTypes.Should().ContainSingle().Which.Should().Be(b);

            Action addSelf = () => a.AddAssignableType(a);
            addSelf.Should().Throw<InvalidOperationException>();

            Action addDuplicate = () => a.AddAssignableType(b);
            addDuplicate.Should().Throw<InvalidOperationException>();
        }
    }
}
