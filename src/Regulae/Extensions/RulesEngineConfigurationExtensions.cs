namespace Regulae
{
    using System;
    using Regulae.Builder;
    using Regulae.Cache;

    /// <summary>
    /// Defines rules engine configuration extensions.
    /// </summary>
    public static class RulesEngineConfigurationExtensions
    {
        /// <summary>
        /// Disables the automatic creation of rulesets when any ruleset does not exist on new rules
        /// add to the rules engine. This setting affects the rules engine's behavior when adding
        /// new rules, which acts according to it when the ruleset is not found.
        /// </summary>
        /// <param name="rulesEngineConfiguration">The rules engine configuration.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">rulesEngineConfiguration</exception>
        public static IRulesEngineConfiguration DisableAutoCreateRulesets(
            this IRulesEngineConfiguration rulesEngineConfiguration)
        {
            if (rulesEngineConfiguration == null)
            {
                throw new ArgumentNullException(nameof(rulesEngineConfiguration));
            }

            return rulesEngineConfiguration.SetAutoCreateRulesets(autoCreateRulesets: false);
        }

        /// <summary>
        /// Enables the automatic creation of rulesets when any ruleset does not exist on new rules
        /// add to the rules engine. This setting affects the rules engine's behavior when adding
        /// new rules, which acts according to it when the ruleset is not found.
        /// </summary>
        /// <param name="rulesEngineConfiguration">The rules engine configuration.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">rulesEngineConfiguration</exception>
        public static IRulesEngineConfiguration EnableAutoCreateRulesets(
            this IRulesEngineConfiguration rulesEngineConfiguration)
        {
            if (rulesEngineConfiguration == null)
            {
                throw new ArgumentNullException(nameof(rulesEngineConfiguration));
            }

            return rulesEngineConfiguration.SetAutoCreateRulesets(autoCreateRulesets: true);
        }

        /// <summary>
        /// Uses the compiled evaluation strategy for each rule's conditions.
        /// </summary>
        /// <param name="rulesEngineConfiguration">The rules engine configuration.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">rulesEngineConfiguration</exception>
        public static IRulesEngineConfiguration UseCompiledEvaluationStrategy(
            this IRulesEngineConfiguration rulesEngineConfiguration)
        {
            if (rulesEngineConfiguration == null)
            {
                throw new ArgumentNullException(nameof(rulesEngineConfiguration));
            }

            return rulesEngineConfiguration.UseEvaluationStrategy(EvaluationStrategies.Compiled);
        }

        /// <summary>
        /// Uses a in memory cache.
        /// </summary>
        /// <param name="rulesEngineConfiguration">The rules engine configuration.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">rulesEngineConfiguration</exception>
        public static IRulesEngineConfiguration UseInMemoryCache(
            this IRulesEngineConfiguration rulesEngineConfiguration)
        {
            return rulesEngineConfiguration.UseInMemoryCache(cacheName: $"Regulae:{Guid.NewGuid()}");
        }

        /// <summary>
        /// Uses a in memory cache.
        /// </summary>
        /// <param name="rulesEngineConfiguration">The rules engine configuration.</param>
        /// <param name="cacheName">Name of the cache.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">rulesEngineConfiguration</exception>
        public static IRulesEngineConfiguration UseInMemoryCache(
            this IRulesEngineConfiguration rulesEngineConfiguration,
            string cacheName)
        {
            if (rulesEngineConfiguration == null)
            {
                throw new ArgumentNullException(nameof(rulesEngineConfiguration));
            }

            return rulesEngineConfiguration.UseCache(new InMemoryCache(cacheName));
        }

        /// <summary>
        /// Uses the interpreted evaluation strategy for each rule's conditions.
        /// </summary>
        /// <param name="rulesEngineConfiguration">The rules engine configuration.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">rulesEngineConfiguration</exception>
        public static IRulesEngineConfiguration UseInterpretedEvaluationStrategy(
            this IRulesEngineConfiguration rulesEngineConfiguration)
        {
            if (rulesEngineConfiguration == null)
            {
                throw new ArgumentNullException(nameof(rulesEngineConfiguration));
            }

            return rulesEngineConfiguration.UseEvaluationStrategy(EvaluationStrategies.Interpreted);
        }

        /// <summary>
        /// Uses as priority criteria the largest number, considering larger numbers as rules rules
        /// with the most priority. Priority criteria is used to determine the order in which rules
        /// are evaluated.
        /// </summary>
        /// <param name="rulesEngineConfiguration">The rules engine configuration.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">rulesEngineConfiguration</exception>
        public static IRulesEngineConfiguration UseLargestNumberPriorityCriteria(
            this IRulesEngineConfiguration rulesEngineConfiguration)
        {
            if (rulesEngineConfiguration == null)
            {
                throw new ArgumentNullException(nameof(rulesEngineConfiguration));
            }

            return rulesEngineConfiguration.UsePriorityCriteria(PriorityCriterias.PrioritizeLargestNumber);
        }

        /// <summary>
        /// Uses as priority criteria the smallest number, considering smaller numbers as rules with
        /// the most priority. Priority criteria is used to determine the order in which rules are evaluated.
        /// </summary>
        /// <param name="rulesEngineConfiguration">The rules engine configuration.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">rulesEngineConfiguration</exception>
        public static IRulesEngineConfiguration UseSmallestNumberPriorityCriteria(
            this IRulesEngineConfiguration rulesEngineConfiguration)
        {
            if (rulesEngineConfiguration == null)
            {
                throw new ArgumentNullException(nameof(rulesEngineConfiguration));
            }

            return rulesEngineConfiguration.UsePriorityCriteria(PriorityCriterias.PrioritizeSmallestNumber);
        }
    }
}