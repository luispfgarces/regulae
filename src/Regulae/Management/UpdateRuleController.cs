namespace Regulae.Management
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentValidation;
    using Regulae.Source;
    using Regulae.Validation;

    internal class UpdateRuleController : IUpdateRuleController
    {
        private readonly IRulesSource rulesSource;
        private readonly IValidatorProvider validatorProvider;

        public UpdateRuleController(IRulesSource rulesSource, IValidatorProvider validatorProvider)
        {
            this.rulesSource = rulesSource ?? throw new ArgumentNullException(nameof(rulesSource));
            this.validatorProvider = validatorProvider ?? throw new ArgumentNullException(nameof(validatorProvider));
        }
        public async ValueTask<OperationResult> UpdateRuleAsync(Rule rule)
        {
            await ManagementOperations.Manage(rule.Ruleset)
                .UsingSource(this.rulesSource)
                .FilterPrioritiesRangeUsingUpdatedRule(rule)
                .ReOrganizePrioritiesUsingUpdatedRule(rule)
                .SetRuleForUpdate(rule)
                .UpdateRules()
                .ExecuteOperationsAsync()
                .ConfigureAwait(false);

            return Operation.Success();
        }

        public async ValueTask<OperationResult> ValidateUpdateRuleAsync(Rule rule)
        {
            var ruleValidator = this.validatorProvider.GetValidatorFor<Rule>();
            var validationContext = new ValidationContext<Rule>(rule);
            validationContext.RootContextData["Mode"] = "Update";
            var validationResult = await ruleValidator.ValidateAsync(validationContext).ConfigureAwait(false);

            if (!validationResult.IsValid)
            {
                return Operation.Failure([.. validationResult.Errors.Select(ve => OperationError.Create(ve.ErrorCode, ve.ErrorMessage))]);
            }

            return Operation.Success();
        }
    }
}
