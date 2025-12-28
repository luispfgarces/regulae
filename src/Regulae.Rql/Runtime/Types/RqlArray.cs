namespace Regulae.Rql.Runtime.Types
{
    using System;
    using System.Diagnostics;
    using System.Text;
    using Regulae.Rql.Runtime;

    /// <summary>
    /// Defines the .NET representation of a RQL &lt;array&gt; value.
    /// </summary>
    [DebuggerDisplay("<{this.Type.Name,nq}>")]
    public readonly struct RqlArray : IRuntimeValue, IEquatable<RqlArray>
    {
        private static readonly Type runtimeType = typeof(object[]);
        private static readonly RqlType type = RqlTypes.Array;
        private readonly int size;

        /// <summary>
        /// Initializes a new instance of the <see cref="RqlArray"/> struct.
        /// </summary>
        /// <param name="size">The size.</param>
        public RqlArray(int size)
            : this(size, true)
        {
        }

        internal RqlArray(int size, bool shouldInitializeElements)
        {
            if (size < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(size), size, $"The size of '{nameof(RqlArray)}' cannot be negative.");
            }

            this.size = size;
            this.Value = new RqlAny[size];
            if (shouldInitializeElements)
            {
                Array.Fill(this.Value, new RqlAny());
            }
        }

        /// <inheritdoc/>
        public Type RuntimeType => runtimeType;

        /// <inheritdoc/>
        public object RuntimeValue => ConvertToNativeArray(this);

        /// <summary>
        /// Gets the RQL &lt;array&gt; size.
        /// </summary>
        /// <value>The size.</value>
        public RqlInteger Size => this.size;

        /// <inheritdoc/>
        public RqlType Type => type;

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public readonly RqlAny[] Value { get; }

        /// <summary>
        /// Converts the RQL &lt;array&gt; to a native array.
        /// </summary>
        /// <param name="rqlArray">The RQL &lt;array&gt; to convert.</param>
        /// <returns>the native array typed as object.</returns>
        public static object[] ConvertToNativeArray(RqlArray rqlArray)
        {
            var result = new object[rqlArray.size];
            for (var i = 0; i < rqlArray.size; i++)
            {
                result[i] = rqlArray.Value[i].RuntimeValue;
            }

            return result;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="RqlArray"/> to <see cref="RqlAny"/>.
        /// </summary>
        /// <param name="rqlArray">The RQL array.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator RqlAny(RqlArray rqlArray) => new RqlAny(rqlArray);

        /// <inheritdoc/>
        public bool Equals(RqlArray other)
        {
            if (this.Size != other.Size)
            {
                return false;
            }

            for (var i = 0; i < this.size; i++)
            {
                if (!this.Value[i].Equals(other.Value[i]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Sets the value at specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// thrown when specified index is out of the array boundaries.
        /// </exception>
        public RqlNothing SetAtIndex(RqlInteger index, RqlAny value)
        {
            if (index.Value < 0 || index.Value >= this.size)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, $"The value of '{index}' is out of the '{nameof(RqlArray)}' range.");
            }

            this.Value[index.Value] = value;
            return new RqlNothing();
        }

        /// <inheritdoc/>
        public override string ToString()
            => this.ToPrettyString();

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is RqlArray array && this.Equals(array);

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(this.size);
            foreach (var item in this.Value)
            {
                hashCode.Add(item);
            }

            return hashCode.ToHashCode();
        }

        /// <inheritdoc/>
        public string ToPrettyString() => this.ToPrettyString(0);

        /// <inheritdoc/>
        public string ToPrettyString(int indentLevel)
        {
            var stringBuilder = new StringBuilder()
                .Append('<')
                .Append(this.Type.Name)
                .Append('>');

            if (this.size > 0)
            {
                stringBuilder.AppendLine()
                    .Append(new string(' ', indentLevel))
                    .Append('{')
                    .AppendLine();
                var min = Math.Min(this.size, 5);
                for (var i = 0; i < min; i++)
                {
                    stringBuilder.Append(new string(' ', indentLevel + 4))
                        .Append(this.Value[i].ToPrettyString(indentLevel + 4));

                    if (i < min - 1)
                    {
                        stringBuilder.Append(',')
                            .AppendLine();
                    }
                }

                if (min < this.size)
                {
                    stringBuilder.Append(',')
                        .AppendLine()
                        .Append(new string(' ', indentLevel + 4))
                        .Append("...");
                }

                stringBuilder.AppendLine()
                    .Append(new string(' ', indentLevel))
                    .Append('}');
            }
            else
            {
                stringBuilder.Append(" { (empty) }");
            }

            return stringBuilder.ToString();
        }
    }
}