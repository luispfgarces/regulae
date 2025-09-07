namespace Regulae.Builder.Generic
{
    using System;
    using System.Collections.Generic;
    using Regulae.Builder;
    using Regulae.Generic;

    internal static class RuleBuilderResult
    {
        /// <summary>
        /// Creates a result marked with failure.
        /// </summary>
        /// <param name="errors">The errors.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">errors</exception>
        public static RuleBuilderResult<TRuleset, TCondition> Failure<TRuleset, TCondition>(IEnumerable<string> errors)
            where TRuleset : notnull
            where TCondition : notnull
        {
            ArgumentNullException.ThrowIfNull(errors);

            return new RuleBuilderResult<TRuleset, TCondition>(isSuccess: false, null!, errors);
        }

        /// <summary>
        /// Creates a result marked with success.
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">rule</exception>
        public static RuleBuilderResult<TRuleset, TCondition> Success<TRuleset, TCondition>(Rule<TRuleset, TCondition> rule)
            where TRuleset : notnull
            where TCondition : notnull
        {
            ArgumentNullException.ThrowIfNull(rule);

            return new RuleBuilderResult<TRuleset, TCondition>(isSuccess: true, rule, []);
        }
    }

    /// <summary>
    /// Contains the results information from a generic rule build operation.
    /// </summary>
    public class RuleBuilderResult<TRuleset, TCondition> : RuleBuilderResultBase
        where TRuleset : notnull
        where TCondition : notnull
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuleBuilderResult"/> class.
        /// </summary>
        /// <param name="isSuccess">if set to <see langword="true"/> [is success].</param>
        /// <param name="rule">The rule.</param>
        /// <param name="errors">The errors.</param>
        internal RuleBuilderResult(bool isSuccess, Rule<TRuleset, TCondition> rule, IEnumerable<string> errors)
            : base(isSuccess, errors)
        {
            this.Rule = rule;
        }

        /// <summary>
        /// Gets the rule.
        /// </summary>
        /// <value>The rule.</value>
        public Rule<TRuleset, TCondition> Rule { get; }


    }
}