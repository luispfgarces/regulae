namespace Regulae.Management.Operations
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Regulae;
    using Regulae.Management;
    using Regulae.Source;

    internal sealed class UpdateRulesManagementOperation : IManagementOperation
    {
        private readonly IRulesSource rulesSource;

        public UpdateRulesManagementOperation(IRulesSource rulesSource)
        {
            this.rulesSource = rulesSource;
        }

        public async Task<IEnumerable<Rule>> ApplyAsync(IEnumerable<Rule> rules)
        {
            foreach (var existentRule in rules)
            {
                var updateRuleArgs = new UpdateRuleArgs
                {
                    Rule = existentRule,
                };

                await this.rulesSource.UpdateRuleAsync(updateRuleArgs).ConfigureAwait(false);
            }

            return rules;
        }
    }
}