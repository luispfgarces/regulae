namespace Regulae.Rql.Pipeline.Parse.Strategies
{
    using Regulae.Rql.Ast.Expressions;
    using Regulae.Rql.Pipeline.Parse;

    internal class IdentifierParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        public IdentifierParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            var identifierToken = parseContext.GetCurrentToken();
            return new IdentifierExpression(identifierToken);
        }
    }
}