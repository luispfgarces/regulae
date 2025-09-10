namespace Regulae.Rql.Runtime.Types
{
    using System;
    using System.Diagnostics;
    using Regulae.Rql.Runtime;

    /// <summary>
    /// Defines the .NET representation of a RQL &lt;nothing&gt; value.
    /// </summary>
    [DebuggerDisplay("<{this.Type.Name,nq}>")]
    public readonly struct RqlNothing : IRuntimeValue, IEquatable<RqlNothing>
    {
        private static readonly Type runtimeType = typeof(object);
        private static readonly RqlType type = RqlTypes.Nothing;

        /// <inheritdoc/>
        public Type RuntimeType => runtimeType;

        /// <inheritdoc/>
        public object RuntimeValue => null;

        /// <inheritdoc/>
        public RqlType Type => type;

        /// <summary>
        /// Performs an implicit conversion from <see cref="RqlNothing"/> to <see cref="RqlAny"/>.
        /// </summary>
        /// <param name="rqlNothing">The RQL nothing.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator RqlAny(RqlNothing rqlNothing) => new RqlAny(rqlNothing);

        /// <inheritdoc/>
        public bool Equals(RqlNothing other) => true;

        /// <inheritdoc/>
        public override string ToString()
                    => $"<{this.Type.Name}>";

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is RqlNothing && this.Equals((RqlNothing)obj);

        /// <inheritdoc/>
        public override int GetHashCode() => HashCode.Combine<object>(null);
    }
}