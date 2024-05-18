namespace Regulae.Rql.Pipeline.Parse.Strategies
{
    using Regulae.Rql.Ast.Expressions;
    using Regulae.Rql.Pipeline.Parse;
    using Regulae.Rql.Tokens;

    internal class BaseExpressionParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        public BaseExpressionParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            var currentToken = parseContext.GetCurrentToken();
            if (parseContext.IsMatchCurrentToken(Constants.AllowedUnescapedIdentifierNames) || currentToken.IsEscaped && !parseContext.IsMatchCurrentToken(Constants.AllowedEscapedIdentifierNames))
            {
                // TODO: logic to be changed to flow first through a indexer parse rule.
                return this.ParseExpressionWith<IdentifierParseStrategy>(parseContext);
            }

            if (parseContext.IsMatchCurrentToken(TokenType.ARRAY, TokenType.BRACE_LEFT))
            {
                return this.ParseExpressionWith<ArrayParseStrategy>(parseContext);
            }

            if (parseContext.IsMatchCurrentToken(TokenType.OBJECT))
            {
                return this.ParseExpressionWith<ObjectParseStrategy>(parseContext);
            }

            if (parseContext.IsMatchCurrentToken(TokenType.NOTHING))
            {
                return this.ParseExpressionWith<NothingParseStrategy>(parseContext);
            }

            if (parseContext.IsMatchCurrentToken(TokenType.STRING, TokenType.INT, TokenType.BOOL, TokenType.DECIMAL, TokenType.DATE))
            {
                return this.ParseExpressionWith<LiteralParseStrategy>(parseContext);
            }

            parseContext.EnterPanicMode("Expected expression.", currentToken);
            return Expression.None;
        }
    }
}