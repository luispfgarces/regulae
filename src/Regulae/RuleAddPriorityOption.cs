namespace Regulae
{
    /// <summary>
    /// The set of options to influence a new rule's priority when adding to data source.
    /// </summary>
    public class RuleAddPriorityOption
    {
        private RuleAddPriorityOption()
        {
        }

        /// <summary>
        /// Creates a <see cref="RuleAddPriorityOption"/> setted to placed at the largest priority number.
        /// </summary>
        public static RuleAddPriorityOption AtLargestNumber { get; } = new()
        {
            AtRuleNameOptionValue = null,
            AtNumberOptionValue = 0,
            PriorityOption = PriorityOptions.AtLargestNumber,
        };

        /// <summary>
        /// Creates a <see cref="RuleAddPriorityOption"/> setted to placed at the smallest priority number.
        /// </summary>
        public static RuleAddPriorityOption AtSmallestNumber { get; } = new()
        {
            AtRuleNameOptionValue = null,
            AtNumberOptionValue = 0,
            PriorityOption = PriorityOptions.AtSmallestNumber,
        };

        /// <summary>
        /// Gets the priority number to use when <c
        /// cref="PriorityOptions.AtNumber">PriorityOptions.AtNumber</c> option is selected.
        /// </summary>
        /// <value>A priority number.</value>
        public int AtNumberOptionValue { get; private set; }

        /// <summary>
        /// Gets the rule name to use when <c
        /// cref="PriorityOptions.AtRuleName">PriorityOptions.AtRuleName</c> option is selected.
        /// </summary>
        /// <value>A rule name.</value>
        public string? AtRuleNameOptionValue { get; private set; }

        /// <summary>
        /// Gets the priority option.
        /// </summary>
        /// <value>The priority option.</value>
        public PriorityOptions PriorityOption { get; private set; }

        /// <summary>
        /// Creates a <see cref="RuleAddPriorityOption"/> setted by priority number.
        /// </summary>
        /// <param name="number">The priority number.</param>
        /// <returns></returns>
        public static RuleAddPriorityOption AtNumber(int number) => new()
        {
            AtRuleNameOptionValue = null,
            AtNumberOptionValue = number,
            PriorityOption = PriorityOptions.AtNumber,
        };

        /// <summary>
        /// Creates a <see cref="RuleAddPriorityOption"/> setted by rule name.
        /// </summary>
        /// <param name="ruleName">Name of the rule.</param>
        /// <returns></returns>
        public static RuleAddPriorityOption AtRuleName(string ruleName) => new()
        {
            AtRuleNameOptionValue = ruleName,
            AtNumberOptionValue = 0,
            PriorityOption = PriorityOptions.AtRuleName,
        };
    }
}