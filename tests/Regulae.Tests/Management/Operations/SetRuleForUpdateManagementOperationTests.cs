namespace Regulae.Tests.Management.Operations
{
    using System.Threading.Tasks;
    using FluentAssertions;
    using Regulae;
    using Regulae.Management.Operations;
    using Xunit;

    public class SetRuleForUpdateManagementOperationTests
    {
        [Fact]
        public async Task ApplyAsync_ReplacesRuleWithUpdated()
        {
            // Arrange
            var r1 = Rule.Create("a").InRuleset("rs").SetContent("c").Since(System.DateTime.UtcNow).Build().Rule;
            r1.Priority = 1;
            var r2 = Rule.Create("b").InRuleset("rs").SetContent("c").Since(System.DateTime.UtcNow).Build().Rule;
            r2.Priority = 2;

            var updated = Rule.Create("a").InRuleset("rs").SetContent("updated").Since(System.DateTime.UtcNow).Build().Rule;
            updated.Priority = 9;

            var op = new SetRuleForUpdateManagementOperation(updated);

            // Act
            var result = await op.ApplyAsync([r1, r2]);

            // Assert
            result.Should().Contain(updated);
            result.Should().Contain(r2);
        }
    }
}
