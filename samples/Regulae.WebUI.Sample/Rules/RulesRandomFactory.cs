namespace Regulae.WebUI.Sample.Rules
{
    using System;
    using System.Collections.Generic;
    using Regulae;
    using Regulae.Builder.Generic;
    using Regulae.WebUI.Sample.Engine;
    using Regulae.WebUI.Sample.Enums;

    internal class RulesRandomFactory : IRuleSpecificationsProvider
    {
        private readonly int finalNumber = 50;
        private readonly int intialNumber = 10;
        private readonly Random random;

        public RulesRandomFactory()
        {
            this.random = new Random();
        }

        public (ConditionNames Condition, DataTypes DataType)[] Conditions =>
        [
            (ConditionNames.RoyalNumber, DataTypes.Integer),
            (ConditionNames.SumAll, DataTypes.Integer),
            (ConditionNames.IsPrimeNumber, DataTypes.Boolean),
            (ConditionNames.CanNumberBeDividedBy3, DataTypes.Boolean),
        ];

        public RulesetNames[] Rulesets =>
        [
            RulesetNames.TestDateTime,
            RulesetNames.TestDecimal,
            RulesetNames.TestLong,
            RulesetNames.TestBoolean,
            RulesetNames.TestShort,
            RulesetNames.TestNumber,
            RulesetNames.TestString,
            RulesetNames.TestBlob,
        ];

        public IEnumerable<RuleSpecification> GetRulesSpecifications()
        {
            var currentYear = DateTime.UtcNow.Year;
            var rulesSpecifications = new List<RuleSpecification>();

            foreach (var ruleset in Enum.GetValues(typeof(RulesetNames)).Cast<RulesetNames>())
            {
                for (var i = 1; i < this.random.Next(this.intialNumber, this.finalNumber); i++)
                {
                    var dateBegin = this.CreateRandomDateBegin(currentYear);

                    this.Add(CreateMultipleRule(ruleset, i, dateBegin, this.CreateRandomDateEnd(dateBegin)),
                        RuleAddPriorityOption.AtNumber(i),
                        rulesSpecifications);
                }

                var deactiveDateBegin = this.CreateRandomDateBegin(currentYear);

                this.Add(CreateMultipleRule(ruleset, this.finalNumber, deactiveDateBegin, this.CreateRandomDateEnd(deactiveDateBegin), isActive: false),
                        RuleAddPriorityOption.AtNumber(this.finalNumber),
                        rulesSpecifications);
            }

            return rulesSpecifications;
        }

        private static RuleBuilderResult<RulesetNames, ConditionNames> CreateMultipleRule(
            RulesetNames ruleset,
            int value,
            DateTime dateBegin,
            DateTime? dateEnd,
            bool isActive = true) => Rule.Create<RulesetNames, ConditionNames>($"Multi rule for test {ruleset} {value}")
                .InRuleset(ruleset)
                .SetContent(new { Value = value })
                .Since(dateBegin)
                .Until(dateEnd)
                .WithActive(isActive)
                .ApplyWhen(rootCond => rootCond
                    .Or(o => o
                        .Value(ConditionNames.RoyalNumber, Operators.Equal, 7)
                        .Value(ConditionNames.SumAll, Operators.In, new int[] { 9, 8, 6 })
                        .And(a => a
                            .Value(ConditionNames.IsPrimeNumber, Operators.Equal, false)
                            .Value(ConditionNames.SumAll, Operators.GreaterThanOrEqual, 15)
                        )
                        .And(a => a
                            .Value(ConditionNames.CanNumberBeDividedBy3, Operators.Equal, false)
                            .Value(ConditionNames.SumAll, Operators.NotEqual, 0)
                        )
                        .And(a => a
                            .Value(ConditionNames.IsPrimeNumber, Operators.Equal, true)
                            .Value(ConditionNames.SumAll, Operators.LesserThan, 5)
                            .Value(ConditionNames.CanNumberBeDividedBy3, Operators.Equal, false)
                        )))
                .Build();

        private void Add(
            RuleBuilderResult<RulesetNames, ConditionNames> rule,
            RuleAddPriorityOption ruleAddPriorityOption, List<RuleSpecification> rulesSpecifications)
            => rulesSpecifications.Add(new RuleSpecification(rule, ruleAddPriorityOption));

        private DateTime CreateRandomDateBegin(int year)
        {
            var months = this.random.Next(1, 11);
            year = this.random.Next(0, 1) + year;
            return new DateTime(year, 1, 1).AddMonths(months);
        }

        private DateTime? CreateRandomDateEnd(DateTime dateBegin)
        {
            var months = this.random.Next(0, 13);
            if (months == 13)
            {
                return null;
            }

            return dateBegin.AddMonths(months).AddDays(1);
        }
    }
}