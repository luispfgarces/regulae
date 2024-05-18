namespace Regulae.Rql.Pipeline.Interpret
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Regulae.Rql.Ast.Statements;

    internal interface IInterpreter
    {
        Task<InterpretResult> InterpretAsync(IReadOnlyList<Statement> statements);
    }
}