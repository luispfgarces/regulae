namespace Regulae
{
    using System.Collections.Generic;
    using Regulae.Cache;

    internal sealed class RulesEngineOptions : IRulesEngineOptions
    {
        private RulesEngineOptions()
        {
            this.DataTypeDefaults = new Dictionary<DataTypes, object>();
        }

        public bool AutoCreateRulesets { get; set; }

        public ICache? Cache { get; set; }

        public IDictionary<DataTypes, object> DataTypeDefaults { get; }

        public EvaluationStrategies EvaluationStrategy { get; set; }

        public MissingConditionBehaviors MissingConditionBehavior { get; set; }

        public PriorityCriterias PriorityCriteria { get; set; }

        public static RulesEngineOptions NewWithDefaults()
        {
            RulesEngineOptions rulesEngineOptions = new()
            {
                EvaluationStrategy = EvaluationStrategies.Interpreted,
                MissingConditionBehavior = MissingConditionBehaviors.UseDataTypeDefault,
                PriorityCriteria = PriorityCriterias.SmallestNumber,
                DataTypeDefaults =
                {
                    [DataTypes.Boolean] = default(bool),
                    [DataTypes.Decimal] = default(decimal),
                    [DataTypes.Integer] = default(int),
                    [DataTypes.String] = string.Empty,
                },
                AutoCreateRulesets = false,
            };

            return rulesEngineOptions;
        }
    }
}