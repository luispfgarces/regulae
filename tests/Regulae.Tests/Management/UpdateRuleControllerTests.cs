namespace Regulae.Tests.Management
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using FluentValidation;
    using FluentValidation.Results;
    using Moq;
    using Regulae;
    using Regulae.Management;
    using Regulae.Source;
    using Regulae.Validation;
    using Xunit;

    public class UpdateRuleControllerTests
    {
        [Fact]
        public async Task ValidateUpdateRuleAsync_ReturnsFailure_WhenInvalid()
        {
            // Arrange
            var rulesSource = Mock.Of<IRulesSource>();
            var validatorProvider = new Mock<IValidatorProvider>();

            var failures = new[] { new ValidationFailure("Prop", "msg") { ErrorCode = "E1" } };
            var validationResult = new ValidationResult(failures);
            var ruleValidator = new Mock<IValidator<Rule>>();
            ruleValidator.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<Rule>>(), default))
                .ReturnsAsync(validationResult);

            validatorProvider.Setup(v => v.GetValidatorFor<Rule>())
                .Returns(ruleValidator.Object);

            var sut = new UpdateRuleController(rulesSource, validatorProvider.Object);

            var rule = Rule.Create("r").InRuleset("rs").SetContent("c").Since(DateTime.UtcNow).Build().Rule;

            // Act
            var result = await sut.ValidateUpdateRuleAsync(rule);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().HaveCount(1);
            result.Errors.First().Code.Should().Be("E1");
        }

        [Fact]
        public async Task ValidateUpdateRuleAsync_ReturnsSuccess_WhenValid()
        {
            // Arrange
            var rulesSource = Mock.Of<IRulesSource>();
            var validatorProvider = new Mock<IValidatorProvider>();

            var validationResult = new ValidationResult();
            var ruleValidator = new Mock<IValidator<Rule>>();
            ruleValidator.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<Rule>>(), default))
                .ReturnsAsync(validationResult);

            validatorProvider.Setup(v => v.GetValidatorFor<Rule>())
                .Returns(ruleValidator.Object);

            var sut = new UpdateRuleController(rulesSource, validatorProvider.Object);

            var rule = Rule.Create("r").InRuleset("rs").SetContent("c").Since(DateTime.UtcNow).Build().Rule;

            // Act
            var result = await sut.ValidateUpdateRuleAsync(rule);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public async Task UpdateRuleAsync_ExecutesManagementOperations()
        {
            // Arrange
            var rule = Rule.Create("r").InRuleset("rs").SetContent("c").Since(DateTime.UtcNow).Build().Rule;
            var rulesSource = new Mock<IRulesSource>();
            rulesSource.Setup(x => x.GetRulesFilteredAsync(It.IsAny<GetRulesFilteredArgs>()))
                .ReturnsAsync([rule]);
            rulesSource.Setup(x => x.UpdateRuleAsync(It.IsAny<UpdateRuleArgs>()))
                .Returns(new ValueTask())
                .Verifiable();

            var validatorProvider = Mock.Of<IValidatorProvider>();

            var sut = new UpdateRuleController(rulesSource.Object, validatorProvider);

            var updated = Rule.Create("r").InRuleset("rs").SetContent("c").Since(DateTime.UtcNow).Until(DateTime.UtcNow.AddHours(2)).Build().Rule;

            // Act
            var result = await sut.UpdateRuleAsync(updated);

            // Assert
            result.IsSuccess.Should().BeTrue();
            rulesSource.Verify(x => x.UpdateRuleAsync(It.IsAny<UpdateRuleArgs>()), Times.Once);
        }
    }
}
