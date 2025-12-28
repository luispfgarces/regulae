namespace Regulae.Tests.Extensions
{
    using System;
    using System.Globalization;
    using FluentAssertions;
    using Regulae;
    using Regulae.Extensions;
    using Regulae.Tests.TestStubs;
    using Xunit;

    public class RuleBuilderExtensionsTests
    {
        [Fact]
        public void Since_String_SetsDateBegin_UsingCurrentCulture()
        {
            var originalCulture = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

                var dateStr = "2024-08-17";
                var builder = Rule.Create("r").InRuleset("rs").SetContent(new object());

                var result = builder.Since(dateStr).Until(null).Build();

                result.IsSuccess.Should().BeTrue();
                result.Rule!.DateBegin.Should().Be(DateTime.Parse(dateStr, CultureInfo.CurrentCulture, DateTimeStyles.None));
            }
            finally
            {
                CultureInfo.CurrentCulture = originalCulture;
            }
        }

        [Fact]
        public void Since_Generic_String_SetsDateBegin()
        {
            var originalCulture = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

                var dateStr = "2023-12-01";
                var builder = Rule.Create<RulesetNames, ConditionNames>("r").InRuleset(RulesetNames.Type1).SetContent(new object());

                var result = builder.Since(dateStr).Until(null).Build();

                result.IsSuccess.Should().BeTrue();
                result.Rule!.DateBegin.Should().Be(DateTime.Parse(dateStr, CultureInfo.CurrentCulture, DateTimeStyles.None));
            }
            finally
            {
                CultureInfo.CurrentCulture = originalCulture;
            }
        }

        [Fact]
        public void SinceUtc_SetsUtcKind()
        {
            var builder = Rule.Create("r").InRuleset("rs").SetContent(new object());

            var result = builder.SinceUtc(2020, 1, 2).Until(null).Build();

            result.IsSuccess.Should().BeTrue();
            result.Rule!.DateBegin.Kind.Should().Be(DateTimeKind.Utc);
            result.Rule.DateBegin.Year.Should().Be(2020);
            result.Rule.DateBegin.Month.Should().Be(1);
            result.Rule.DateBegin.Day.Should().Be(2);
        }

        [Fact]
        public void SinceUtc_Generic_SetsUtcKind()
        {
            var builder = Rule.Create<RulesetNames, ConditionNames>("r").InRuleset(RulesetNames.Type1).SetContent(new object());

            var result = builder.SinceUtc(2019, 5, 6).Until(null).Build();

            result.IsSuccess.Should().BeTrue();
            result.Rule!.DateBegin.Kind.Should().Be(DateTimeKind.Utc);
            result.Rule.DateBegin.Year.Should().Be(2019);
            result.Rule.DateBegin.Month.Should().Be(5);
            result.Rule.DateBegin.Day.Should().Be(6);
        }

        [Fact]
        public void Until_Generic_String_SetsDateEnd_WhenValueProvided()
        {
            var originalCulture = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

                var dateBegin = DateTime.Parse("2025-01-01");
                var dateEndStr = "2025-01-02";
                var builder = Rule.Create<RulesetNames, ConditionNames>("r").InRuleset(RulesetNames.Type2).SetContent(new object()).Since(dateBegin);

                var result = builder.Until(dateEndStr).Build();

                result.IsSuccess.Should().BeTrue();
                result.Rule!.DateEnd.Should().Be(DateTime.Parse(dateEndStr, CultureInfo.CurrentCulture, DateTimeStyles.None));
            }
            finally
            {
                CultureInfo.CurrentCulture = originalCulture;
            }
        }

        [Fact]
        public void Until_String_SetsDateEnd_WhenValueProvided()
        {
            var originalCulture = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

                var dateBegin = DateTime.Parse("2025-01-01");
                var dateEndStr = "2025-01-02";
                var builder = Rule.Create("r").InRuleset("rs").SetContent(new object()).Since(dateBegin);

                var result = builder.Until(dateEndStr).Build();

                result.IsSuccess.Should().BeTrue();
                result.Rule!.DateEnd.Should().Be(DateTime.Parse(dateEndStr, CultureInfo.CurrentCulture, DateTimeStyles.None));
            }
            finally
            {
                CultureInfo.CurrentCulture = originalCulture;
            }
        }

        [Fact]
        public void Until_Generic_String_Null_LeavesDateEndNull()
        {
            var builder = Rule.Create<RulesetNames, ConditionNames>("r").InRuleset(RulesetNames.Type2).SetContent(new object()).Since(DateTime.UtcNow);

            var result = builder.Until((string)null).Build();

            result.IsSuccess.Should().BeTrue();
            result.Rule!.DateEnd.Should().BeNull();
        }

        [Fact]
        public void Until_String_Null_LeavesDateEndNull()
        {
            var builder = Rule.Create("r").InRuleset("rs").SetContent(new object()).Since(DateTime.UtcNow);

            var result = builder.Until((string)null).Build();

            result.IsSuccess.Should().BeTrue();
            result.Rule!.DateEnd.Should().BeNull();
        }

        [Fact]
        public void UntilUtc_SetsDateEnd_WithUtcKind()
        {
            var builder = Rule.Create("r").InRuleset("rs").SetContent(new object()).Since(DateTime.UtcNow);

            var result = builder.UntilUtc(2030, 2, 3).Build();

            result.IsSuccess.Should().BeTrue();
            result.Rule!.DateEnd.Should().NotBeNull();
            result.Rule.DateEnd.Value.Kind.Should().Be(DateTimeKind.Utc);
            result.Rule.DateEnd.Value.Year.Should().Be(2030);
            result.Rule.DateEnd.Value.Month.Should().Be(2);
            result.Rule.DateEnd.Value.Day.Should().Be(3);
        }

        [Fact]
        public void UntilUtc_Generic_SetsDateEnd_WithUtcKind()
        {
            var builder = Rule.Create<RulesetNames, ConditionNames>("r").InRuleset(RulesetNames.Type1).SetContent(new object()).Since(DateTime.UtcNow);

            var result = builder.UntilUtc(2031, 3, 4).Build();

            result.IsSuccess.Should().BeTrue();
            result.Rule!.DateEnd.Should().NotBeNull();
            result.Rule.DateEnd.Value.Kind.Should().Be(DateTimeKind.Utc);
            result.Rule.DateEnd.Value.Year.Should().Be(2031);
        }
    }
}
