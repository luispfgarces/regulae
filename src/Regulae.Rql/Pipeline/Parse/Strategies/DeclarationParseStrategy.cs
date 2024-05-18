namespace Regulae.Rql.Pipeline.Parse.Strategies
{
    using Regulae.Rql.Ast.Statements;
    using Regulae.Rql.Pipeline.Parse;

    internal class DeclarationParseStrategy : ParseStrategyBase<Statement>, IStatementParseStrategy
    {
        public DeclarationParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Statement Parse(ParseContext parseContext)
        {
            // TODO: future logic to be added here for dealing with variables.

            return this.ParseStatementWith<StatementParseStrategy>(parseContext);
        }
    }
}