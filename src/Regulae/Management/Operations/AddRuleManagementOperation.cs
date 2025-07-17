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
        private readonly IRulesSource rulesDataSource;

        public AddRuleManagementOperation(IRulesSource rulesDataSource, Rule rule)
        {
            this.rulesDataSource = rulesDataSource;
            this.rule = rule;
        }

        public async ValueTask<IEnumerable<Rule>> ApplyAsync(IEnumerable<Rule> rules)
        {
            var addRuleArgs = new AddRuleArgs
            {
                Rule = this.rule,
            };

            await this.rulesDataSource.AddRuleAsync(addRuleArgs).ConfigureAwait(false);

            var rulesResult = new List<Rule>(rules) { this.rule };

            return rulesResult;
        }
    }
}