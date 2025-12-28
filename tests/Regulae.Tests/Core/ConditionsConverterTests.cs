namespace Regulae.Tests.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using Regulae;
    using Regulae.Core;
    using Regulae.Source;
    using Xunit;

    public class ConditionsConverterTests
    {
        [Fact]
        public async Task ConvertConditionsAsync_ConvertsAllSupportedTypes()
        {
            var conditionsMeta = new Dictionary<string, Condition>
            {
                { "str", new Condition("str", DateTime.UtcNow, DataTypes.String) },
                { "int", new Condition("int", DateTime.UtcNow, DataTypes.Integer) },
                { "dec", new Condition("dec", DateTime.UtcNow, DataTypes.Decimal) },
                { "bool", new Condition("bool", DateTime.UtcNow, DataTypes.Boolean) }
            };

            var rulesSource = new Mock<IRulesSource>();
            rulesSource.Setup(x => x.GetConditionsAsync(It.IsAny<GetConditionsArgs>()))
                .ReturnsAsync(conditionsMeta);

            var converter = new ConditionsConverter(rulesSource.Object);

            var input = new Dictionary<string, object>
            {
                { "str", "hello" },
                { "int", 42 },
                { "dec", 3.14m },
                { "bool", true }
            };

            var result = await converter.ConvertConditionsAsync(input);

            result.Should().HaveCount(4);
            result["str"].Value.Should().Be("hello");
            result["int"].Value.Should().Be(42);
            result["dec"].Value.Should().Be(3.14m);
            result["bool"].Value.Should().Be(true);
            result["str"].DataType.Should().Be(DataTypes.String);
            result["int"].DataType.Should().Be(DataTypes.Integer);
            result["dec"].DataType.Should().Be(DataTypes.Decimal);
            result["bool"].DataType.Should().Be(DataTypes.Boolean);
            result["str"].Cardinality.Should().Be(Cardinalities.One);
        }

        [Fact]
        public async Task ConvertConditionsAsync_ThrowsIfConditionNotFound()
        {
            var rulesSource = new Mock<IRulesSource>();
            rulesSource.Setup(x => x.GetConditionsAsync(It.IsAny<GetConditionsArgs>()))
                .ReturnsAsync(new Dictionary<string, Condition>());

            var converter = new ConditionsConverter(rulesSource.Object);
            var input = new Dictionary<string, object> { { "missing", "value" } };

            Func<Task> act = async () => await converter.ConvertConditionsAsync(input);

            await act.Should().ThrowAsync<ArgumentOutOfRangeException>()
                .WithMessage("*does not exist*");
        }

        [Fact]
        public async Task ConvertConditionsAsync_ConvertsEnumerableValues()
        {
            var conditionsMeta = new Dictionary<string, Condition>
            {
                { "ints", new Condition("ints", DateTime.UtcNow, DataTypes.Integer) },
                { "bools", new Condition("bools", DateTime.UtcNow, DataTypes.Boolean) },
                { "strs", new Condition("strs", DateTime.UtcNow, DataTypes.String) },
                { "decs", new Condition("decs", DateTime.UtcNow, DataTypes.Decimal) },
            };

            var rulesSource = new Mock<IRulesSource>();
            rulesSource.Setup(x => x.GetConditionsAsync(It.IsAny<GetConditionsArgs>()))
                .ReturnsAsync(conditionsMeta);

            var converter = new ConditionsConverter(rulesSource.Object);
            var input = new Dictionary<string, object>
            {
                { "ints", new[] { 1, 2, 3 } },
                { "bools", new List<bool> { true, false } },
                { "strs", new ArrayList { "a", "b", "c" } },
                { "decs", new List<decimal> { 1.1m, 2.2m }.Select(v => v) }
            };

            var result = await converter.ConvertConditionsAsync(input);

            result["ints"].Cardinality.Should().Be(Cardinalities.Many);
            result["ints"].Value.Should().BeAssignableTo<IEnumerable>();
            result["ints"].Value.Should().BeEquivalentTo(new[] { 1, 2, 3 });
            result["bools"].Cardinality.Should().Be(Cardinalities.Many);
            result["bools"].Value.Should().BeEquivalentTo(new[] { true, false });
            result["strs"].Cardinality.Should().Be(Cardinalities.Many);
            result["strs"].Value.Should().BeEquivalentTo(new[] { "a", "b", "c" });
            result["decs"].Cardinality.Should().Be(Cardinalities.Many);
            result["decs"].Value.Should().BeEquivalentTo(new[] { 1.1m, 2.2m });
        }

        [Theory]
        [InlineData(DataTypes.Boolean, "true", true)]
        [InlineData(DataTypes.Boolean, "false", false)]
        [InlineData(DataTypes.Integer, "123", 123)]
        [InlineData(DataTypes.Decimal, "1.23", 1.23)]
        [InlineData(DataTypes.String, 456, "456")]
        public async Task ConvertConditionsAsync_ConvertsStringRepresentations(DataTypes type, object inputValue, object expected)
        {
            var condName = "cond";
            var conditionsMeta = new Dictionary<string, Condition>
            {
                { condName, new Condition(condName, DateTime.UtcNow, type) }
            };

            var rulesSource = new Mock<IRulesSource>();
            rulesSource.Setup(x => x.GetConditionsAsync(It.IsAny<GetConditionsArgs>()))
                .ReturnsAsync(conditionsMeta);

            var converter = new ConditionsConverter(rulesSource.Object);
            var input = new Dictionary<string, object> { { condName, inputValue } };

            var result = await converter.ConvertConditionsAsync(input);

            result[condName].Value.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ConvertSingle_ThrowsForUnsupportedType()
        {
            var method = typeof(ConditionsConverter).GetMethod("ConvertSingle", BindingFlags.NonPublic | BindingFlags.Static);
            Action act = () => method.Invoke(null, new object[] { (DataTypes)999, "val" });
            act.Should().Throw<TargetInvocationException>()
                .WithInnerException<NotSupportedException>()
                .WithMessage("The data type '999' is not supported.");
        }

        [Fact]
        public void ConvertEnumerable_ThrowsForUnsupportedType()
        {
            var method = typeof(ConditionsConverter).GetMethod("ConvertEnumerable", BindingFlags.NonPublic | BindingFlags.Static);
            Action act = () => method.Invoke(null, new object[] { (DataTypes)999, new[] { "a", "b" } });
            act.Should().Throw<TargetInvocationException>()
                .WithInnerException<NotSupportedException>()
                .WithMessage("The data type '999' is not supported.");
        }

        [Theory]
        [InlineData(DataTypes.Integer, "notanint")]
        [InlineData(DataTypes.Boolean, "notaboolean")]
        [InlineData(DataTypes.Decimal, "notadecimal")]
        public void ConvertSingle_ThrowsForInvalidCast(DataTypes dataType, object value)
        {
            var method = typeof(ConditionsConverter).GetMethod("ConvertSingle", BindingFlags.NonPublic | BindingFlags.Static);
            Action act = () => method.Invoke(null, new object[] { dataType, value });
            act.Should().Throw<TargetInvocationException>()
                .WithInnerException<InvalidCastException>()
                .WithMessage("The value nota* is not convertible to *.");
        }
    }
}