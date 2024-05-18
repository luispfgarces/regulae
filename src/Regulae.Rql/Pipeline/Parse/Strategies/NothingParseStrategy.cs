namespace Regulae.Rql.Pipeline.Parse.Strategies
{
    using System;
    using Regulae.Rql.Ast.Expressions;
    using Regulae.Rql.Pipeline.Parse;
    using Regulae.Rql.Tokens;

    internal class NothingParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        public NothingParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            if (!parseContext.IsMatchCurrentToken(TokenType.NOTHING))
            {
                throw new InvalidOperationException("Unable to handle nothing expression.");
            }

            return this.ParseExpressionWith<LiteralParseStrategy>(parseContext);
        }
    }
}