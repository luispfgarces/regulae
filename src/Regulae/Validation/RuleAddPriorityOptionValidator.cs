namespace Regulae.Validation
{
    using System;
    using System.Linq;
    using FluentValidation;
    using FluentValidation.Results;
    using Regulae.Source;

    internal class RuleAddPriorityOptionValidator : AbstractValidator<RuleAddPriorityOption>
    {
        private readonly IRulesSource rulesSource;

        public RuleAddPriorityOptionValidator(IRulesSource rulesSource)
        {
            ArgumentNullException.ThrowIfNull(rulesSource);
            this.rulesSource = rulesSource;

            this.RuleFor(x => x.PriorityOption).IsInEnum().WithErrorCode(Constants.ErrorCodes.R0022);
            this.When(x => x.PriorityOption == PriorityOptions.AtNumber, () =>
            {
                this.RuleFor(x => x.AtNumberOptionValue).GreaterThan(0).WithErrorCode(Constants.ErrorCodes.R0023);
            });
            this.When(x => x.PriorityOption == PriorityOptions.AtRuleName, () =>
            {
                this.RuleFor(x => x.AtRuleNameOptionValue).NotEmpty().WithErrorCode(Constants.ErrorCodes.R0024);
                this.RuleFor(x => x).CustomAsync(async (option, context, cancellation) =>
                {
                    var rulesFilterArgs = new GetRulesFilteredArgs
                    {
                        Name = option.AtRuleNameOptionValue,
                    };
                    var rules = await this.rulesSource.GetRulesFilteredAsync(rulesFilterArgs).ConfigureAwait(false);
                    cancellation.ThrowIfCancellationRequested();
                    if (!string.IsNullOrWhiteSpace(option.AtRuleNameOptionValue) &&
                        !rules.Any(r => r.Name.Equals(option.AtRuleNameOptionValue, StringComparison.Ordinal)))
                    {
                        var validationFailure = new ValidationFailure(
                            nameof(RuleAddPriorityOption.AtRuleNameOptionValue),
                            $"Specified rule name '{option.AtRuleNameOptionValue}' does not exist. Please specify an existent rule name.")
                        {
                            AttemptedValue = option.AtRuleNameOptionValue,
                            ErrorCode = Constants.ErrorCodes.R0025,
                        };
                        context.AddFailure(validationFailure);
                    }
                });
            });
        }
    }
}
