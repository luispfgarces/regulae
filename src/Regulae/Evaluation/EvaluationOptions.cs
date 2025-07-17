namespace Regulae.Evaluation
{
    using System;

    internal sealed class EvaluationOptions : IEquatable<EvaluationOptions>
    {
        public bool ExcludeRulesWithoutSearchConditions { get; set; }

        public MatchModes MatchMode { get; set; }

        public static bool operator !=(EvaluationOptions left, EvaluationOptions right) => !(left == right);

        public static bool operator ==(EvaluationOptions left, EvaluationOptions right) => left.Equals(right);

        public override bool Equals(object obj)
                            => obj is EvaluationOptions options && this.Equals(options);

        public bool Equals(EvaluationOptions other)
            => this.ExcludeRulesWithoutSearchConditions == other.ExcludeRulesWithoutSearchConditions
                && this.MatchMode == other.MatchMode;

        public override int GetHashCode()
            => HashCode.Combine(this.ExcludeRulesWithoutSearchConditions, this.MatchMode);
    }
}