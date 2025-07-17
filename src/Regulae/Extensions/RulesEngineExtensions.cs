namespace Regulae.Extensions
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Regulae;
    using Regulae.Generic;

    /// <summary>
    /// Extensions for rules engine
    /// </summary>
    public static class RulesEngineExtensions
    {
        /// <summary>
        /// Creates a generic rules engine.
        /// </summary>
        /// <typeparam name="TRuleset">The ruleset type that strongly types rulesets.</typeparam>
        /// <typeparam name="TCondition">The condition type that strongly types conditions.</typeparam>
        /// <param name="rulesEngine">The rules engine.</param>
        /// <returns>A new instance of generic engine</returns>
        public static IRulesEngine<TRuleset, TCondition> MakeGeneric<TRuleset, TCondition>(
            this IRulesEngine rulesEngine)
            => rulesEngine.MakeGeneric<TRuleset, TCondition>(opt => { });

        /// <summary>
        /// Creates a generic rules engine.
        /// </summary>
        /// <typeparam name="TRuleset">The ruleset type that strongly types rulesets.</typeparam>
        /// <typeparam name="TCondition">The condition type that strongly types conditions.</typeparam>
        /// <param name="rulesEngine">The rules engine.</param>
        /// <param name="configureGenericRulesEngineOptions">
        /// The configure generic rules engine options action.
        /// </param>
        /// <returns>A new instance of generic engine</returns>
        /// <exception cref="System.InvalidOperationException">
        /// The condition value '{condition}' does not declare attribute [DataType] which is
        /// required to auto create conditions. Please declare the attribute with the desired data type.
        /// </exception>
        public static IRulesEngine<TRuleset, TCondition> MakeGeneric<TRuleset, TCondition>(
            this IRulesEngine rulesEngine,
            Action<GenericRulesEngineOptions> configureGenericRulesEngineOptions)
        {
            var genericRulesEngineOptions = new GenericRulesEngineOptions();
            configureGenericRulesEngineOptions(genericRulesEngineOptions);

            var genericRulesEngine = new RulesEngine<TRuleset, TCondition>(rulesEngine);
            if (genericRulesEngineOptions.AutoCreateConditions)
            {
                var conditionType = typeof(TCondition);
                var conditions = Enum.GetValues(conditionType).Cast<TCondition>();
                foreach (var condition in conditions)
                {
                    var dataTypeAttribute = conditionType.GetMember(condition!.ToString()).First().GetCustomAttribute<DataTypeAttribute>();
                    if (dataTypeAttribute is null)
                    {
                        throw new InvalidOperationException($"The condition value '{condition}' does not declare attribute [DataType] which is required " +
                            $"to auto create conditions. Please declare the attribute with the desired data type.");
                    }

                    genericRulesEngine.CreateConditionAsync(condition, dataTypeAttribute.DataType).GetAwaiter().GetResult();
                }
            }

            return genericRulesEngine;
        }
    }
}