namespace Regulae.Management.Operations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Regulae;
    using Regulae.Management;

    internal sealed class FilterPrioritiesRangeManagementOperation : IManagementOperation
    {
        private int? bottomPriorityThreshold;
        private int? topPriorityThreshold;
        private readonly Rule? updatedRule;

        public FilterPrioritiesRangeManagementOperation(int? topPriorityThreshold, int? bottomPriorityThreshold)
        {
            this.bottomPriorityThreshold = bottomPriorityThreshold;
            this.topPriorityThreshold = topPriorityThreshold;
            this.updatedRule = null;
        }

        public FilterPrioritiesRangeManagementOperation(Rule updatedRule)
        {
            this.bottomPriorityThreshold = null;
            this.topPriorityThreshold = null;
            this.updatedRule = updatedRule;
        }

        public ValueTask<IEnumerable<Rule>> ApplyAsync(IEnumerable<Rule> rules)
        {
            if (this.updatedRule is not null)
            {
                var existentRule = rules.First(r => string.Equals(r.Name, this.updatedRule.Name, StringComparison.Ordinal));
                if (this.updatedRule.Priority != existentRule.Priority)
                {
                    this.topPriorityThreshold = Math.Min(this.updatedRule.Priority, existentRule.Priority);
                    this.bottomPriorityThreshold = Math.Max(this.updatedRule.Priority, existentRule.Priority);
                }
            }

            var filteredRules = rules;
            if (this.topPriorityThreshold.HasValue)
            {
                filteredRules = filteredRules.Where(r => r.Priority >= this.topPriorityThreshold);
            }

            if (this.bottomPriorityThreshold.HasValue)
            {
                filteredRules = filteredRules.Where(r => r.Priority <= this.bottomPriorityThreshold);
            }

            return new ValueTask<IEnumerable<Rule>>(filteredRules);
        }
    }
}