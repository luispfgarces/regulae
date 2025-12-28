namespace Regulae.Management.Operations
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Regulae;
    using Regulae.Management;
    using Regulae.Source;

    internal sealed class AddRuleManagementOperation : IManagementOperation
    {
        private readonly Rule rule;
        private readonly IRulesSource rulesSource;

        public AddRuleManagementOperation(IRulesSource rulesSource, Rule rule)
        {
            this.rulesSource = rulesSource;
            this.rule = rule;
        }

        public async ValueTask<IEnumerable<Rule>> ApplyAsync(IEnumerable<Rule> rules)
        {
            var addRuleArgs = new AddRuleArgs
            {
                Rule = this.rule,
            };

            await this.rulesSource.AddRuleAsync(addRuleArgs).ConfigureAwait(false);

            var rulesResult = new List<Rule>(rules) { this.rule };

            return rulesResult;
        }
    }
}