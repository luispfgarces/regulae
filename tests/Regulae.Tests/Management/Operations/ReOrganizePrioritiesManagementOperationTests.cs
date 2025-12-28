namespace Regulae.Tests.Management.Operations
{
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Regulae;
    using Regulae.Management.Operations;
    using Xunit;

    public class ReOrganizePrioritiesManagementOperationTests
    {
        [Fact]
        public async Task ApplyAsync_WithFactor_ShiftsPriorities()
        {
            // Arrange
            var r1 = Rule.Create("a").InRuleset("rs").SetContent("c").Since(System.DateTime.UtcNow).Build().Rule;
            r1.Priority = 1;
            var r2 = Rule.Create("b").InRuleset("rs").SetContent("c").Since(System.DateTime.UtcNow).Build().Rule;
            r2.Priority = 2;

            var op = new ReOrganizePrioritiesManagementOperation(1);

            // Act
            var result = await op.ApplyAsync([r1, r2]);

            // Assert
            result.ElementAt(0).Priority.Should().Be(2);
            result.ElementAt(1).Priority.Should().Be(3);
        }

        [Fact]
        public async Task ApplyAsync_WithUpdatedRule_AdjustsBasedOnExisting()
        {
            // Arrange
            var r1 = Rule.Create("a").InRuleset("rs").SetContent("c").Since(System.DateTime.UtcNow).Build().Rule;
            r1.Priority = 1;
            var r2 = Rule.Create("b").InRuleset("rs").SetContent("c").Since(System.DateTime.UtcNow).Build().Rule;
            r2.Priority = 2;

            var updated = Rule.Create("a").InRuleset("rs").SetContent("c").Since(System.DateTime.UtcNow).Build().Rule;
            updated.Priority = 5; // greater than existent (1)

            var op = new ReOrganizePrioritiesManagementOperation(updated);

            // Act
            var result = await op.ApplyAsync([r1, r2]);

            // Assert
            // updated.Priority > existent.Rule.Priority -> priorityMoveFactor = -1 so decrement all
            result.ElementAt(0).Priority.Should().Be(0);
            result.ElementAt(1).Priority.Should().Be(1);
        }
    }
}
