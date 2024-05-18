namespace Regulae.Rql.Pipeline.Parse.Strategies
{
    using Regulae.Rql.Ast.Statements;
    using Regulae.Rql.Pipeline.Parse;
    using Regulae.Rql.Tokens;

    internal class ExpressionStatementParseStrategy : ParseStrategyBase<Statement>, IStatementParseStrategy
    {
        public ExpressionStatementParseStrategy(IParseStrategyProvider parseStrategyProvider)
            : base(parseStrategyProvider)
        {
        }

        public override Statement Parse(ParseContext parseContext)
        {
            var expression = this.ParseExpressionWith<ExpressionParseStrategy>(parseContext);
            if (parseContext.PanicMode)
            {
                parseContext.Synchronize();
                return ExpressionStatement.Create(expression, expression.BeginPosition, parseContext.GetCurrentToken().EndPosition);
            }

            if (!parseContext.MoveNextIfNextToken(TokenType.SEMICOLON))
            {
                parseContext.EnterPanicMode("Expected token ';'.", parseContext.GetNextToken());
                parseContext.Synchronize();
            }

            return ExpressionStatement.Create(expression, expression.BeginPosition, parseContext.GetCurrentToken().EndPosition);
        }
    }
}