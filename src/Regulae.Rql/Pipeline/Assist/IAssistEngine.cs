namespace Regulae.Rql.Pipeline.Assist
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Regulae.Rql;
    using Regulae.Rql.Ast.Statements;
    using Regulae.Rql.Tokens;

    internal interface IAssistEngine
    {
        Task<IReadOnlyList<IAssistSuggestion>> ProcessAssistAsync(
            IReadOnlyList<Token> tokens,
            IReadOnlyList<Statement> statements,
            RqlSourcePosition position);
    }
}