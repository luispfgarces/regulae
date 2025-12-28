namespace Regulae.Rql.Tests.Runtime.Types
{
    using System.Collections.Generic;
    using System.Text;
    using FluentAssertions;
    using Regulae.Rql.Runtime.Types;
    using Xunit;

    public class RqlObjectTests
    {
        [Fact]
        public void Ctor_Default_NoProperties()
        {
            // Act
            var obj = new RqlObject();
            var implicitObj = (RqlAny)obj;

            // Assert
            var dict = (IDictionary<string, object>)obj.Value;
            dict.Should().BeEmpty();
            obj.Type.Should().Be(RqlTypes.Object);
            obj.RuntimeType.Should().Be(typeof(object));
            obj.RuntimeValue.Should().BeEquivalentTo(dict);
            implicitObj.Value.Should().BeEquivalentTo(dict);
        }

        [Fact]
        public void Equals_GivenSameProperties_ReturnsTrue()
        {
            // Arrange
            var obj1 = new RqlObject();
            obj1.SetPropertyValue("Name", new RqlString("Alice"));
            obj1.SetPropertyValue("Age", new RqlInteger(25));
            var obj2 = new RqlObject();
            obj2.SetPropertyValue("Name", new RqlString("Alice"));
            obj2.SetPropertyValue("Age", new RqlInteger(25));

            // Act
            obj1.Equals(obj2).Should().BeTrue();
            obj1.Equals((object)obj2).Should().BeTrue();
            obj1.GetHashCode().Should().Be(obj2.GetHashCode());
        }

        [Fact]
        public void Equals_GivenDifferentProperties_ReturnsFalse()
        {
            // Arrange
            var obj1 = new RqlObject();
            obj1.SetPropertyValue("Name", new RqlString("Alice"));
            obj1.SetPropertyValue("Age", new RqlInteger(25));
            var obj2 = new RqlObject();
            obj2.SetPropertyValue("Name", new RqlString("Bob"));
            obj2.SetPropertyValue("Age", new RqlInteger(30));

            // Act
            var areEqual = obj1.Equals(obj2);

            // Assert
            areEqual.Should().BeFalse();
        }

        [Fact]
        public void SetPropertyValue_GivenVariousProperties_AddsAll()
        {
            // Arrange
            var obj = new RqlObject();
            var tags = new RqlArray(3);
            tags.SetAtIndex(0, new RqlString("admin"));
            tags.SetAtIndex(1, new RqlString("employed"));
            tags.SetAtIndex(2, new RqlString("parkUser"));

            // Act
            obj.SetPropertyValue("Name", new RqlString("Bob"));
            obj.SetPropertyValue("Age", new RqlInteger(30));
            obj.SetPropertyValue("Tags", tags);

            // Assert
            var dict = (IDictionary<string, object>)obj.Value;
            dict.Should().ContainKey("Name");
            dict["Name"].Should().Be("Bob");
            dict.Should().ContainKey("Age");
            dict["Age"].Should().Be(30);
            dict.Should().ContainKey("Tags");
            dict["Tags"].Should().BeEquivalentTo(new object[] { "admin", "employed", "parkUser" });
        }

        [Fact]
        public void ToString_GivenObject_ReturnsPrettyString()
        {
            // Arrange
            var expectedString = new StringBuilder()
                .AppendLine("<object>")
                .Append('{')
                .AppendLine()
                .AppendLine("    Name: <any> (<string> \"Bob\")")
                .AppendLine("    Age: <any> (<integer> 30)")
                .AppendLine("    Tags: <any> (<array>")
                .AppendLine("    {")
                .AppendLine("        <any> (<string> \"admin\"),")
                .AppendLine("        <any> (<string> \"employed\"),")
                .AppendLine("        <any> (<string> \"parkUser\")")
                .AppendLine("    })")
                .Append('}')
                .ToString();

            var obj = new RqlObject();
            var tags = new RqlArray(3);
            tags.SetAtIndex(0, new RqlString("admin"));
            tags.SetAtIndex(1, new RqlString("employed"));
            tags.SetAtIndex(2, new RqlString("parkUser"));
            obj.SetPropertyValue("Name", new RqlString("Bob"));
            obj.SetPropertyValue("Age", new RqlInteger(30));
            obj.SetPropertyValue("Tags", tags);

            // Act
            var prettyString = obj.ToString();

            // Assert

            prettyString.Should().Be(expectedString);
        }
    }
}
