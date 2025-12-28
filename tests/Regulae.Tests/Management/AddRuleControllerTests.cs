namespace Regulae.Tests.Management
{
    using System;
    using System.Collections.Generic;
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

    public class AddRuleControllerTests
    {
        [Fact]
        public async Task ValidateAddRuleAsync_Throws_WhenRuleIsNull()
        {
            // Arrange
            var sanitizer = Mock.Of<IRuleSanitizer>();
            var rulesSource = Mock.Of<IRulesSource>();
            var validatorProvider = Mock.Of<IValidatorProvider>();
            var sut = new AddRuleController(sanitizer, rulesSource, validatorProvider);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await sut.ValidateAddRuleAsync(null!, RuleAddPriorityOption.AtSmallestNumber));
        }

        [Fact]
        public async Task ValidateAddRuleAsync_ReturnsFailure_WhenSanitizerFails()
        {
            // Arrange
            var errors = new List<OperationError> { OperationError.Create("E1", "err") };
            var sanitizer = new Mock<IRuleSanitizer>();
            sanitizer.Setup(s => s.SanitizeAsync(It.IsAny<Rule>())).ReturnsAsync(Operation.Failure(errors));

            var rulesSource = Mock.Of<IRulesSource>();

            var ruleValidator = new Mock<IValidator<Rule>>();
            ruleValidator.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<Rule>>(), default)).ReturnsAsync(new ValidationResult());
            var optionValidator = new Mock<IValidator<RuleAddPriorityOption>>();
            optionValidator.Setup(v => v.ValidateAsync(It.IsAny<RuleAddPriorityOption>(), default)).ReturnsAsync(new ValidationResult());

            var validatorProvider = new Mock<IValidatorProvider>();
            validatorProvider.Setup(v => v.GetValidatorFor<Rule>()).Returns(ruleValidator.Object);
            validatorProvider.Setup(v => v.GetValidatorFor<RuleAddPriorityOption>()).Returns(optionValidator.Object);

            var sut = new AddRuleController(sanitizer.Object, rulesSource, validatorProvider.Object);

            var rule = Rule.Create("r").InRuleset("rs").SetContent("c").Since(DateTime.UtcNow).Build().Rule;

            // Act
            var result = await sut.ValidateAddRuleAsync(rule, RuleAddPriorityOption.AtSmallestNumber);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().BeEquivalentTo(errors);
        }

        [Fact]
        public async Task ValidateAddRuleAsync_ReturnsFailure_WhenRuleValidatorInvalid()
        {
            // Arrange
            var sanitizer = new Mock<IRuleSanitizer>();
            sanitizer.Setup(s => s.SanitizeAsync(It.IsAny<Rule>())).ReturnsAsync(Operation.Success());

            var rulesSource = Mock.Of<IRulesSource>();

            var failures = new List<ValidationFailure> { new("Prop", "msg") { ErrorCode = "C1" } };
            var ruleValidationResult = new ValidationResult(failures);
            var optionValidationResult = new ValidationResult();

            var ruleValidator = new Mock<IValidator<Rule>>();
            ruleValidator.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<Rule>>(), default)).ReturnsAsync(ruleValidationResult);
            var optionValidator = new Mock<IValidator<RuleAddPriorityOption>>();
            optionValidator.Setup(v => v.ValidateAsync(It.IsAny<RuleAddPriorityOption>(), default)).ReturnsAsync(optionValidationResult);

            var validatorProvider = new Mock<IValidatorProvider>();
            validatorProvider.Setup(v => v.GetValidatorFor<Rule>()).Returns(ruleValidator.Object);
            validatorProvider.Setup(v => v.GetValidatorFor<RuleAddPriorityOption>()).Returns(optionValidator.Object);

            var sut = new AddRuleController(sanitizer.Object, rulesSource, validatorProvider.Object);

            var rule = Rule.Create("r").InRuleset("rs").SetContent("c").Since(DateTime.UtcNow).Build().Rule;

            // Act
            var result = await sut.ValidateAddRuleAsync(rule, RuleAddPriorityOption.AtSmallestNumber);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().HaveCount(1);
            result.Errors[0].Code.Should().Be("C1");
        }

        [Fact]
        public async Task ValidateAddRuleAsync_ReturnsFailure_WhenOptionValidatorInvalid()
        {
            // Arrange
            var sanitizer = new Mock<IRuleSanitizer>();
            sanitizer.Setup(s => s.SanitizeAsync(It.IsAny<Rule>())).ReturnsAsync(Operation.Success());

            var rulesSource = Mock.Of<IRulesSource>();

            var failures = new List<ValidationFailure> { new("Prop", "msg") { ErrorCode = "C2" } };
            var ruleValidationResult = new ValidationResult();
            var optionValidationResult = new ValidationResult(failures);

            var ruleValidator = new Mock<IValidator<Rule>>();
            ruleValidator.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<Rule>>(), default)).ReturnsAsync(ruleValidationResult);
            var optionValidator = new Mock<IValidator<RuleAddPriorityOption>>();
            optionValidator.Setup(v => v.ValidateAsync(It.IsAny<RuleAddPriorityOption>(), default)).ReturnsAsync(optionValidationResult);

            var validatorProvider = new Mock<IValidatorProvider>();
            validatorProvider.Setup(v => v.GetValidatorFor<Rule>()).Returns(ruleValidator.Object);
            validatorProvider.Setup(v => v.GetValidatorFor<RuleAddPriorityOption>()).Returns(optionValidator.Object);

            var sut = new AddRuleController(sanitizer.Object, rulesSource, validatorProvider.Object);

            var rule = Rule.Create("r").InRuleset("rs").SetContent("c").Since(DateTime.UtcNow).Build().Rule;

            // Act
            var result = await sut.ValidateAddRuleAsync(rule, RuleAddPriorityOption.AtSmallestNumber);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().HaveCount(1);
            result.Errors[0].Code.Should().Be("C2");
        }

        [Fact]
        public async Task ValidateAddRuleAsync_ReturnsSuccess_WhenAllValid()
        {
            // Arrange
            var sanitizer = new Mock<IRuleSanitizer>();
            sanitizer.Setup(s => s.SanitizeAsync(It.IsAny<Rule>())).ReturnsAsync(Operation.Success());

            var rulesSource = Mock.Of<IRulesSource>();

            var ruleValidator = new Mock<IValidator<Rule>>();
            ruleValidator.Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<Rule>>(), default)).ReturnsAsync(new ValidationResult());
            var optionValidator = new Mock<IValidator<RuleAddPriorityOption>>();
            optionValidator.Setup(v => v.ValidateAsync(It.IsAny<RuleAddPriorityOption>(), default)).ReturnsAsync(new ValidationResult());

            var validatorProvider = new Mock<IValidatorProvider>();
            validatorProvider.Setup(v => v.GetValidatorFor<Rule>()).Returns(ruleValidator.Object);
            validatorProvider.Setup(v => v.GetValidatorFor<RuleAddPriorityOption>()).Returns(optionValidator.Object);

            var sut = new AddRuleController(sanitizer.Object, rulesSource, validatorProvider.Object);

            var rule = Rule.Create("r").InRuleset("rs").SetContent("c").Since(DateTime.UtcNow).Build().Rule;

            // Act
            var result = await sut.ValidateAddRuleAsync(rule, RuleAddPriorityOption.AtSmallestNumber);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task AddRuleAsync_AtSmallestNumber_SetsPriorityAndCallsUpdateAndAdd()
        {
            // Arrange
            var rulesSource = new Mock<IRulesSource>();
            var existing1 = Rule.Create("e1").InRuleset("rs").SetContent("c").Since(DateTime.UtcNow).Build().Rule; existing1.Priority = 1;
            var existing2 = Rule.Create("e2").InRuleset("rs").SetContent("c").Since(DateTime.UtcNow).Build().Rule; existing2.Priority = 2;

            rulesSource.Setup(x => x.GetRulesFilteredAsync(It.IsAny<GetRulesFilteredArgs>())).ReturnsAsync([existing1, existing2]);
            rulesSource.Setup(x => x.UpdateRuleAsync(It.IsAny<UpdateRuleArgs>())).Returns(new ValueTask()).Verifiable();
            rulesSource.Setup(x => x.AddRuleAsync(It.IsAny<AddRuleArgs>())).Returns(new ValueTask()).Verifiable();

            var sut = new AddRuleController(Mock.Of<IRuleSanitizer>(), rulesSource.Object, Mock.Of<IValidatorProvider>());

            var rule = Rule.Create("r").InRuleset("rs").SetContent("c").Since(DateTime.UtcNow).Build().Rule;

            // Act
            var result = await sut.AddRuleAsync(rule, RuleAddPriorityOption.AtSmallestNumber);

            // Assert
            result.IsSuccess.Should().BeTrue();
            rule.Priority.Should().Be(1);
            existing1.Priority.Should().Be(2);
            existing2.Priority.Should().Be(3);
            rulesSource.Verify(x => x.UpdateRuleAsync(It.IsAny<UpdateRuleArgs>()), Times.AtLeastOnce);
            rulesSource.Verify(x => x.AddRuleAsync(It.IsAny<AddRuleArgs>()), Times.Once);
        }

        [Fact]
        public async Task AddRuleAsync_AtLargestNumber_SetsPriorityToMaxPlusOne_WhenExistents()
        {
            // Arrange
            var rulesSource = new Mock<IRulesSource>();
            var existing1 = Rule.Create("e1").InRuleset("rs").SetContent("c").Since(DateTime.UtcNow).Build().Rule; existing1.Priority = 3;
            var existent = new[] { existing1 };
            rulesSource.Setup(x => x.GetRulesFilteredAsync(It.IsAny<GetRulesFilteredArgs>())).ReturnsAsync(existent);
            rulesSource.Setup(x => x.AddRuleAsync(It.IsAny<AddRuleArgs>())).Returns(new ValueTask()).Verifiable();

            var sut = new AddRuleController(Mock.Of<IRuleSanitizer>(), rulesSource.Object, Mock.Of<IValidatorProvider>());
            var rule = Rule.Create("r").InRuleset("rs").SetContent("c").Since(DateTime.UtcNow).Build().Rule;

            // Act
            var result = await sut.AddRuleAsync(rule, RuleAddPriorityOption.AtLargestNumber);

            // Assert
            result.IsSuccess.Should().BeTrue();
            existing1.Priority.Should().Be(3);
            rule.Priority.Should().Be(4);
            rulesSource.Verify(x => x.AddRuleAsync(It.IsAny<AddRuleArgs>()), Times.Once);
        }

        [Fact]
        public async Task AddRuleAsync_AtLargestNumber_SetsPriorityToOne_WhenNoExistents()
        {
            // Arrange
            var rulesSource = new Mock<IRulesSource>();
            rulesSource.Setup(x => x.GetRulesFilteredAsync(It.IsAny<GetRulesFilteredArgs>())).ReturnsAsync([]);
            rulesSource.Setup(x => x.AddRuleAsync(It.IsAny<AddRuleArgs>())).Returns(new ValueTask()).Verifiable();

            var sut = new AddRuleController(Mock.Of<IRuleSanitizer>(), rulesSource.Object, Mock.Of<IValidatorProvider>());
            var rule = Rule.Create("r").InRuleset("rs").SetContent("c").Since(DateTime.UtcNow).Build().Rule;

            // Act
            var result = await sut.AddRuleAsync(rule, RuleAddPriorityOption.AtLargestNumber);

            // Assert
            result.IsSuccess.Should().BeTrue();
            rule.Priority.Should().Be(1);
            rulesSource.Verify(x => x.AddRuleAsync(It.IsAny<AddRuleArgs>()), Times.Once);
        }

        [Fact]
        public async Task AddRuleAsync_AtNumber_BoundsPriorityAndUpdatesAndAdds()
        {
            // Arrange
            var rulesSource = new Mock<IRulesSource>();
            var existing1 = Rule.Create("e1").InRuleset("rs").SetContent("c").Since(DateTime.UtcNow).Build().Rule; existing1.Priority = 1;
            var existing2 = Rule.Create("e2").InRuleset("rs").SetContent("c").Since(DateTime.UtcNow).Build().Rule; existing2.Priority = 5;
            rulesSource.Setup(x => x.GetRulesFilteredAsync(It.IsAny<GetRulesFilteredArgs>())).ReturnsAsync([existing1, existing2]);
            rulesSource.Setup(x => x.AddRuleAsync(It.IsAny<AddRuleArgs>())).Returns(new ValueTask()).Verifiable();

            var sut = new AddRuleController(Mock.Of<IRuleSanitizer>(), rulesSource.Object, Mock.Of<IValidatorProvider>());
            var rule = Rule.Create("r").InRuleset("rs").SetContent("c").Since(DateTime.UtcNow).Build().Rule;

            var option = RuleAddPriorityOption.AtNumber(10);

            // Act
            var result = await sut.AddRuleAsync(rule, option);

            // Assert
            result.IsSuccess.Should().BeTrue();
            // priorityMax + 1 = 6, so bounded to 6
            existing1.Priority.Should().Be(1);
            existing2.Priority.Should().Be(5);
            rule.Priority.Should().Be(6);
            rulesSource.Verify(x => x.AddRuleAsync(It.IsAny<AddRuleArgs>()), Times.Once);
        }

        [Fact]
        public async Task AddRuleAsync_AtRuleName_SetsPriorityToReferencedRulePriority_AndUpdatesAndAdds()
        {
            // Arrange
            var rulesSource = new Mock<IRulesSource>();
            var target = Rule.Create("target").InRuleset("rs").SetContent("c").Since(DateTime.UtcNow).Build().Rule; target.Priority = 7;
            var other = Rule.Create("other").InRuleset("rs").SetContent("c").Since(DateTime.UtcNow).Build().Rule; other.Priority = 3;

            rulesSource.Setup(x => x.GetRulesFilteredAsync(It.IsAny<GetRulesFilteredArgs>())).ReturnsAsync([target, other]);
            rulesSource.Setup(x => x.UpdateRuleAsync(It.IsAny<UpdateRuleArgs>())).Returns(new ValueTask()).Verifiable();
            rulesSource.Setup(x => x.AddRuleAsync(It.IsAny<AddRuleArgs>())).Returns(new ValueTask()).Verifiable();

            var sut = new AddRuleController(Mock.Of<IRuleSanitizer>(), rulesSource.Object, Mock.Of<IValidatorProvider>());
            var rule = Rule.Create("r").InRuleset("rs").SetContent("c").Since(DateTime.UtcNow).Build().Rule;

            var option = RuleAddPriorityOption.AtRuleName("target");

            // Act
            var result = await sut.AddRuleAsync(rule, option);

            // Assert
            result.IsSuccess.Should().BeTrue();
            other.Priority.Should().Be(3);
            rule.Priority.Should().Be(7);
            target.Priority.Should().Be(8);
            rulesSource.Verify(x => x.UpdateRuleAsync(It.IsAny<UpdateRuleArgs>()), Times.AtLeastOnce);
            rulesSource.Verify(x => x.AddRuleAsync(It.IsAny<AddRuleArgs>()), Times.Once);
        }
    }
}
