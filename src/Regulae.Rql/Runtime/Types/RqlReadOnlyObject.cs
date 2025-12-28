namespace Regulae.Rql.Runtime.Types
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using Regulae.Rql.Runtime;

    /// <summary>
    /// Defines the .NET representation of a RQL &lt;read_only_object&gt; value.
    /// </summary>
    [DebuggerDisplay("<{this.Type.Name,nq}>")]
    public readonly struct RqlReadOnlyObject : IRuntimeValue, IEquatable<RqlReadOnlyObject>
    {
        private static readonly Type runtimeType = typeof(object);
        private static readonly RqlType type = RqlTypes.ReadOnlyObject;
        private readonly IDictionary<string, RqlAny> properties;

        internal RqlReadOnlyObject(IDictionary<string, RqlAny> properties)
        {
            this.properties = properties;
        }

        /// <inheritdoc/>
        public Type RuntimeType => runtimeType;

        /// <inheritdoc/>
        public object RuntimeValue => this.Value;

        /// <inheritdoc/>
        public RqlType Type => type;

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public object Value => ConvertToDictionary(this);

        /// <summary>
        /// Performs an implicit conversion from <see cref="RqlReadOnlyObject"/> to <see cref="RqlAny"/>.
        /// </summary>
        /// <param name="rqlReadOnlyObject">The RQL read only object.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator RqlAny(RqlReadOnlyObject rqlReadOnlyObject) => new RqlAny(rqlReadOnlyObject);

        /// <inheritdoc/>
        public bool Equals(RqlReadOnlyObject other)
        {
            foreach (var kvp in this.properties)
            {
                if (!other.properties.TryGetValue(kvp.Key, out var otherValue) || !kvp.Value.Equals(otherValue))
                {
                    return false;
                }
            }

            return true;
        }

        /// <inheritdoc/>
        public override string ToString() => this.ToPrettyString();

        private static IDictionary<string, object> ConvertToDictionary(RqlReadOnlyObject value)
        {
            var result = new Dictionary<string, object>(StringComparer.Ordinal);
            foreach (var kvp in value.properties)
            {
                result[kvp.Key] = kvp.Value.RuntimeValue;
            }

            return result;
        }


        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is RqlReadOnlyObject @object && this.Equals(@object);

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            foreach (var property in this.properties)
            {
                hashCode.Add(property.Key);
                hashCode.Add(property.Value);
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
                .Append('>')
                .AppendLine()
                .Append(new string(' ', indentLevel))
                .Append('{');

            foreach (var property in this.properties)
            {
                stringBuilder.AppendLine()
                    .Append(new string(' ', indentLevel + 4))
                    .Append(property.Key)
                    .Append(": ")
                    .Append(property.Value.ToPrettyString(indentLevel + 4));
            }

            return stringBuilder.AppendLine()
                .Append(new string(' ', indentLevel))
                .Append('}')
                .ToString();
        }
    }
}