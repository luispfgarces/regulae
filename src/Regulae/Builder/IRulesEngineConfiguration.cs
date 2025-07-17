namespace Regulae.Builder
{
    using Regulae.Cache;

    /// <summary>
    /// Defines the rules engine configuration interface.
    /// </summary>
    public interface IRulesEngineConfiguration
    {
        /// <summary>
        /// Sets the configuration to wether create rulesets automatically or require manual and
        /// explicit creation. This setting affects the rules engine's behavior when adding new
        /// rules, which acts according to it when the ruleset is not found.
        /// </summary>
        /// <param name="autoCreateRulesets">
        /// if set to <c>true</c> rulesets are automatically created if not existent.
        /// </param>
        /// <returns></returns>
        IRulesEngineConfiguration SetAutoCreateRulesets(bool autoCreateRulesets);

        /// <summary>
        /// Sets the data type default value. The data type default value is used when a particular
        /// condition's value was not provided by the rules engine client for evaluation.
        /// </summary>
        /// <param name="dataType">Type of the data.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        IRulesEngineConfiguration SetDataTypeDefault(DataTypes dataType, object defaultValue);

        /// <summary>
        /// Sets the conditions evaluation mechanism behavior when a condition's value is missing
        /// (not provided by the rules engine client for evaluation).
        /// </summary>
        /// <param name="missingConditionBehavior">The missing condition behavior.</param>
        /// <returns></returns>
        IRulesEngineConfiguration SetMissingConditionBehavior(MissingConditionBehaviors missingConditionBehavior);

        /// <summary>
        /// Uses the provided cache.
        /// </summary>
        /// <param name="cache">The cache.</param>
        /// <returns></returns>
        IRulesEngineConfiguration UseCache(ICache cache);

        /// <summary>
        /// Uses the provided evaluation strategy for each rule's conditions.
        /// </summary>
        /// <param name="evaluationStrategy">The evaluation strategy.</param>
        /// <returns></returns>
        IRulesEngineConfiguration UseEvaluationStrategy(EvaluationStrategies evaluationStrategy);

        /// <summary>
        /// Uses the provided priority criteria. Priority criteria is used to determine the order in
        /// which rules are evaluated.
        /// </summary>
        /// <param name="priorityCriteria">The priority criteria.</param>
        /// <returns></returns>
        IRulesEngineConfiguration UsePriorityCriteria(PriorityCriterias priorityCriteria);
    }
}