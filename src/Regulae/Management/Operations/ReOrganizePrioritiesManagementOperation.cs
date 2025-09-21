namespace Regulae.Management.Operations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Regulae;
    using Regulae.Management;

    internal sealed class ReOrganizePrioritiesManagementOperation : IManagementOperation
    {
        private int priorityMoveFactor;
        private readonly Rule? updatedRule;

        public ReOrganizePrioritiesManagementOperation(int priorityMoveFactor)
        {
            this.priorityMoveFactor = priorityMoveFactor;
            this.updatedRule = null;
        }

        public ReOrganizePrioritiesManagementOperation(Rule updatedRule)
        {
            this.priorityMoveFactor = 0;
            this.updatedRule = updatedRule;
        }

        public ValueTask<IEnumerable<Rule>> ApplyAsync(IEnumerable<Rule> rules)
        {
            if (this.updatedRule is not null)
            {
                var existentRule = rules.First(r => string.Equals(r.Name, this.updatedRule.Name, StringComparison.Ordinal));
                this.priorityMoveFactor = this.updatedRule.Priority switch
                {
                    int p when p > existentRule.Priority => -1,
                    int p when p < existentRule.Priority => 1,
                    _ => 0,
                };
            }

            if (this.priorityMoveFactor != 0)
            {
                foreach (var rule in rules)
                {
                    rule.Priority += this.priorityMoveFactor;
                }
            }

            return new ValueTask<IEnumerable<Rule>>(rules);
        }
    }
}