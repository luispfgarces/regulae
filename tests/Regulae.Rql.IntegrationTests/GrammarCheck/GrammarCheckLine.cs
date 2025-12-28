namespace Regulae.Rql.IntegrationTests.GrammarCheck
{
    internal sealed class GrammarCheckLine
    {
        public string[]? ExpectedMessages { get; init; }
        public bool? ExpectsSuccess { get; init; }
        public string? Rql { get; init; }
        public string[]? Tags { get; init; }
    }
}