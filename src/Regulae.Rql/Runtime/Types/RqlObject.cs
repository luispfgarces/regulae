namespace Regulae.Rql.Runtime.Types
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using Regulae.Rql.Runtime;

    /// <summary>
    /// Defines the .NET representation of a RQL &lt;object&gt; value.
    /// </summary>
    [DebuggerDisplay("<{this.Type.Name,nq}>")]
    public readonly struct RqlObject : IRuntimeValue, IPropertySet, IEquatable<RqlObject>
    {
        private static readonly Type runtimeType = typeof(object);
        private static readonly RqlType type = RqlTypes.Object;
        private readonly Dictionary<string, RqlAny> properties;

        /// <summary>
        /// Initializes a new instance of the <see cref="RqlObject"/> struct.
        /// </summary>
        public RqlObject()
        {
            this.properties = new Dictionary<string, RqlAny>(StringComparer.Ordinal);
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
        /// Performs an implicit conversion from <see cref="RqlObject"/> to <see cref="RqlAny"/>.
        /// </summary>
        /// <param name="rqlObject">The RQL object.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator RqlAny(RqlObject rqlObject) => new RqlAny(rqlObject);

        /// <inheritdoc/>
        public bool Equals(RqlObject other)
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
        public RqlAny SetPropertyValue(RqlString name, RqlAny value) => this.properties[name.Value] = value;

        /// <inheritdoc/>
        public override string ToString() => this.ToPrettyString();

        private static IDictionary<string, object> ConvertToDictionary(RqlObject value)
        {
            var result = new Dictionary<string, object>(StringComparer.Ordinal);
            foreach (var kvp in value.properties)
            {
                result[kvp.Key] = kvp.Value.RuntimeValue;
            }

            return result;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is RqlObject @object && this.Equals(@object);

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