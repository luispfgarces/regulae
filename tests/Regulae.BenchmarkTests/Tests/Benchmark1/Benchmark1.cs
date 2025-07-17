namespace Regulae.BenchmarkTests.Tests.Benchmark1
{
    using System.Threading.Tasks;
    using BenchmarkDotNet.Attributes;
    using Regulae;
    using Regulae.BenchmarkTests.Tests;
    using Regulae.Extensions;
    using Regulae.Generic;
    using Regulae.IntegrationTests.Common.Scenarios;
    using Regulae.IntegrationTests.Common.Scenarios.Scenario6;

    [SkewnessColumn, KurtosisColumn]
    public class Benchmark1 : IBenchmark
    {
        private readonly Scenario6Data benchmarkData = new Scenario6Data();

        private readonly IDictionary<ConditionNames, object> conditions = new Dictionary<ConditionNames, object>
        {
            { ConditionNames.StringCondition, "Let's benchmark this!" },
        };

        private readonly DateTime matchDate = DateTime.Parse("2022-10-01");
        private IRulesEngine<Rulesets, ConditionNames>? genericRulesEngine;

        [ParamsAllValues]
        public EvaluationStrategies EvaluationStrategy { get; set; }

        [Params("in-memory")]
        public string? Provider { get; set; }

        [Benchmark]
        public async Task RunAsync()
        {
            await this.genericRulesEngine!.MatchOneAsync(Rulesets.Sample1, this.matchDate, this.conditions);
        }

        [GlobalSetup]
        public async Task SetUpAsync()
        {
            var rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .SetDataSourceForBenchmark(this.Provider!, nameof(Benchmark1))
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
            await Extensions.TearDownProviderAsync(this.Provider!, nameof(Benchmark1));
            this.genericRulesEngine = null;
        }
    }
}