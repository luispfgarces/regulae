namespace Regulae.Rql.IntegrationTests.AssistCheck
{
    internal sealed class AssistCheckLine
    {
        public uint Column { get; init; }

        public string[]? ExpectedAssistSuggestions { get; init; }

        public uint Line { get; init; }

        public string? Rql { get; init; }
    }
}
