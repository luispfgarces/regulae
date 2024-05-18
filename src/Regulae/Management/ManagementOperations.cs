namespace Regulae.Management
{
    using System.Collections.Generic;
    using Regulae;
    using Regulae.Source;

    internal static class ManagementOperations
    {
        public static ManagementOperationsSelector Manage(IEnumerable<Rule> rules) => new(rules);

        internal sealed class ManagementOperationsSelector
        {
            private readonly IEnumerable<Rule> rules;

            public ManagementOperationsSelector(IEnumerable<Rule> rules)
            {
                this.rules = rules;
            }

            public ManagementOperationsController UsingSource(IRulesSource rulesDataSource)
                => new ManagementOperationsController(rulesDataSource, this.rules);
        }
    }
}