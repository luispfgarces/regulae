namespace Regulae.BenchmarkTests.Tests.Benchmark3
{
    using System.Threading.Tasks;
    using BenchmarkDotNet.Attributes;
    using Regulae;
    using Regulae.BenchmarkTests.Tests;
    using Regulae.Extensions;
    using Regulae.Generic;
    using Regulae.IntegrationTests.Common.Scenarios;
    using Regulae.IntegrationTests.Common.Scenarios.Scenario8;

    [SkewnessColumn, KurtosisColumn]
    public class Benchmark3 : IBenchmark
    {
        private readonly Scenario8Data benchmarkData = new Scenario8Data();

        private readonly IDictionary<PokerConditions, object> conditions = new Dictionary<PokerConditions, object>
        {
            { PokerConditions.NumberOfKings, 1 },
            { PokerConditions.NumberOfQueens, 1 },
            { PokerConditions.NumberOfJacks, 1 },
            { PokerConditions.NumberOfTens, 1 },
            { PokerConditions.NumberOfNines, 1 },
            { PokerConditions.KingOfClubs, true },
            { PokerConditions.QueenOfDiamonds, true },
            { PokerConditions.JackOfClubs, true },
            { PokerConditions.TenOfHearts, true },
            { PokerConditions.NineOfSpades, true },
        };

        private readonly DateTime matchDate = DateTime.Parse("2022-12-01");
        private IRulesEngine<PokerRulesets, PokerConditions>? genericRulesEngine;

        [ParamsAllValues]
        public EvaluationStrategies EvaluationStrategy { get; set; }

        [Params("in-memory")]
        public string? Provider { get; set; }

        [Benchmark]
        public async Task RunAsync()
        {
            await this.genericRulesEngine!.MatchOneAsync(PokerRulesets.TexasHoldemPokerSingleCombinations, this.matchDate, this.conditions);
        }

        [GlobalSetup]
        public async Task SetUpAsync()
        {
            var rulesEngine = RulesEngineBuilder.CreateRulesEngine()
                .SetDataSourceForBenchmark(this.Provider!, nameof(Benchmark3))
                .Configure(options =>
                {
                    options.UseEvaluationStrategy(this.EvaluationStrategy);
                })
                .Build();

            await ScenarioLoader.LoadScenarioAsync(rulesEngine, this.benchmarkData);
            this.genericRulesEngine = rulesEngine.MakeGeneric<PokerRulesets, PokerConditions>();
        }

        [GlobalCleanup]
        public async Task TearDownAsync()
        {
            await Extensions.TearDownProviderAsync(this.Provider!, nameof(Benchmark3));
            this.genericRulesEngine = null;
        }
    }
}