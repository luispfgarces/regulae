namespace Regulae.Rql.Pipeline.Parse
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Regulae.Rql.Tokens;

    [ExcludeFromCodeCoverage]
    internal readonly struct PanicModeInfo : IEquatable<PanicModeInfo>
    {
        public static readonly PanicModeInfo None = new(causeToken: null!, message: null!);

        public PanicModeInfo(Token causeToken, string message)
        {
            this.CauseToken = causeToken;
            this.Message = message;
        }

        public Token CauseToken { get; }

        public string Message { get; }

        public bool Equals(PanicModeInfo other)
        {
            return this.CauseToken == other.CauseToken && string.Equals(this.Message, other.Message, StringComparison.Ordinal);
        }

        public override bool Equals(object obj) => obj is PanicModeInfo pmi && this.Equals(pmi);

        public override int GetHashCode() => HashCode.Combine(this.CauseToken, this.Message);
    }
}