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
            object sanitizedValue = null!;
            var sanitationSucceeded = false;
            if (defaultValue is not null)
            {
                sanitationSucceeded = dataType switch
                {
                    DataTypes.Integer => TrySanitizeInteger(defaultValue, out sanitizedValue),
                    DataTypes.Decimal => TrySanitizeDecimal(defaultValue, out sanitizedValue),
                    DataTypes.String => TrySanitizeString(defaultValue, out sanitizedValue),
                    DataTypes.Boolean => TrySanitizeBool(defaultValue, out sanitizedValue),
                    _ => throw new ArgumentOutOfRangeException(nameof(dataType), $"Invalid {nameof(DataTypes)} value specified: {dataType}."),
                };
            }

            if (!sanitationSucceeded)
            {
                throw new ArgumentException(
                    $"Specified invalid default value for data type {dataType}: {defaultValue ?? "null"}.", nameof(defaultValue));
            }

            this.dataTypeDefaults[dataType] = sanitizedValue;
            return this;
        }

        private static bool TrySanitizeBool(object inputValue, out object sanitizedValue)
        {
            if (inputValue is bool nativeValue)
            {
                sanitizedValue = nativeValue;
                return true;
            }

            if (bool.TryParse(inputValue.ToString(), out var parsedValue))
            {
                sanitizedValue = parsedValue;
                return true;
            }

            sanitizedValue = null!;
            return false;
        }

        private static bool TrySanitizeDecimal(object inputValue, out object sanitizedValue)
        {
            if (inputValue is decimal nativeValue)
            {
                sanitizedValue = nativeValue;
                return true;
            }

            if (decimal.TryParse(inputValue.ToString(), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var parsedValue))
            {
                sanitizedValue = parsedValue;
                return true;
            }

            sanitizedValue = null!;
            return false;
        }

        private static bool TrySanitizeInteger(object inputValue, out object sanitizedValue)
        {
            if (inputValue is int intValue)
            {
                sanitizedValue = intValue;
                return true;
            }

            if (int.TryParse(inputValue.ToString(), NumberStyles.None, CultureInfo.InvariantCulture, out var intParsedValue))
            {
                sanitizedValue = intParsedValue;
                return true;
            }

            sanitizedValue = null!;
            return false;
        }

        private static bool TrySanitizeString(object inputValue, out object sanitizedValue)
        {
            if (inputValue is string nativeValue)
            {
                sanitizedValue = nativeValue;
                return true;
            }

            sanitizedValue = null!;
            return false;
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