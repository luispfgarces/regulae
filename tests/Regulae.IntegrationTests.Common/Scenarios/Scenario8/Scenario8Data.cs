namespace Regulae.IntegrationTests.Common.Scenarios.Scenario8
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Regulae.Generic;
    using Regulae.IntegrationTests.Common.Scenarios;

    public partial class Scenario8Data : IScenarioData<PokerRulesets, PokerConditions>
    {
        public IEnumerable<(PokerConditions, DataTypes)> AllConditions => this.GetConditions();

        public IEnumerable<Rule<PokerRulesets, PokerConditions>> AllRules => this.GetRules();

        public IEnumerable<PokerRulesets> AllRulesets => new[] { PokerRulesets.TexasHoldemPokerSingleCombinations };

        private IEnumerable<(PokerConditions, DataTypes)> GetConditions()
        {
            var conditionType = typeof(PokerConditions);
            var conditions = Enum.GetValues(conditionType).Cast<PokerConditions>();
            foreach (var condition in conditions)
            {
                var dataTypeAttribute = conditionType.GetMember(condition!.ToString()).First().GetCustomAttribute<DataTypeAttribute>();
                yield return (condition, dataTypeAttribute.DataType);
            }

            yield break;
        }

        private IEnumerable<Rule<PokerRulesets, PokerConditions>> GetRules()
        {
            // Does not consider the double pairs and full house combinations, as they would imply a
            // combinatorial explosion. For the purpose of the benchmark, scenario already simulates
            // a high number of rules.
            var highCards = this.GetHighCardsRules();

            var pairs = this.GetPairsRules();

            var threeOfAKind = this.GetThreeOfAKindRules();

            var straights = this.GetStraightRules();

            var flushs = this.GetFlushRules();

            var fourOfAKind = this.GetFourOfAKindRules();

            var straightFlushs = this.GetStraightFlushRules();

            var royalFlushs = this.GetRoyalFlushRules();

            return highCards.Concat(pairs).Concat(threeOfAKind).Concat(straights).Concat(flushs).Concat(fourOfAKind).Concat(straightFlushs).Concat(royalFlushs);
        }
    }
}