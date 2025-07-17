namespace Regulae
{
    using System;

    /// <summary>
    /// Annotates a target member with the correspondent data type.
    /// </summary>
    /// <seealso cref="System.Attribute"/>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class DataTypeAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataTypeAttribute"/> class.
        /// </summary>
        /// <param name="dataType">Type of the data.</param>
        public DataTypeAttribute(DataTypes dataType)
        {
            this.DataType = dataType;
        }

        /// <summary>
        /// Gets the data type associated with target member.
        /// </summary>
        /// <value>The data type.</value>
        public DataTypes DataType { get; }
    }
}