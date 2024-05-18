namespace Regulae.Rql.Pipeline.Parse.Strategies
{
    using Regulae.Rql.Ast.Expressions;
    using Regulae.Rql.Pipeline.Parse;
    using Regulae.Rql.Tokens;

    internal class TermParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        public TermParseStrategy(IParseStrategyProvider parseStrategyProvider) : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            var unaryExpression = this.ParseExpressionWith<FactorParseStrategy>(parseContext);

            if (parseContext.MoveNextIfNextToken(TokenType.PLUS, TokenType.MINUS))
            {
                var rightExpression = Expression.None;
                var operatorSegment = this.ParseSegmentWith<OperatorParseStrategy>(parseContext);
                if (parseContext.PanicMode)
                {
                    return new BinaryExpression(unaryExpression, operatorSegment, rightExpression);
                }

                _ = parseContext.MoveNext();
                rightExpression = this.ParseExpressionWith<FactorParseStrategy>(parseContext);
                return new BinaryExpression(unaryExpression, operatorSegment, rightExpression);
            }

            return unaryExpression;
        }
    }
}