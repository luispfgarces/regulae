namespace Regulae.Tests.Management.Operations
{
    using System.Threading.Tasks;
    using FluentAssertions;
    using Regulae;
    using Regulae.Management.Operations;
    using Xunit;

    public class FilterPrioritiesRangeManagementOperationTests
    {
        [Fact]
        public async Task ApplyAsync_FiltersByTopThreshold()
        {
            // Arrange
            var r1 = Rule.Create("a").InRuleset("rs").SetContent("c").Since(System.DateTime.UtcNow).Build().Rule; r1.Priority = 1;
            var r2 = Rule.Create("b").InRuleset("rs").SetContent("c").Since(System.DateTime.UtcNow).Build().Rule; r2.Priority = 5;
            var r3 = Rule.Create("c").InRuleset("rs").SetContent("c").Since(System.DateTime.UtcNow).Build().Rule; r3.Priority = 10;

            var op = new FilterPrioritiesRangeManagementOperation(1, 5);

            // Act
            var result = await op.ApplyAsync([r1, r2, r3]);

            // Assert
            result.Should().Contain(r1).And.Contain(r2).And.NotContain(r3);
        }

        [Fact]
        public async Task ApplyAsync_WithUpdatedRule_AdjustsThresholds()
        {
            // Arrange
            var r1 = Rule.Create("a").InRuleset("rs").SetContent("c").Since(System.DateTime.UtcNow).Build().Rule; r1.Priority = 1;
            var r2 = Rule.Create("b").InRuleset("rs").SetContent("c").Since(System.DateTime.UtcNow).Build().Rule; r2.Priority = 5;
            var r3 = Rule.Create("c").InRuleset("rs").SetContent("c").Since(System.DateTime.UtcNow).Build().Rule; r2.Priority = 5;

            var updated = Rule.Create("a").InRuleset("rs").SetContent("c").Since(System.DateTime.UtcNow).Build().Rule; updated.Priority = 7; // changed

            var op = new FilterPrioritiesRangeManagementOperation(updated);

            // Act
            var result = await op.ApplyAsync([r1, r2, r3]);

            // Assert
            result.Should().Contain(r1).And.Contain(r2).And.NotContain(r3);
        }
    }
}
