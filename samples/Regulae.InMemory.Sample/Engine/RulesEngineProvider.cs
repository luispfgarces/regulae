namespace Regulae.InMemory.Sample.Engine
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Regulae;
    using Regulae.Providers.InMemory;

    internal class RulesEngineProvider
    {
        private readonly Lazy<Task<IRulesEngine>> lazyRulesEngine;

        public RulesEngineProvider(RulesBuilder rulesBuilder)
        {
            lazyRulesEngine = new Lazy<Task<IRulesEngine>>(async () =>
            {
                var rulesEngine = RulesEngineBuilder
                    .CreateRulesEngine()
                    .SetInMemoryDataSource()
                    .Configure(c => c.UseLargestNumberPriorityCriteria())
                    .Build();

                await rulesBuilder.BuildAsync(rulesEngine).ConfigureAwait(false);

                return rulesEngine;
            }, LazyThreadSafetyMode.ExecutionAndPublication);
        }

        public Task<IRulesEngine> GetRulesEngineAsync()
            => lazyRulesEngine.Value;
    }
}