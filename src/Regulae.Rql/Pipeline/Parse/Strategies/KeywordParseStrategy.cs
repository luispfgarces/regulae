namespace Regulae.Rql.Pipeline.Parse.Strategies
{
    using Regulae.Rql.Ast.Expressions;
    using Regulae.Rql.Pipeline.Parse;

    internal class KeywordParseStrategy : ParseStrategyBase<Expression>, IExpressionParseStrategy
    {
        public KeywordParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Expression Parse(ParseContext parseContext)
        {
            var keywordToken = parseContext.GetCurrentToken();
            return KeywordExpression.Create(keywordToken);
        }
    }
}