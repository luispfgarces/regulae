namespace Regulae
{
    using System;

    /// <summary>
    /// Defines a condition used to constrain the applicability of rules.
    /// </summary>
    public class Condition
    {
        internal Condition(string name, DateTime creation, DataTypes dataType)
        {
            this.Name = name;
            this.Creation = creation;
            this.DataType = dataType;
        }

        /// <summary>
        /// Gets the creation date and time.
        /// </summary>
        /// <value>
        /// The creation date and time.
        /// </value>
        public DateTime Creation { get; }

        /// <summary>
        /// Gets the data type of the condition.
        /// </summary>
        /// <value>
        /// The data type of the condition.
        /// </value>
        public DataTypes DataType { get; }

        /// <summary>
        /// Gets the name of the condition.
        /// </summary>
        /// <value>
        /// The name of the condition.
        /// </value>
        public string Name { get; }
    }
}