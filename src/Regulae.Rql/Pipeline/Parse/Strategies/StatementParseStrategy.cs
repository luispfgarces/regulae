namespace Regulae.Rql.Pipeline.Parse.Strategies
{
    using Regulae.Rql.Ast.Statements;
    using Regulae.Rql.Pipeline.Parse;

    internal class StatementParseStrategy : ParseStrategyBase<Statement>, IStatementParseStrategy
    {
        public StatementParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Statement Parse(ParseContext parseContext)
        {
            // TODO: future logic to be added here for dealing with if, foreach, and block statements.

            return this.ParseStatementWith<ExpressionStatementParseStrategy>(parseContext);
        }
    }
}