namespace Regulae.Tests.Validation
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Moq;
    using Regulae;
    using Regulae.Builder.Validation;
    using Regulae.Source;
    using Xunit;

    public class RuleValidatorTests
    {
        [Fact]
        public void Instance_NotNull()
        {
            RuleValidator.Instance.Should().NotBeNull();
        }

        [Fact]
        public async Task Validate_AddMode_NameAlreadyExists_R0007()
        {
            // Arrange
            var rulesSource = new Mock<IRulesSource>();
            var existing = Rule.Create("r").InRuleset("rs").SetContent(new object()).Since(DateTime.UtcNow).Build().Rule;
            rulesSource.Setup(x => x.GetRulesFilteredAsync(It.IsAny<GetRulesFilteredArgs>()))
                .ReturnsAsync([existing]);
            rulesSource.Setup(x => x.GetRulesetsAsync(It.IsAny<GetRulesetsArgs>()))
                .ReturnsAsync(new Dictionary<string, Ruleset> { { "rs", new Ruleset("rs", DateTime.UtcNow) } });

            var options = RulesEngineOptions.NewWithDefaults();
            var validator = new RuleValidator(rulesSource.Object, options);

            var rule = new Rule("r", "rs", DateTime.UtcNow, null, new ObjectContentContainer(new object()));

            var context = new FluentValidation.ValidationContext<Rule>(rule);
            context.RootContextData["Mode"] = "Add";

            // Act
            var result = await validator.ValidateAsync(context);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.ErrorCode == Constants.ErrorCodes.R0007);
        }

        [Fact]
        public async Task Validate_UpdateMode_NameDoesNotExist_R0008()
        {
            // Arrange
            var rulesSource = new Mock<IRulesSource>();
            rulesSource.Setup(x => x.GetRulesFilteredAsync(It.IsAny<GetRulesFilteredArgs>()))
                .ReturnsAsync([]);
            rulesSource.Setup(x => x.GetRulesetsAsync(It.IsAny<GetRulesetsArgs>()))
                .ReturnsAsync(new Dictionary<string, Ruleset> { { "rs", new Ruleset("rs", DateTime.UtcNow) } });

            var options = RulesEngineOptions.NewWithDefaults();
            var validator = new RuleValidator(rulesSource.Object, options);

            var rule = new Rule("r", "rs", DateTime.UtcNow, null, new ObjectContentContainer(new object()));

            var context = new FluentValidation.ValidationContext<Rule>(rule);
            context.RootContextData["Mode"] = "Update";

            // Act
            var result = await validator.ValidateAsync(context);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.ErrorCode == Constants.ErrorCodes.R0008);
        }

        [Fact]
        public async Task Validate_RulesetDoesNotExist_R0006()
        {
            // Arrange
            var rulesSource = new Mock<IRulesSource>();
            rulesSource.Setup(x => x.GetRulesetsAsync(It.IsAny<GetRulesetsArgs>())).ReturnsAsync(new Dictionary<string, Ruleset>());

            var options = RulesEngineOptions.NewWithDefaults();
            var validator = new RuleValidator(rulesSource.Object, options);

            var rule = new Rule("r", "missing", DateTime.UtcNow, null, new ObjectContentContainer(new object()));

            // Act
            var result = await validator.ValidateAsync(rule);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.ErrorCode == Constants.ErrorCodes.R0006);
        }

        [Fact]
        public void Validate_DefaultValidatorRules_ValidatesSimpleRule()
        {
            // Arrange
            var rule = new Rule("r", "rs", DateTime.UtcNow, null, new ObjectContentContainer(new object()));

            // Act
            var validationResult = RuleValidator.Instance.Validate(rule);

            // Assert
            validationResult.IsValid.Should().BeTrue();
        }
    }
}
