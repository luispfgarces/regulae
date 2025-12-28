namespace Regulae.Tests.Management.Operations
{
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using Regulae;
    using Regulae.Management.Operations;
    using Regulae.Source;
    using Xunit;

    public class UpdateRulesManagementOperationTests
    {
        [Fact]
        public async Task ApplyAsync_CallsUpdateForEachRule()
        {
            // Arrange
            var rulesSource = new Mock<IRulesSource>();
            var r1 = Rule.Create("r1").InRuleset("rs").SetContent("c").Since(System.DateTime.UtcNow).Build().Rule;
            var r2 = Rule.Create("r2").InRuleset("rs").SetContent("c").Since(System.DateTime.UtcNow).Build().Rule;

            rulesSource.Setup(x => x.UpdateRuleAsync(It.IsAny<UpdateRuleArgs>()))
                .Returns(new ValueTask())
                .Verifiable();

            var op = new UpdateRulesManagementOperation(rulesSource.Object);

            // Act
            var result = await op.ApplyAsync([r1, r2]);

            // Assert
            rulesSource.Verify(x => x.UpdateRuleAsync(It.IsAny<UpdateRuleArgs>()), Times.Exactly(2));
            result.Should().HaveCount(2);
        }
    }
}
