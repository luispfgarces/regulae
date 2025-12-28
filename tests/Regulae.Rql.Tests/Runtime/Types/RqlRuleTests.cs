namespace Regulae.Rql.Tests.Runtime.Types
{
    using System;
    using System.Globalization;
    using System.Text;
    using FluentAssertions;
    using Regulae;
    using Regulae.Rql.Runtime.Types;
    using Xunit;

    public class RqlRuleTests
    {
        [Fact]
        public void Ctor_GivenRule_SetsValue()
        {
            // Arrange
            var rule = Rule.Create("n")
                .InRuleset("rs")
                .SetContent(new object())
                .Since(DateTime.UtcNow)
                .ApplyWhen(x => x
                    .Or(o => o
                        .And(a => a
                            .Value("SingleInteger", Operators.Equal, 2)
                            .Value("SingleString", Operators.Equal, "test")
                            .Value("SingleDecimal", Operators.NotEqual, 10.5m)
                            .Value("SingleBoolean", Operators.NotEqual, true)
                        )
                        .And(a => a
                            .Value("MultipleInteger", Operators.In, new[] { 1, 2, 3 })
                            .Value("MultipleString", Operators.In, new[] { "a", "b", "c" })
                            .Value("MultipleDecimal", Operators.NotIn, new[] { 5.1m, 10.2m, 15.3m })
                            .Value("MultipleBoolean", Operators.NotIn, new[] { true, false })
                        )
                    )
                )
                .Build().Rule;

            // Act
            var r = new RqlRule(rule);

            // Assert
            r.Value.Should().Be(rule);
            r.Type.Should().Be(RqlTypes.Rule);
            r.RuntimeValue.Should().Be(rule);
            r.RuntimeType.Should().Be(typeof(Rule));
        }

        [Fact]
        public void Equals_SameRuleReference_AreEqualAndHashCodesEqual()
        {
            var rule = Rule.Create("Rule A")
                .InRuleset("RS")
                .SetContent(new object())
                .Since(DateTime.Parse("2024-01-01Z"))
                .Build().Rule;

            var r1 = new RqlRule(rule);
            var r2 = new RqlRule(rule);

            r1.Equals(r2).Should().BeTrue();
            r1.GetHashCode().Should().Be(r2.GetHashCode());
        }

        [Fact]
        public void Equals_DifferentRuleReferences_AreNotEqual()
        {
            var rule1 = Rule.Create("Rule 1").InRuleset("RS").SetContent(new object()).Since(DateTime.UtcNow).Build().Rule;
            var rule2 = Rule.Create("Rule 2").InRuleset("RS").SetContent(new object()).Since(DateTime.UtcNow).Build().Rule;

            var r1 = new RqlRule(rule1);
            var r2 = new RqlRule(rule2);

            r1.Equals(r2).Should().BeFalse();
        }

        [Fact]
        public void ToString_GivenRqlRule_ReturnsStringRepresentation()
        {
            // Arrange
            var expected = new StringBuilder()
                .AppendLine("<rule>")
                .AppendLine("{")
                .AppendLine("    Active: <any> (<bool> True)")
                .AppendLine("    DateBegin: <any> (<date> 2025-01-01T00:00:00.000Z)")
                .AppendLine("    DateEnd: <any> (<nothing>)")
                .AppendLine("    Name: <any> (<string> \"Test name\")")
                .AppendLine("    Priority: <any> (<integer> 0)")
                .AppendLine("    RootCondition: <any> (<read_only_object>")
                .AppendLine("    {")
                .AppendLine("        ChildConditionNodes: <any> (<array>")
                .AppendLine("        {")
                .AppendLine("            <any> (<read_only_object>")
                .AppendLine("            {")
                .AppendLine("                Condition: <any> (<string> \"Condition1\")")
                .AppendLine("                LogicalOperator: <any> (<string> \"Eval\")")
                .AppendLine("                Operand: <any> (<read_only_object>")
                .AppendLine("                {")
                .AppendLine("                    Cardinality: <any> (<string> \"One\")")
                .AppendLine("                    DataType: <any> (<string> \"String\")")
                .AppendLine("                    Value: <any> (<string> \"value1\")")
                .AppendLine("                })")
                .AppendLine("                Operator: <any> (<string> \"Equal\")")
                .AppendLine("            }),")
                .AppendLine("            <any> (<read_only_object>")
                .AppendLine("            {")
                .AppendLine("                Condition: <any> (<string> \"Condition2\")")
                .AppendLine("                LogicalOperator: <any> (<string> \"Eval\")")
                .AppendLine("                Operand: <any> (<read_only_object>")
                .AppendLine("                {")
                .AppendLine("                    Cardinality: <any> (<string> \"One\")")
                .AppendLine("                    DataType: <any> (<string> \"Integer\")")
                .AppendLine("                    Value: <any> (<integer> 10)")
                .AppendLine("                })")
                .AppendLine("                Operator: <any> (<string> \"GreaterThan\")")
                .AppendLine("            })")
                .AppendLine("        })")
                .AppendLine("        LogicalOperator: <any> (<string> \"And\")")
                .AppendLine("    })")
                .AppendLine("    Ruleset: <any> (<string> \"Test ruleset\")")
                .Append('}')
                .ToString();
            var rule = Rule.Create("Test name")
                .InRuleset("Test ruleset")
                .SetContent(new object())
                .Since(DateTime.Parse("2025-01-01"))
                .ApplyWhen(x => x
                    .And(a => a
                         .Value("Condition1", Operators.Equal, "value1")
                         .Value("Condition2", Operators.GreaterThan, 10)
                    )
                )
                .Build().Rule;
            var r = new RqlRule(rule);

            var originalCulture = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

                // Act
                var result = r.ToString();

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
