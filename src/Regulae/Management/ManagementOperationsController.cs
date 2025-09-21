namespace Regulae.Management
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Regulae;
    using Regulae.Management.Operations;
    using Regulae.Source;

    internal sealed class ManagementOperationsController
    {
        private readonly List<IManagementOperation> managementOperations;
        private readonly string ruleset;
        private readonly IRulesSource rulesSource;

        public ManagementOperationsController(IRulesSource rulesSource, string ruleset)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(ruleset);
            ArgumentNullException.ThrowIfNull(rulesSource);
            this.managementOperations = [];
            this.ruleset = ruleset;
            this.rulesSource = rulesSource;
        }

        public ManagementOperationsController AddRule(Rule rule)
            => this.AddOperation(new AddRuleManagementOperation(this.rulesSource, rule));

        public async ValueTask ExecuteOperationsAsync()
        {
            var getRulesFilteredArgs = new GetRulesFilteredArgs
            {
                Ruleset = this.ruleset,
            };
            var rulesIntermediateResult = (IEnumerable<Rule>)await this.rulesSource.GetRulesFilteredAsync(getRulesFilteredArgs).ConfigureAwait(false);

            foreach (var managementOperation in this.managementOperations)
            {
                rulesIntermediateResult = await managementOperation.ApplyAsync(rulesIntermediateResult).ConfigureAwait(false);
            }
        }

        public ManagementOperationsController FilterPriorityFromThresholdNumberToLargestNumber(int thresholdPriority)
            => this.AddOperation(new FilterPrioritiesRangeManagementOperation(topPriorityThreshold: thresholdPriority, bottomPriorityThreshold: null));

        public ManagementOperationsController FilterPrioritiesRangeUsingUpdatedRule(Rule updatedRule)
            => this.AddOperation(new FilterPrioritiesRangeManagementOperation(updatedRule));

        public ManagementOperationsController IncreasePriority()
            => this.AddOperation(new ReOrganizePrioritiesManagementOperation(1));

        public ManagementOperationsController ReOrganizePrioritiesUsingUpdatedRule(Rule updatedRule)
            => this.AddOperation(new ReOrganizePrioritiesManagementOperation(updatedRule));

        public ManagementOperationsController SetRuleForUpdate(Rule updatedRule)
            => this.AddOperation(new SetRuleForUpdateManagementOperation(updatedRule));

        public ManagementOperationsController UpdateRules()
            => this.AddOperation(new UpdateRulesManagementOperation(this.rulesSource));

        private ManagementOperationsController AddOperation(IManagementOperation managementOperation)
        {
            this.managementOperations.Add(managementOperation);

            return this;
        }
    }
}