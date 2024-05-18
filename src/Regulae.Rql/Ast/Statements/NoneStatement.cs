namespace Regulae.Rql.Ast.Statements
{
    using System.Diagnostics.CodeAnalysis;
    using Regulae.Rql;

    [ExcludeFromCodeCoverage]
    internal class NoneStatement : Statement
    {
        public NoneStatement()
            : base(RqlSourcePosition.Empty, RqlSourcePosition.Empty)
        {
        }

        public override T Accept<T>(IStatementVisitor<T> visitor) => visitor.VisitNoneStatement(this);
    }
}