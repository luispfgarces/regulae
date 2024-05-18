namespace Regulae.Rql.Ast.Segments
{
    using System.Diagnostics.CodeAnalysis;
    using Regulae.Rql.Ast.Expressions;

    [ExcludeFromCodeCoverage]
    internal class CardinalitySegment : Segment
    {
        public CardinalitySegment(Expression cardinalityKeyword, Expression ruleKeyword)
            : base(cardinalityKeyword.BeginPosition, ruleKeyword.EndPosition)
        {
            this.CardinalityKeyword = cardinalityKeyword;
            this.RuleKeyword = ruleKeyword;
        }

        public Expression CardinalityKeyword { get; }

        public Expression RuleKeyword { get; }

        public static CardinalitySegment Create(Expression cardinalityKeyword, Expression ruleKeyword)
            => new(cardinalityKeyword, ruleKeyword);

        public override T Accept<T>(ISegmentVisitor<T> visitor) => visitor.VisitCardinalitySegment(this);
    }
}