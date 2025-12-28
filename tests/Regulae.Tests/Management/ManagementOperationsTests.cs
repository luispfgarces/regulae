namespace Regulae.Tests.Management
{
    using System;
    using FluentAssertions;
    using Moq;
    using Regulae.Management;
    using Regulae.Source;
    using Xunit;

    public class ManagementOperationsTests
    {
        [Fact]
        public void Manage_ThrowsOnEmptyRuleset()
        {
            // Act
            Action act = () => ManagementOperations.Manage("");

            // Assert
            act.Should().Throw<ArgumentException>().Which.ParamName.Should().Be("ruleset");
        }

        [Fact]
        public void ManagementOperationsSelector_UsingSource_ReturnsController()
        {
            // Arrange
            var ruleset = "rs";
            var rulesSource = Mock.Of<IRulesSource>();

            // Act
            var controller = ManagementOperations.Manage(ruleset).UsingSource(rulesSource);

            // Assert
            controller.Should().NotBeNull();
        }
    }
}
