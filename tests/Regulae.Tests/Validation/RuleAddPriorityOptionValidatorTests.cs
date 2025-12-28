namespace Regulae.Tests.Validation
{
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using Regulae;
    using Regulae.Source;
    using Regulae.Validation;
    using Xunit;

    public class RuleAddPriorityOptionValidatorTests
    {
        [Fact]
        public async Task AtNumber_WithNonPositive_IsInvalid_R0023()
        {
            // Arrange
            var storage = Mock.Of<IRulesSource>();
            var validator = new RuleAddPriorityOptionValidator(storage);

            var option = RuleAddPriorityOption.AtNumber(0);

            // Act
            var result = await validator.ValidateAsync(option);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.ErrorCode == Constants.ErrorCodes.R0023);
        }

        [Fact]
        public async Task AtRuleName_WithEmptyName_IsInvalid_R0024()
        {
            // Arrange
            var storage = Mock.Of<IRulesSource>();
            var validator = new RuleAddPriorityOptionValidator(storage);

            var option = RuleAddPriorityOption.AtRuleName("");

            // Act
            var result = await validator.ValidateAsync(option);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.ErrorCode == Constants.ErrorCodes.R0024);
        }

        [Fact]
        public async Task AtRuleName_WithNonExistentRule_IsInvalid_R0025()
        {
            // Arrange
            var rs = new Mock<IRulesSource>();
            rs.Setup(x => x.GetRulesFilteredAsync(It.IsAny<GetRulesFilteredArgs>())).ReturnsAsync(new Rule[0]);

            var validator = new RuleAddPriorityOptionValidator(rs.Object);

            var option = RuleAddPriorityOption.AtRuleName("missing");

            // Act
            var result = await validator.ValidateAsync(option);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.ErrorCode == Constants.ErrorCodes.R0025);
        }

        [Fact]
        public async Task AtRuleName_WithExistentRule_IsValid()
        {
            // Arrange
            var rs = new Mock<IRulesSource>();
            var existent = Rule.Create("r1").InRuleset("rs").SetContent(new object()).Since(System.DateTime.UtcNow).Build().Rule;
            rs.Setup(x => x.GetRulesFilteredAsync(It.IsAny<GetRulesFilteredArgs>())).ReturnsAsync(new[] { existent });

            var validator = new RuleAddPriorityOptionValidator(rs.Object);

            var option = RuleAddPriorityOption.AtRuleName(existent.Name);

            // Act
            var result = await validator.ValidateAsync(option);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task AtLargest_And_AtSmallest_AreValid()
        {
            // Arrange
            var storage = Mock.Of<IRulesSource>();
            var validator = new RuleAddPriorityOptionValidator(storage);

            // Act
            var validationResult1 = await validator.ValidateAsync(RuleAddPriorityOption.AtLargestNumber);
            var validationResult2 = await validator.ValidateAsync(RuleAddPriorityOption.AtSmallestNumber);

            // Assert
            validationResult1.IsValid.Should().BeTrue();
            validationResult2.IsValid.Should().BeTrue();
        }
    }
}
