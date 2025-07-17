namespace Regulae
{
    /// <summary>
    /// The priority options available to influence the priority at which a new rule is added to
    /// data source.
    /// </summary>
    public enum PriorityOptions : byte
    {
        /// <summary>
        /// Specifies to add rule positioned at the smallest priority number.
        /// </summary>
        AtSmallestNumber = 1,

        /// <summary>
        /// Specifies to add rule positioned at the largest priority number.
        /// </summary>
        AtLargestNumber = 2,

        /// <summary>
        /// Specifies to add rule positioned at existent rule's name. All subsequent rules
        /// (including the one referenced) are increased on priority by one.
        /// </summary>
        AtRuleName = 3,

        /// <summary>
        /// Specifies to add rule positioned at specified priority number given. Any existent rules
        /// at priority number given or superior are increase on priority by one.
        /// </summary>
        AtNumber = 4,
    }
}