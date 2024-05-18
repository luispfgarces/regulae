namespace Regulae.Rql.Pipeline.Parse.Strategies
{
    using Regulae.Rql.Ast.Expressions;
    using Regulae.Rql.Ast.Segments;
    using Regulae.Rql.Ast.Statements;
    using Regulae.Rql.Pipeline.Parse;

    internal abstract class ParseStrategyBase<TParseOutput> : IParseStrategy<TParseOutput>
    {
        private readonly IParseStrategyProvider parseStrategyProvider;

        protected ParseStrategyBase(IParseStrategyProvider parseStrategyProvider)
        {
            this.parseStrategyProvider = parseStrategyProvider;
        }

        public abstract TParseOutput Parse(ParseContext parseContext);

        protected Expression ParseExpressionWith<TExpressionParseStrategy>(ParseContext parseContext) where TExpressionParseStrategy : IExpressionParseStrategy
            => this.parseStrategyProvider.GetExpressionParseStrategy<TExpressionParseStrategy>().Parse(parseContext);

        protected Segment ParseSegmentWith<TSegmentParseStrategy>(ParseContext parseContext) where TSegmentParseStrategy : ISegmentParseStrategy
            => this.parseStrategyProvider.GetSegmentParseStrategy<TSegmentParseStrategy>().Parse(parseContext);

        protected Statement ParseStatementWith<TStatementParseStrategy>(ParseContext parseContext) where TStatementParseStrategy : IStatementParseStrategy
            => this.parseStrategyProvider.GetStatementParseStrategy<TStatementParseStrategy>().Parse(parseContext);
    }
}