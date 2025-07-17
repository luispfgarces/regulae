namespace Regulae.Builder
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Regulae.Cache;

    internal sealed class RulesEngineOptionsBuilder : IRulesEngineConfiguration
    {
        private bool? autoCreateRulesets;
        private ICache? cache;
        private IDictionary<DataTypes, object> dataTypeDefaults;
        private EvaluationStrategies? evaluationStrategy;
        private MissingConditionBehaviors? missingConditionBehavior;
        private PriorityCriterias? priorityCriteria;

        public RulesEngineOptionsBuilder()
        {
            this.dataTypeDefaults = new Dictionary<DataTypes, object>();
        }

        public RulesEngineOptions Build()
        {
            var rulesEngineOptions = RulesEngineOptions.NewWithDefaults();

            if (this.autoCreateRulesets.HasValue)
            {
                rulesEngineOptions.AutoCreateRulesets = this.autoCreateRulesets.Value;
            }

            if (this.cache != null)
            {
                rulesEngineOptions.Cache = this.cache;
            }

            if (this.dataTypeDefaults.Count > 0)
            {
                foreach (var dataTypeDefault in this.dataTypeDefaults)
                {
                    rulesEngineOptions.DataTypeDefaults[dataTypeDefault.Key] = dataTypeDefault.Value;
                }
            }

            if (this.evaluationStrategy.HasValue)
            {
                rulesEngineOptions.EvaluationStrategy = this.evaluationStrategy.Value;
            }

            if (this.missingConditionBehavior.HasValue)
            {
                rulesEngineOptions.MissingConditionBehavior = this.missingConditionBehavior.Value;
            }

            if (this.priorityCriteria.HasValue)
            {
                rulesEngineOptions.PriorityCriteria = this.priorityCriteria.Value;
            }

            return rulesEngineOptions;
        }

        public IRulesEngineConfiguration SetAutoCreateRulesets(bool autoCreateRulesets)
        {
            this.autoCreateRulesets = autoCreateRulesets;
            return this;
        }

        public IRulesEngineConfiguration SetDataTypeDefault(DataTypes dataType, object defaultValue)
        {
            ThrowArgumentExceptionIf(
                (dt, v) => dataType switch
                {
                    DataTypes.Integer => defaultValue is null || !int.TryParse(defaultValue.ToString(), NumberStyles.None, CultureInfo.InvariantCulture, out var _),
                    DataTypes.Decimal => defaultValue is null || !decimal.TryParse(defaultValue.ToString(), NumberStyles.None, CultureInfo.InvariantCulture, out var _),
                    DataTypes.String => defaultValue is null || defaultValue is not string,
                    DataTypes.Boolean => defaultValue is null || !bool.TryParse(defaultValue.ToString(), out var _),
                    _ => throw new ArgumentOutOfRangeException(nameof(dataType), $"Invalid {nameof(DataTypes)} value specified: {dataType}."),
                },
                dataType,
                defaultValue);

            this.dataTypeDefaults[dataType] = defaultValue;
            return this;

            static void ThrowArgumentExceptionIf(Func<DataTypes, object, bool> conditionFunc, DataTypes dataType, object defaultValue)
            {
                if (conditionFunc(dataType, defaultValue))
                {
                    throw new ArgumentException(
                        $"Specified invalid default value for data type {dataType}: {defaultValue ?? "null"}.", nameof(defaultValue));
                }
            }
        }

        public IRulesEngineConfiguration SetMissingConditionBehavior(MissingConditionBehaviors missingConditionBehavior)
        {
            if (!Enum.IsDefined(typeof(MissingConditionBehaviors), missingConditionBehavior))
            {
                throw new ArgumentOutOfRangeException(nameof(missingConditionBehavior), $"Invalid {nameof(MissingConditionBehaviors)} value specified.");
            }

            this.missingConditionBehavior = missingConditionBehavior;
            return this;
        }

        public IRulesEngineConfiguration UseCache(ICache cache)
        {
            this.cache = cache;
            return this;
        }

        public IRulesEngineConfiguration UseEvaluationStrategy(EvaluationStrategies evaluationStrategy)
        {
            if (!Enum.IsDefined(typeof(EvaluationStrategies), evaluationStrategy))
            {
                throw new ArgumentOutOfRangeException(nameof(evaluationStrategy), $"Invalid {nameof(EvaluationStrategies)} value specified.");
            }

            this.evaluationStrategy = evaluationStrategy;
            return this;
        }

        public IRulesEngineConfiguration UsePriorityCriteria(PriorityCriterias priorityCriteria)
        {
            if (!Enum.IsDefined(typeof(PriorityCriterias), priorityCriteria))
            {
                throw new ArgumentOutOfRangeException(nameof(priorityCriteria), $"Invalid {nameof(PriorityCriterias)} value specified.");
            }

            this.priorityCriteria = priorityCriteria;
            return this;
        }
    }
}