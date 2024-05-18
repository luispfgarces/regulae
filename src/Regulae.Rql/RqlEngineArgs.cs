namespace Regulae.Rql
{
    using System.Diagnostics.CodeAnalysis;
    using Regulae.Rql.Pipeline.Assist;
    using Regulae.Rql.Pipeline.Interpret;
    using Regulae.Rql.Pipeline.Parse;
    using Regulae.Rql.Pipeline.Scan;

    [ExcludeFromCodeCoverage]
    internal class RqlEngineArgs
    {
        public IAssistEngine AssistEngine { get; set; }

        public IInterpreter Interpreter { get; set; }

        public RqlOptions Options { get; set; }

        public IParser Parser { get; set; }

        public ITokenScanner TokenScanner { get; set; }
    }
}