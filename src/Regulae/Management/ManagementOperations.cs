namespace Regulae.Management
{
    using System;
    using Regulae.Source;

    internal static class ManagementOperations
    {
        public static ManagementOperationsSelector Manage(string ruleset) => new(ruleset);

        internal sealed class ManagementOperationsSelector
        {
            private readonly string ruleset;

            public ManagementOperationsSelector(string ruleset)
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(ruleset);
                this.ruleset = ruleset;
            }

            public ManagementOperationsController UsingSource(IRulesSource rulesDataSource)
                => new ManagementOperationsController(rulesDataSource, this.ruleset);
        }
    }
}