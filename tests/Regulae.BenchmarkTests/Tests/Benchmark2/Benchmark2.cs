namespace Regulae.BenchmarkTests.Tests.Benchmark2
{
    using System.Threading.Tasks;
    using BenchmarkDotNet.Attributes;
    using Regulae;
    using Regulae.BenchmarkTests.Tests;
    using Regulae.Extensions;
    using Regulae.Generic;
    using Regulae.IntegrationTests.Common.Scenarios;
    using Regulae.IntegrationTests.Common.Scenarios.Scenario7;

    [SkewnessColumn, KurtosisColumn]
    public class Benchmark2 : IBenchmark
    {
        private readonly Scenario7Data benchmarkData = new Scenario7Data();

        private readonly IDictionary<ConditionNames, object> conditions = new Dictionary<ConditionNames, object>
        {
            { ConditionNames.Artist, "Queen" },
            { ConditionNames.Lyrics, "Is this the real life?\nIs this just fantasy?\nCaught in a landside,\nNo escape from reality" },
            { ConditionNames.ReleaseYear, 1975 },
        };

        private readonly DateTime matchDate = DateTime.Parse("2022-11-01");
        private IRulesEngine<Rulesets, ConditionNames>? genericRulesEngine;

        [ParamsAllValues]
        public EvaluationStrategies EvaluationStrategy { get; set; }

        [Params("in-memory")]
        public string? Provider { get; set; }

        [Benchmark]
        public async Task RunAsync()
        {
            await this.genericRulesEngine!.MatchOneAsync(Rulesets.Songs, this.matchDate, this.conditions);
        }

        [GlobalSetup]
        public async Task SetUpAsync()
        {
            var rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .SetDataSourceForBenchmark(this.Provider!, nameof(Benchmark2))
                .Configure(options =>
                {
                    options.UseEvaluationStrategy(this.EvaluationStrategy);
                })
                .Build();

            await ScenarioLoader.LoadScenarioAsync(rulesEngine, this.benchmarkData);
            this.genericRulesEngine = rulesEngine.MakeGeneric<Rulesets, ConditionNames>();
        }

        [GlobalCleanup]
        public async Task TearDownAsync()
        {
            await Extensions.TearDownProviderAsync(this.Provider!, nameof(Benchmark2));
            this.genericRulesEngine = null;
        }
    }
}