namespace Regulae
{
    /// <summary>
    /// Defines the cardinalities a operand can assume.
    /// </summary>
    public enum Cardinalities
    {
        /// <summary>
        /// The one cardinality for operands that can assume a single value.
        /// </summary>
        One = 0b0,

        /// <summary>
        /// The many cardinality for operands that can assume multiple values.
        /// </summary>
        Many = 0b1,
    }
}