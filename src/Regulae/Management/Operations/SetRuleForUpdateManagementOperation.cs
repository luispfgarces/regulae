namespace Regulae.Management.Operations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Regulae;
    using Regulae.Management;

    internal sealed class SetRuleForUpdateManagementOperation : IManagementOperation
    {
        private readonly Rule updatedRule;

        public SetRuleForUpdateManagementOperation(Rule updatedRule)
        {
            this.updatedRule = updatedRule;
        }

        public ValueTask<IEnumerable<Rule>> ApplyAsync(IEnumerable<Rule> rules)
        {
            var result = new List<Rule>(rules);

            result.RemoveAll(r => string.Equals(r.Name, this.updatedRule.Name, StringComparison.InvariantCultureIgnoreCase));
            result.Add(this.updatedRule);

            return new ValueTask<IEnumerable<Rule>>(result.AsEnumerable());
        }
    }
}