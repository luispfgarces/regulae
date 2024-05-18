namespace Regulae.Rql.Tests.Pipeline.Parse
{
    using System;
    using Regulae.Rql.Ast.Expressions;
    using Regulae.Rql.Ast.Segments;
    using Regulae.Rql.Ast.Statements;
    using Regulae.Rql.Pipeline.Parse;

    internal class StubParseStrategy : IExpressionParseStrategy, ISegmentParseStrategy, IStatementParseStrategy
    {
        public StubParseStrategy(IParseStrategyProvider parseStrategyProvider)
        {
            this.CreationDateTime = DateTime.UtcNow;
            this.ParseStrategyProvider = parseStrategyProvider;
        }

        public DateTime CreationDateTime { get; }
        public IParseStrategyProvider ParseStrategyProvider { get; }

        Expression IParseStrategy<Expression>.Parse(ParseContext parseContext) => throw new NotImplementedException("Implementation not needed for testing");

        Segment IParseStrategy<Segment>.Parse(ParseContext parseContext) => throw new NotImplementedException("Implementation not needed for testing");

        Statement IParseStrategy<Statement>.Parse(ParseContext parseContext) => throw new NotImplementedException("Implementation not needed for testing");
    }
}