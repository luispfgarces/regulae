namespace Regulae.Tests.Management.Operations
{
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using Regulae.Management.Operations;
    using Regulae.Source;
    using Xunit;

    public class AddRuleManagementOperationTests
    {
        [Fact]
        public async Task ApplyAsync_AppendsRuleAndCallsDataSource()
        {
            // Arrange
            var rulesSource = new Mock<IRulesSource>();
            var rule = Rule.Create("r2").InRuleset("rs").SetContent("c").Since(System.DateTime.UtcNow).Build().Rule;
            var op = new AddRuleManagementOperation(rulesSource.Object, rule);

            var initial = new[] { Rule.Create("a").InRuleset("rs").SetContent("c").Since(System.DateTime.UtcNow).Build().Rule };

            // Act
            var result = await op.ApplyAsync(initial);

            // Assert
            result.Should().Contain(rule);
            rulesSource.Verify(x => x.AddRuleAsync(It.IsAny<AddRuleArgs>()), Times.Once);
        }
    }
}
