namespace Regulae
{
    using Regulae.Evaluation;

    /// <summary>
    /// Defines the supported data types a condition node can assume.
    /// </summary>
    public enum DataTypes
    {
        /// <summary>
        /// The integer data type for condition nodes based on integer values.
        /// </summary>
        [DataTypeValuePattern("[0-9]+")]
        Integer = 1,

        /// <summary>
        /// The decimal data type for condition nodes based on decimal values.
        /// </summary>
        [DataTypeValuePattern(@"[0-9]+((\.|,)[0-9]+)?")]
        Decimal = 2,

        /// <summary>
        /// The string data type for condition nodes based on string values.
        /// </summary>
        [DataTypeValuePattern(".*")]
        String = 3,

        /// <summary>
        /// The boolean data type for condition nodes based on boolean values.
        /// </summary>
        [DataTypeValuePattern("(true|false)")]
        Boolean = 4,
    }
}