namespace Regulae.Tests.Management
{
    using System;
    using System.Threading.Tasks;
    using Moq;
    using Regulae;
    using Regulae.Management;
    using Regulae.Source;
    using Xunit;

    public class ManagementOperationsControllerTests
    {
        [Fact]
        public async Task AddRule_AddsAndExecutes()
        {
            // Arrange
            var ruleset = "rs";
            var rule = Rule.Create("r1").InRuleset(ruleset).SetContent("c").Since(DateTime.UtcNow).Build().Rule;
            var rulesSourceMock = new Mock<IRulesSource>();

            rulesSourceMock.Setup(x => x.GetRulesFilteredAsync(It.IsAny<GetRulesFilteredArgs>()))
                .ReturnsAsync([]);

            rulesSourceMock.Setup(x => x.AddRuleAsync(It.IsAny<AddRuleArgs>()))
                .Returns(new ValueTask())
                .Verifiable();

            var controller = new ManagementOperationsController(rulesSourceMock.Object, ruleset);

            // Act
            controller.AddRule(rule);
            await controller.ExecuteOperationsAsync();

            // Assert
            rulesSourceMock.Verify(x => x.GetRulesFilteredAsync(It.Is<GetRulesFilteredArgs>(g => g.Ruleset == ruleset)), Times.Once);
            rulesSourceMock.Verify(x => x.AddRuleAsync(It.Is<AddRuleArgs>(a => ReferenceEquals(a.Rule, rule))), Times.Once);
        }
    }
}
