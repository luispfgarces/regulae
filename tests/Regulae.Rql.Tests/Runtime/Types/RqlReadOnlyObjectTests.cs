namespace Regulae.Rql.Tests.Runtime.Types
{
    using System.Collections.Generic;
    using System.Text;
    using FluentAssertions;
    using Regulae.Rql.Runtime.Types;
    using Xunit;

    public class RqlReadOnlyObjectTests
    {
        [Fact]
        public void Ctor_Default_NoProperties()
        {
            // Act
            var obj = new RqlReadOnlyObject(new Dictionary<string, RqlAny>(StringComparer.Ordinal)
            {
                { "Prop1", new RqlString("Value1") },
                { "Prop2", new RqlInteger(42) }
            });
            var implicitObj = (RqlAny)obj;

            // Assert
            var dict = (IDictionary<string, object>)obj.Value;
            dict.Should().Contain("Prop1", "Value1");
            dict.Should().Contain("Prop2", 42);
            obj.Type.Should().Be(RqlTypes.ReadOnlyObject);
            obj.RuntimeType.Should().Be(typeof(object));
            obj.RuntimeValue.Should().BeEquivalentTo(dict);
            implicitObj.Value.Should().BeEquivalentTo(dict);
        }

        [Fact]
        public void Equals_GivenSameProperties_ReturnsTrue()
        {
            // Arrange
            var properties1 = new Dictionary<string, RqlAny>(StringComparer.Ordinal)
            {
                { "Name", new RqlString("Alice") },
                { "Age", new RqlInteger(25) }
            };
            var obj1 = new RqlReadOnlyObject(properties1);
            var properties2 = new Dictionary<string, RqlAny>(StringComparer.Ordinal)
            {
                { "Name", new RqlString("Alice") },
                { "Age", new RqlInteger(25) }
            };
            var obj2 = new RqlReadOnlyObject(properties2);

            // Act
            obj1.Equals(obj2).Should().BeTrue();
            obj1.Equals((object)obj2).Should().BeTrue();
            obj1.GetHashCode().Should().Be(obj2.GetHashCode());
        }

        [Fact]
        public void Equals_GivenDifferentProperties_ReturnsFalse()
        {
            // Arrange
            var properties1 = new Dictionary<string, RqlAny>(StringComparer.Ordinal)
            {
                { "Name", new RqlString("Alice") },
                { "Age", new RqlInteger(25) },
                { "Gender", new RqlString("Female") },
            };
            var obj1 = new RqlReadOnlyObject(properties1);
            var properties2 = new Dictionary<string, RqlAny>(StringComparer.Ordinal)
            {
                { "Name", new RqlString("Bob") },
                { "Age", new RqlInteger(30) }
            };
            var obj2 = new RqlReadOnlyObject(properties2);

            // Act
            var areEqual = obj1.Equals(obj2);

            // Assert
            areEqual.Should().BeFalse();
        }

        [Fact]
        public void Equals_GivenDifferentType_ReturnsFalse()
        {
            // Arrange
            var properties = new Dictionary<string, RqlAny>(StringComparer.Ordinal)
            {
                { "Name", new RqlString("Alice") },
                { "Age", new RqlInteger(25) }
            };
            var obj = new RqlReadOnlyObject(properties);
            var other = new RqlString("Not an object");

            // Act
            var areEqual = obj.Equals(other);

            // Assert
            areEqual.Should().BeFalse();
        }

        [Fact]
        public void ToString_GivenReadOnlyObject_ReturnsPrettyString()
        {
            // Arrange
            var expectedString = new StringBuilder()
                .AppendLine("<read_only_object>")
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

            var tags = new RqlArray(3);
            tags.SetAtIndex(0, new RqlString("admin"));
            tags.SetAtIndex(1, new RqlString("employed"));
            tags.SetAtIndex(2, new RqlString("parkUser"));
            var properties = new Dictionary<string, RqlAny>(StringComparer.Ordinal)
            {
                { "Name", new RqlString("Bob") },
                { "Age", new RqlInteger(30) },
                { "Tags", tags },
            };
            var obj = new RqlReadOnlyObject(properties);

            // Act
            var prettyString = obj.ToString();

            // Assert

            prettyString.Should().Be(expectedString);
        }
    }
}
