namespace Regulae.Builder.Validation
{
    using System;
    using FluentValidation;
    using FluentValidation.Results;
    using Regulae;
    using Regulae.Source;

    internal sealed class RuleValidator : AbstractValidator<Rule>
    {
        private readonly ComposedConditionNodeValidator composedConditionNodeValidator;
        private readonly IRulesSource? rulesSource;
        private readonly IRulesEngineOptions? rulesEngineOptions;
        private readonly ValueConditionNodeValidator valueConditionNodeValidator;

        private RuleValidator()
        {
            this.composedConditionNodeValidator = new ComposedConditionNodeValidator();
            this.rulesEngineOptions = null;
            this.rulesSource = null;
            this.valueConditionNodeValidator = new ValueConditionNodeValidator();

            this.RuleFor(r => r.Name).NotEmpty().WithErrorCode(Constants.ErrorCodes.R0001);
            this.RuleFor(r => r.Ruleset).NotEmpty().WithErrorCode(Constants.ErrorCodes.R0002);
            this.RuleFor(r => r.DateBegin).NotEmpty().WithErrorCode(Constants.ErrorCodes.R0003);
            this.RuleFor(r => r.DateEnd).GreaterThanOrEqualTo(r => r.DateBegin).When(r => r.DateEnd != null).WithErrorCode(Constants.ErrorCodes.R0004);
            this.RuleFor(r => r.ContentContainer).NotNull().WithErrorCode(Constants.ErrorCodes.R0005);
            this.RuleFor(r => r.RootCondition).Custom((cn, cc) => cn?.PerformValidation(
                new ConditionNodeValidationArgs<Rule>(this.composedConditionNodeValidator, cc, this.valueConditionNodeValidator)));
        }

        public RuleValidator(IRulesSource rulesSource, IRulesEngineOptions rulesEngineOptions)
            : this()
        {
            ArgumentNullException.ThrowIfNull(rulesEngineOptions);
            ArgumentNullException.ThrowIfNull(rulesSource);
            this.rulesEngineOptions = rulesEngineOptions;
            this.rulesSource = rulesSource;

            this.When((rule, context) => context.RootContextData.TryGetValue("Mode", out var mode) && string.Equals((string)mode, "Add", StringComparison.Ordinal), () =>
            {
                this.RuleFor(r => r.Name).CustomAsync(async (name, context, cancellation) =>
                {
                    var args = new GetRulesFilteredArgs
                    {
                        Name = name,
                        Ruleset = context.InstanceToValidate.Ruleset,
                    };
                    var existentRules = await this.rulesSource.GetRulesFilteredAsync(args).ConfigureAwait(false);
                    cancellation.ThrowIfCancellationRequested();
                    if (existentRules.Count != 0)
                    {
                        var validationFailure = new ValidationFailure(nameof(Rule.Name), $"A rule with name '{name}' already exists for " +
                            $"ruleset '{context.InstanceToValidate.Ruleset}'.")
                        {
                            AttemptedValue = name,
                            ErrorCode = Constants.ErrorCodes.R0007,
                        };
                        context.AddFailure(validationFailure);
                    }
                });
            });

            this.When((rule, context) => context.RootContextData.TryGetValue("Mode", out var mode) && string.Equals((string)mode, "Update", StringComparison.Ordinal), () =>
            {
                this.RuleFor(r => r.Name).CustomAsync(async (name, context, cancellation) =>
                {
                    var args = new GetRulesFilteredArgs
                    {
                        Name = name,
                        Ruleset = context.InstanceToValidate.Ruleset,
                    };
                    var existentRules = await this.rulesSource.GetRulesFilteredAsync(args).ConfigureAwait(false);
                    cancellation.ThrowIfCancellationRequested();
                    if (existentRules.Count == 0)
                    {
                        var validationFailure = new ValidationFailure(nameof(Rule.Name), $"A rule with name '{name}' does not exist for " +
                            $"ruleset '{context.InstanceToValidate.Ruleset}'.")
                        {
                            AttemptedValue = name,
                            ErrorCode = Constants.ErrorCodes.R0008,
                        };
                        context.AddFailure(validationFailure);
                    }
                });
            });

            this.RuleFor(r => r.Ruleset).CustomAsync(async (ruleset, context, cancellation) =>
            {
                var rulesets = await this.rulesSource.GetRulesetsAsync(new GetRulesetsArgs()).ConfigureAwait(false);
                cancellation.ThrowIfCancellationRequested();

                if (!rulesets.ContainsKey(ruleset))
                {
                    var validationFailure = new ValidationFailure(nameof(Rule.Ruleset), $"Specified ruleset '{ruleset}' does not exist. " +
                        $"Please create the ruleset first or set the rules engine option '{nameof(this.rulesEngineOptions.AutoCreateRulesets)}' to true.")
                    {
                        AttemptedValue = ruleset,
                        ErrorCode = Constants.ErrorCodes.R0006,
                    };
                    context.AddFailure(validationFailure);
                }
            });
        }

        public static RuleValidator Instance { get; } = new RuleValidator();
    }
}