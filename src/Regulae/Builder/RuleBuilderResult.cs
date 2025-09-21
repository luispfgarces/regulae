namespace Regulae.Builder
{
    using System.Collections.Generic;
    using Regulae;

    /// <summary>
    /// Contains the results information from a non-generic rule build operation.
    /// </summary>
    public class RuleBuilderResult : OperationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RuleBuilderResult"/> class.
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <param name="errors">The errors.</param>
        internal RuleBuilderResult(Rule? rule, IList<OperationError> errors)
            : base(errors)
        {
            this.Rule = rule;
        }

        /// <summary>
        /// Gets the rule.
        /// </summary>
        /// <value>The rule.</value>
        public Rule? Rule { get; }
    }
}