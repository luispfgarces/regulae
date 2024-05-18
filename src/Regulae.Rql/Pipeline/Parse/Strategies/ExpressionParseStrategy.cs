namespace Regulae.Rql.Pipeline.Parse.Strategies
{
    using Regulae.Rql.Ast.Expressions;
    using Regulae.Rql.Pipeline.Parse;

    internal class ExpressionParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        public ExpressionParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            return this.ParseExpressionWith<AssignmentParseStrategy>(parseContext);
        }
    }
}