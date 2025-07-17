namespace Regulae.Rql.IntegrationTests.Scenarios.Scenario8
{
    using System;
    using Regulae;
    using Regulae.IntegrationTests.Common.Scenarios;
    using Regulae.IntegrationTests.Common.Scenarios.Scenario8;
    using Regulae.Providers.InMemory;

    public class RulesEngineWithScenario8RulesFixture : IDisposable
    {
        public RulesEngineWithScenario8RulesFixture()
        {
            this.RulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .SetInMemoryDataSource()
                .Configure(c => c.UseCompiledEvaluationStrategy())
                .Build();

            var scenarioData = new Scenario8Data();

            ScenarioLoader.LoadScenarioAsync(this.RulesEngine, scenarioData).GetAwaiter().GetResult();
        }

        public IRulesEngine RulesEngine { get; private set; }

        public void Dispose()
        {
            if (this.RulesEngine != null)
            {
                this.RulesEngine = null!;
            }

            GC.SuppressFinalize(this);
        }
    }
}