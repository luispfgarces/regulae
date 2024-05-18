namespace Regulae.Rql.Pipeline.Parse.Strategies
{
    using Regulae.Rql.Ast.Expressions;
    using Regulae.Rql.Pipeline.Parse;
    using Regulae.Rql.Tokens;

    internal class UnaryParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        public UnaryParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            if (parseContext.IsMatchCurrentToken(TokenType.MINUS))
            {
                var @operator = parseContext.GetCurrentToken();
                _ = parseContext.MoveNext();
                var right = this.ParseExpressionWith<UnaryParseStrategy>(parseContext);
                return new UnaryExpression(@operator, right);
            }

            return this.ParseExpressionWith<BaseExpressionParseStrategy>(parseContext);
        }
    }
}