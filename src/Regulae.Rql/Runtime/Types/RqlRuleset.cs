namespace Regulae.Rql.Runtime.Types
{
    using System;
    using System.Diagnostics;
    using Regulae;
    using Regulae.Rql.Runtime;

    /// <summary>
    /// Defines the .NET representation of a RQL &lt;ruleset&gt; value.
    /// </summary>
    [DebuggerDisplay("<{this.Type.Name,nq}> {this.Value.Name,nq}")]
    public readonly struct RqlRuleset : IRuntimeValue, IEquatable<RqlRuleset>
    {
        private static readonly Type runtimeType = typeof(Ruleset);

        /// <summary>
        /// Initializes a new instance of the <see cref="RqlRuleset"/> struct.
        /// </summary>
        /// <param name="ruleset">The ruleset.</param>
        public RqlRuleset(Ruleset ruleset)
        {
            this.Value = ruleset;
        }

        /// <inheritdoc/>
        public Type RuntimeType => runtimeType;

        /// <inheritdoc/>
        public object RuntimeValue => this.Value;

        /// <inheritdoc/>
        public RqlType Type => RqlTypes.Ruleset;

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public readonly Ruleset Value { get; }

        /// <summary>
        /// Performs an implicit conversion from <see cref="RqlRuleset"/> to <see cref="RqlAny"/>.
        /// </summary>
        /// <param name="rqlRuleset">The RQL ruleset.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator RqlAny(RqlRuleset rqlRuleset) => new RqlAny(rqlRuleset);

        /// <inheritdoc/>
        public bool Equals(RqlRuleset other) => this.Value.Equals(other.Value);

        /// <inheritdoc/>
        public override string ToString() => $"<{this.Type.Name}> {this.Value.Name}";

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is RqlRuleset ruleset && this.Equals(ruleset);

        /// <inheritdoc/>
        public override int GetHashCode() => this.Value.GetHashCode();
    }
}