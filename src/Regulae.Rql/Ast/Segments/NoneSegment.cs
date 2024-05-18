namespace Regulae.Rql.Ast.Segments
{
    using System.Diagnostics.CodeAnalysis;
    using Regulae.Rql;

    [ExcludeFromCodeCoverage]
    internal class NoneSegment : Segment
    {
        public NoneSegment()
            : base(RqlSourcePosition.Empty, RqlSourcePosition.Empty)
        {
        }

        public override T Accept<T>(ISegmentVisitor<T> visitor) => visitor.VisitNoneSegment(this);
    }
}