namespace Regulae.Rql.Pipeline.Parse.Strategies
{
    using Regulae.Rql.Ast.Expressions;
    using Regulae.Rql.Pipeline.Parse;

    internal class AssignmentParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        public AssignmentParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            return this.ParseExpressionWith<RulesManipulationParseStrategy>(parseContext);

            // TODO: future logic to be added here for dealing with assignment of variables.
        }
    }
}