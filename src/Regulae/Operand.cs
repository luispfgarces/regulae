namespace Regulae
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Defines an operand that can be used in rules definition and evaluation.
    /// </summary>
    public sealed class Operand
    {
        /// <summary>
        /// The cardinality
        /// </summary>
        public readonly Cardinalities Cardinality;

        /// <summary>
        /// The data type
        /// </summary>
        public readonly DataTypes DataType;

        /// <summary>
        /// The value
        /// </summary>
        public readonly object? Value;

        private static readonly Dictionary<DataTypes, Operand> DefaultValues = new()
        {
            { DataTypes.String, new Operand(null, DataTypes.String, Cardinalities.One) },
            { DataTypes.Integer, new Operand(null, DataTypes.Integer, Cardinalities.One) },
            { DataTypes.Decimal, new Operand(null, DataTypes.Decimal, Cardinalities.One) },
            { DataTypes.Boolean, new Operand(null, DataTypes.Boolean, Cardinalities.One) },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="Operand"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <exception cref="System.NotSupportedException">
        /// The provided value has a unsupported runtime type, and a data type and a cardinality
        /// cannot be determined: '{value?.GetType().FullName}'.
        /// </exception>
        public Operand(object? value)
        {
            this.Value = value;
            switch (value)
            {
                case string:
                    this.DataType = DataTypes.String;
                    this.Cardinality = Cardinalities.One;
                    break;

                case int:
                    this.DataType = DataTypes.Integer;
                    this.Cardinality = Cardinalities.One;
                    break;

                case decimal:
                    this.DataType = DataTypes.Decimal;
                    this.Cardinality = Cardinalities.One;
                    break;

                case bool:
                    this.DataType = DataTypes.Boolean;
                    this.Cardinality = Cardinalities.One;
                    break;

                case IEnumerable<string>:
                    this.DataType = DataTypes.String;
                    this.Cardinality = Cardinalities.Many;
                    break;

                case IEnumerable<int>:
                    this.DataType = DataTypes.Integer;
                    this.Cardinality = Cardinalities.Many;
                    break;

                case IEnumerable<decimal>:
                    this.DataType = DataTypes.Decimal;
                    this.Cardinality = Cardinalities.Many;
                    break;

                case IEnumerable<bool>:
                    this.DataType = DataTypes.Boolean;
                    this.Cardinality = Cardinalities.Many;
                    break;

                default:
                    throw new NotSupportedException(
                        $"The provided value has a unsupported runtime type, and a data type and a cardinality cannot be determined: '{value?.GetType().FullName}'.");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Operand"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="dataType">Type of the data.</param>
        /// <param name="cardinality">The cardinality.</param>
        public Operand(object? value, DataTypes dataType, Cardinalities cardinality)
        {
            this.Value = value;
            this.DataType = dataType;
            this.Cardinality = cardinality;
        }

        /// <summary>
        /// Gets the default operand value for the specified data type.
        /// </summary>
        /// <param name="dataType">Type of the data.</param>
        /// <returns></returns>
        public static Operand DefaultFor(DataTypes dataType)
        {
            return DefaultValues[dataType];
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Boolean"/> to <see cref="Operand"/>.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Operand(bool value) => new(value);

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Decimal"/> to <see cref="Operand"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Operand(decimal value) => new(value);

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Int32"/> to <see cref="Operand"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Operand(int value) => new(value);

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="Operand"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Operand(string value) => new(value);

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Boolean[]"/> to <see cref="Operand"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Operand(bool[] value) => new(value);

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Decimal[]"/> to <see cref="Operand"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Operand(decimal[] value) => new(value);

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Int32[]"/> to <see cref="Operand"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Operand(int[] value) => new(value);

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.String[]"/> to <see cref="Operand"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Operand(string[] value) => new(value);

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            if (obj is Operand operand)
            {
                var result = this.DataType == operand.DataType && this.Cardinality == operand.Cardinality;
                if (this.Value is null && operand.Value is null)
                {
                    return result;
                }

                if (this.Cardinality == Cardinalities.Many && operand.Cardinality == Cardinalities.Many)
                {
                    result &= this.Value switch
                    {
                        IEnumerable<string> arrayString1 when operand.Value is IEnumerable<string> arrayString2 => arrayString1.SequenceEqual(arrayString2, StringComparer.Ordinal),
                        IEnumerable<int> arrayInteger1 when operand.Value is IEnumerable<int> arrayInteger2 => arrayInteger1.SequenceEqual(arrayInteger2),
                        IEnumerable<decimal> arrayDecimal1 when operand.Value is IEnumerable<decimal> arrayDecimal2 => arrayDecimal1.SequenceEqual(arrayDecimal2),
                        IEnumerable<bool> arrayBool1 when operand.Value is IEnumerable<bool> arrayBool2 => arrayBool1.SequenceEqual(arrayBool2),
                        _ => throw new NotSupportedException($"Data type is not supported for equality comparison: '{this.DataType}'."),
                    };
                }
                else
                {
                    result &= EqualityComparer<object>.Default.Equals(this.Value!, operand.Value!);
                }

                return result;
            }

            return false;
        }

        /// <inheritdoc/>
        public override int GetHashCode() => HashCode.Combine(this.DataType, this.Cardinality, this.Value);
    }
}