namespace Regulae.Rql.Tests.Runtime.Types
{
    using System;
    using System.Globalization;
    using System.Text;
    using FluentAssertions;
    using Regulae.Rql.Runtime.Types;
    using Xunit;

    public class RqlArrayTests
    {
        [Fact]
        public void RuntimeType_ReturnsObjectArrayType()
        {
            // Arrange
            var a = new RqlArray(0);

            // Act
            var runtimeType = a.RuntimeType;

            // Assert
            runtimeType.Should().Be(typeof(object[]));
        }

        [Fact]
        public void RuntimeValue_GivenArrayWithIntegerAndString_ReturnsObjectArrayWithBothNativeValues()
        {
            // Arrange
            var a = new RqlArray(2);

            // Act
            a.SetAtIndex(0, (RqlAny)new RqlInteger(5));
            a.SetAtIndex(1, (RqlAny)new RqlString("s"));

            // Assert
            var native = a.RuntimeValue;
            native.Should().BeEquivalentTo(new object[] { 5, "s" });
        }

        [Fact]
        public void Ctor_UnfilledArray_FillsIndexesWithNothing()
        {
            // Act
            var a = new RqlArray(2);

            // Assert
            a.Size.Value.Should().Be(2);
            a.Value[0].Unwrap().Should().BeOfType<RqlNothing>();
            a.Value[1].Unwrap().Should().BeOfType<RqlNothing>();
        }

        [Fact]
        public void Ctor_ArrayWithNegativeSize_ThrowsArgumentOutOfRangeException()
        {
            // Act
            Action act = () => _ = new RqlArray(-1);

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void Equals_SameSizeAndValues_AreEqualAndHashCodesEqual()
        {
            var a = new RqlArray(2);
            a.SetAtIndex((RqlInteger)0, (RqlAny)((RqlInteger)1));
            a.SetAtIndex((RqlInteger)1, (RqlAny)((RqlInteger)2));

            var b = new RqlArray(2);
            b.SetAtIndex((RqlInteger)0, (RqlAny)((RqlInteger)1));
            b.SetAtIndex((RqlInteger)1, (RqlAny)((RqlInteger)2));

            a.Equals(b).Should().BeTrue();
            a.GetHashCode().Should().Be(b.GetHashCode());
        }

        [Fact]
        public void Equals_DifferentSize_AreNotEqual()
        {
            var a = new RqlArray(1);
            var b = new RqlArray(2);

            a.Equals(b).Should().BeFalse();
        }

        [Fact]
        public void Equals_DifferentElements_AreNotEqual()
        {
            var a = new RqlArray(1);
            a.SetAtIndex((RqlInteger)0, (RqlAny)((RqlInteger)1));

            var b = new RqlArray(1);
            b.SetAtIndex((RqlInteger)0, (RqlAny)((RqlInteger)2));

            a.Equals(b).Should().BeFalse();
        }

        [Fact]
        public void SetAtIndex_GivenIntegerAndStringOnValidIndexes_StoresBothValues()
        {
            // Arrange
            var a = new RqlArray(2);

            // Act
            a.SetAtIndex(0, (RqlAny)new RqlInteger(5));
            a.SetAtIndex(1, (RqlAny)new RqlString("s"));

            // Assert
            a.Size.Value.Should().Be(2);
            a.Value[0].Unwrap().Should().BeOfType<RqlInteger>();
            a.Value[1].Unwrap().Should().BeOfType<RqlString>();
        }

        [Fact]
        public void SetAtIndex_GivenInvalidIndex_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var a = new RqlArray(2);

            // Act
            Action actNegative = () => a.SetAtIndex((RqlInteger)(-1), (RqlAny)new RqlInteger(10));
            Action actTooLarge = () => a.SetAtIndex((RqlInteger)2, (RqlAny)new RqlInteger(10));

            // Assert
            actNegative.Should().Throw<ArgumentOutOfRangeException>();
            actTooLarge.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void ToString_ArrayWith0Elements_ReturnsEmptyFormat()
        {
            // Arrange

            var expected = "<array> { (empty) }";

            var a = new RqlArray(0);

            // Act
            var result = a.ToString();

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void ToString_ArrayWith3Elements_ReturnsFormatWithAllElements()
        {
            // Arrange

            var expected = new StringBuilder()
                .AppendLine("<array>")
                .AppendLine("{")
                .AppendLine("    <any> (<integer> 1),")
                .AppendLine("    <any> (<string> \"two\"),")
                .AppendLine("    <any> (<integer> 3)")
                .Append('}')
                .ToString();

            var a = new RqlArray(3);
            a.SetAtIndex(0, (RqlAny)new RqlInteger(1));
            a.SetAtIndex(1, (RqlAny)new RqlString("two"));
            a.SetAtIndex(2, (RqlAny)new RqlInteger(3));

            // Act
            var result = a.ToString();

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void ToString_ArrayWith6Elements_ReturnsFormatWithOnly5Elements()
        {
            // Arrange

            var expected = new StringBuilder()
                .AppendLine("<array>")
                .AppendLine("{")
                .AppendLine("    <any> (<integer> 1),")
                .AppendLine("    <any> (<string> \"two\"),")
                .AppendLine("    <any> (<integer> 3),")
                .AppendLine("    <any> (<decimal> 3.0),")
                .AppendLine("    <any> (<array>")
                .AppendLine("    {")
                .AppendLine("        <any> (<integer> 42)")
                .AppendLine("    }),")
                .AppendLine("    ...")
                .Append('}')
                .ToString();
            var innerArray = new RqlArray(1);
            innerArray.SetAtIndex(0, (RqlAny)new RqlInteger(42));

            var a = new RqlArray(6);
            a.SetAtIndex(0, (RqlAny)new RqlInteger(1));
            a.SetAtIndex(1, (RqlAny)new RqlString("two"));
            a.SetAtIndex(2, (RqlAny)new RqlInteger(3));
            a.SetAtIndex(3, (RqlAny)new RqlDecimal(3.0m));
            a.SetAtIndex(4, (RqlAny)innerArray);
            a.SetAtIndex(5, (RqlAny)new RqlDate(DateTime.Parse("2025-01-01")));

            var originalCulture = CultureInfo.CurrentCulture;
            try
            {
                // Act
                CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
                var result = a.ToString();

                // Assert
                result.Should().Be(expected);
            }
            finally
            {
                CultureInfo.CurrentCulture = originalCulture;
            }
        }
    }
}
