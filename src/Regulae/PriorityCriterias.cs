namespace Regulae
{
    /// <summary>
    /// Defines the available rules engine priority criterias to untie when multiple rules are
    /// matched to the set of conditions supplied.
    /// </summary>
    public enum PriorityCriterias : byte
    {
        /// <summary>
        /// Sets the rule with the smallest priority number to win on a untie scenario.
        /// </summary>
        SmallestNumber = 0,

        /// <summary>
        /// Sets the rule with the largest priority number to win on a untie scenario.
        /// </summary>
        LargestNumber = 1,
    }
}