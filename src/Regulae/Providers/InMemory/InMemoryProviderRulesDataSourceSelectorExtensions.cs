namespace Regulae.Providers.InMemory
{
    using System;
    using Regulae.Builder;

    /// <summary>
    /// Rules data source selector extensions from in-memory provider.
    /// </summary>
    public static class InMemoryProviderRulesDataSourceSelectorExtensions
    {
        /// <summary>
        /// Sets the rules engine data source from a in-memory data source.
        /// </summary>
        /// <param name="rulesDataSourceSelector">The rules data source selector.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">rulesDataSourceSelector</exception>
        public static IConfiguredRulesEngineBuilder SetInMemoryDataSource(
            this IRulesDataSourceSelector rulesDataSourceSelector)
            => rulesDataSourceSelector.SetInMemoryDataSource(new InMemoryRulesStorage());

        /// <summary>
        /// Sets the rules engine data source from a in-memory data source.
        /// </summary>
        /// <param name="rulesDataSourceSelector">The rules data source selector.</param>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">rulesDataSourceSelector or serviceProvider</exception>
        public static IConfiguredRulesEngineBuilder SetInMemoryDataSource(
            this IRulesDataSourceSelector rulesDataSourceSelector,
            IServiceProvider serviceProvider)
        {
            ArgumentNullException.ThrowIfNull(serviceProvider);

            var inMemoryRulesStorage = (IInMemoryRulesStorage?)serviceProvider
                .GetService(typeof(IInMemoryRulesStorage));

            if (inMemoryRulesStorage is null)
            {
                throw new InvalidOperationException(
                    $"The service provider is not configured for in-memory rules data source. " +
                    $"Please make sure you call {nameof(ServiceCollectionExtensions.AddInMemoryRulesDataSource)}(...) when building the service collection.");
            }

            return rulesDataSourceSelector.SetInMemoryDataSource(inMemoryRulesStorage);
        }

        private static IConfiguredRulesEngineBuilder SetInMemoryDataSource(
            this IRulesDataSourceSelector rulesDataSourceSelector,
            IInMemoryRulesStorage inMemoryRulesStorage)
        {
            ArgumentNullException.ThrowIfNull(rulesDataSourceSelector);

            var ruleFactory = new RuleFactory();
            var inMemoryProviderRulesDataSource
                = new InMemoryProviderRulesDataSource(inMemoryRulesStorage, ruleFactory);

            return rulesDataSourceSelector.SetDataSource(inMemoryProviderRulesDataSource);
        }
    }
}