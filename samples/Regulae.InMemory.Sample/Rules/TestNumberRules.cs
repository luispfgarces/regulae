namespace Regulae.InMemory.Sample.Rules
{
    using System;
    using System.Collections.Generic;
    using Regulae;
    using Regulae.Builder.Generic;
    using Regulae.InMemory.Sample.Engine;
    using Regulae.InMemory.Sample.Enums;

    internal class TestNumberRules : IRuleSpecificationsProvider
    {
        public readonly List<RuleSpecification> rulesSpecifications;

        public TestNumberRules()
        {
            this.rulesSpecifications = [];
        }

        public RulesetNames[] Rulesets => [RulesetNames.TestNumber,];

        public IReadOnlyDictionary<ConditionNames, DataTypes> Conditions => new Dictionary<ConditionNames, DataTypes>
        {
            { ConditionNames.RoyalNumber, DataTypes.Integer },
            { ConditionNames.IsPrimeNumber, DataTypes.Boolean },
            { ConditionNames.CanNumberBeDividedBy3, DataTypes.Boolean },
            { ConditionNames.SumAll, DataTypes.Integer },
        };

        public IEnumerable<RuleSpecification> GetRulesSpecifications()
        {
            this.Add(this.CreateRuleForCoolNumbers(), RuleAddPriorityOption.AtNumber(3));
            this.Add(this.CreateRuleForSosoNumbers(), RuleAddPriorityOption.AtNumber(2));
            this.Add(this.CreateDefaultRule(), RuleAddPriorityOption.AtNumber(1));

            return this.rulesSpecifications;
        }

        private void Add(
            RuleBuilderResult<RulesetNames, ConditionNames> rule,
            RuleAddPriorityOption ruleAddPriorityOption) => this.rulesSpecifications.Add(
                new RuleSpecification
                {
                    RuleBuilderResult = rule,
                    RuleAddPriorityOption = ruleAddPriorityOption,
                });

        private RuleBuilderResult<RulesetNames, ConditionNames> CreateDefaultRule() => Rule.Create<RulesetNames, ConditionNames>("Default rule for test number")
            .InRuleset(RulesetNames.TestNumber)
            .SetContent(":| default nothing special about this number")
            .Since(new DateTime(2019, 01, 01))
            .Build();

        private RuleBuilderResult<RulesetNames, ConditionNames> CreateRuleForCoolNumbers() => Rule.Create<RulesetNames, ConditionNames>("Rule for cool numbers")
            .InRuleset(RulesetNames.TestNumber)
            .SetContent(":D this number is so COOL!")
            .Since(new DateTime(2019, 01, 01))
            .ApplyWhen(c => c
                .Or(o => o
                    .Value(ConditionNames.RoyalNumber, Operators.Equal, 7)
                    .And(a => a
                        .Value(ConditionNames.IsPrimeNumber, Operators.Equal, 7)
                        .Value(ConditionNames.SumAll, Operators.GreaterThanOrEqual, 5))))
            .Build();

        private RuleBuilderResult<RulesetNames, ConditionNames> CreateRuleForSosoNumbers() => Rule.Create<RulesetNames, ConditionNames>("Rule for so so numbers")
            .InRuleset(RulesetNames.TestNumber)
            .SetContent(":) this number is so so")
            .Since(new DateTime(2019, 01, 01))
            .ApplyWhen(c => c
                .Or(o => o
                    .Value(ConditionNames.CanNumberBeDividedBy3, Operators.Equal, true)
                    .Value(ConditionNames.IsPrimeNumber, Operators.Equal, false)
                    .Value(ConditionNames.SumAll, Operators.GreaterThanOrEqual, 9)))
            .Build();
    }
}