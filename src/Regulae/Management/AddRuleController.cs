namespace Regulae.Management
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentValidation;
    using Regulae.Extensions;
    using Regulae.Source;
    using Regulae.Validation;

    internal sealed class AddRuleController : IAddRuleController
    {
        private readonly IRuleSanitizer ruleSanitizer;
        private readonly IRulesSource rulesSource;
        private readonly IValidatorProvider validatorProvider;

        public AddRuleController(IRuleSanitizer ruleSanitizer, IRulesSource rulesSource, IValidatorProvider validatorProvider)
        {
            this.ruleSanitizer = ruleSanitizer;
            this.rulesSource = rulesSource;
            this.validatorProvider = validatorProvider;
        }

        public async ValueTask<OperationResult> ValidateAddRuleAsync(Rule rule, RuleAddPriorityOption ruleAddPriorityOption)
        {
            ArgumentNullException.ThrowIfNull(rule);
            var errors = new List<OperationError>();

            var ruleSanitizeResult = await this.ruleSanitizer.SanitizeAsync(rule).ConfigureAwait(false);
            if (!ruleSanitizeResult.IsSuccess)
            {
                errors.AddRange(ruleSanitizeResult.Errors);
                return Operation.Failure(errors);
            }

            var ruleValidator = this.validatorProvider.GetValidatorFor<Rule>();
            var ruleAddPriorityOptionValidator = this.validatorProvider.GetValidatorFor<RuleAddPriorityOption>();
            var validationContext = new ValidationContext<Rule>(rule);
            validationContext.RootContextData["Mode"] = "Add";
            var ruleValidationResult = await ruleValidator.ValidateAsync(validationContext).ConfigureAwait(false);
            var ruleAddPriorityOptionValidationResult = await ruleAddPriorityOptionValidator.ValidateAsync(ruleAddPriorityOption).ConfigureAwait(false);

            if (!ruleValidationResult.IsValid || !ruleAddPriorityOptionValidationResult.IsValid)
            {
                errors.AddRange(ruleValidationResult.Errors.Select(e => OperationError.Create(e.ErrorCode, e.ErrorMessage)));
                errors.AddRange(ruleAddPriorityOptionValidationResult.Errors.Select(e => OperationError.Create(e.ErrorCode, e.ErrorMessage)));
                return Operation.Failure(errors);
            }

            return Operation.Success();
        }

        public async ValueTask<OperationResult> AddRuleAsync(Rule rule, RuleAddPriorityOption ruleAddPriorityOption)
        {
            switch (ruleAddPriorityOption.PriorityOption)
            {
                case PriorityOptions.AtSmallestNumber:
                    await this.AddRuleInternalAtSmallestNumberAsync(rule).ConfigureAwait(false);
                    break;

                case PriorityOptions.AtLargestNumber:

                    await this.AddRuleInternalAtHighestNumberAsync(rule).ConfigureAwait(false);
                    break;

                case PriorityOptions.AtNumber:
                    await this.AddRuleInternalAtPriorityNumberAsync(rule, ruleAddPriorityOption).ConfigureAwait(false);
                    break;

                case PriorityOptions.AtRuleName:
                    await this.AddRuleInternalAtRuleNameAsync(rule, ruleAddPriorityOption).ConfigureAwait(false);
                    break;

                default:
                    throw new NotSupportedException($"The placement option '{ruleAddPriorityOption.PriorityOption}' is not supported.");
            }

            return Operation.Success();
        }

        private async ValueTask AddRuleInternalAtHighestNumberAsync(Rule rule)
        {
            var getRulesFilteredArgs = new GetRulesFilteredArgs
            {
                Ruleset = rule.Ruleset,
            };
            var existentRules = await this.rulesSource.GetRulesFilteredAsync(getRulesFilteredArgs).ConfigureAwait(false);
            rule.Priority = existentRules.Count == 0 ? 1 : existentRules.Max(r => r.Priority) + 1;

            await ManagementOperations.Manage(rule.Ruleset)
                .UsingSource(this.rulesSource)
                .AddRule(rule)
                .ExecuteOperationsAsync().ConfigureAwait(false);
        }

        private async ValueTask AddRuleInternalAtPriorityNumberAsync(Rule rule, RuleAddPriorityOption ruleAddPriorityOption)
        {
            var getRulesFilteredArgs = new GetRulesFilteredArgs
            {
                Ruleset = rule.Ruleset,
            };
            var existentRules = await this.rulesSource.GetRulesFilteredAsync(getRulesFilteredArgs).ConfigureAwait(false);
            var priorityMin = existentRules.MinOrDefault(r => r.Priority);
            var priorityMax = existentRules.MaxOrDefault(r => r.Priority);

            var rulePriority = ruleAddPriorityOption.AtNumberOptionValue;
            rulePriority = Math.Min(rulePriority, priorityMax + 1);
            rulePriority = Math.Max(rulePriority, priorityMin);

            rule.Priority = rulePriority;

            await ManagementOperations.Manage(rule.Ruleset)
                .UsingSource(this.rulesSource)
                .FilterPriorityFromThresholdNumberToLargestNumber(rulePriority)
                .IncreasePriority()
                .UpdateRules()
                .AddRule(rule)
                .ExecuteOperationsAsync().ConfigureAwait(false);
        }

        private async ValueTask AddRuleInternalAtRuleNameAsync(Rule rule, RuleAddPriorityOption ruleAddPriorityOption)
        {
            var getRulesFilteredArgs = new GetRulesFilteredArgs
            {
                Ruleset = rule.Ruleset,
            };
            var existentRules = await this.rulesSource.GetRulesFilteredAsync(getRulesFilteredArgs).ConfigureAwait(false);
            var firstPriorityToIncrement = existentRules
                .First(r => string.Equals(r.Name, ruleAddPriorityOption.AtRuleNameOptionValue, StringComparison.OrdinalIgnoreCase))
                .Priority;
            rule.Priority = firstPriorityToIncrement;

            await ManagementOperations.Manage(rule.Ruleset)
                .UsingSource(this.rulesSource)
                .FilterPriorityFromThresholdNumberToLargestNumber(firstPriorityToIncrement)
                .IncreasePriority()
                .UpdateRules()
                .AddRule(rule)
                .ExecuteOperationsAsync().ConfigureAwait(false);
        }

        private ValueTask AddRuleInternalAtSmallestNumberAsync(Rule rule)
        {
            rule.Priority = 1;

            return ManagementOperations.Manage(rule.Ruleset)
                .UsingSource(this.rulesSource)
                .IncreasePriority()
                .UpdateRules()
                .AddRule(rule)
                .ExecuteOperationsAsync();
        }
    }
}
