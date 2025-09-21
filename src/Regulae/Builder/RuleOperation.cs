namespace Regulae.Builder
{
    using System.Collections.Generic;

    internal static class RuleOperation
    {
        /// <summary>
        /// Creates a result marked with failure.
        /// </summary>
        /// <param name="errors">The errors.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">errors</exception>
        internal static RuleBuilderResult Failure(IList<OperationError> errors)
        {
            if (errors is null)
            {
                throw new System.ArgumentNullException(nameof(errors));
            }

            return new RuleBuilderResult(rule: null, errors: errors);
        }

        /// <summary>
        /// Creates a result marked with success.
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">rule</exception>
        internal static RuleBuilderResult Success(Rule rule)
        {
            if (rule is null)
            {
                throw new System.ArgumentNullException(nameof(rule));
            }

            return new RuleBuilderResult(rule: rule, errors: []);
        }
    }
}
