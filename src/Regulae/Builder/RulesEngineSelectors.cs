namespace Regulae.Builder
{
    using System;
    using Regulae;

    internal static class RulesEngineSelectors
    {
        internal sealed class RulesDataSourceSelector : IRulesDataSourceSelector
        {
            public IConfiguredRulesEngineBuilder SetDataSource(IRulesDataSource rulesDataSource)
            {
                if (rulesDataSource == null)
                {
                    throw new ArgumentNullException(nameof(rulesDataSource));
                }

                return new ConfiguredRulesEngineBuilder(rulesDataSource);
            }
        }
    }
}