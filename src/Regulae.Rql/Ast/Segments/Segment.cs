namespace Regulae.Rql.Ast.Segments
{
    using System.Diagnostics.CodeAnalysis;
    using Regulae.Rql;
    using Regulae.Rql.Ast;

    [ExcludeFromCodeCoverage]
    internal abstract class Segment : IAstElement
    {
        protected Segment(RqlSourcePosition beginPosition, RqlSourcePosition endPosition)
        {
            this.BeginPosition = beginPosition;
            this.EndPosition = endPosition;
        }

        public static Segment None { get; } = new NoneSegment();

        public RqlSourcePosition BeginPosition { get; }

        public RqlSourcePosition EndPosition { get; }

        public abstract T Accept<T>(ISegmentVisitor<T> visitor);

        public bool ContainsPosition(RqlSourcePosition position)
            => this.BeginPosition <= position && this.EndPosition >= RqlSourcePosition.From(position.Line, position.Column - 1);
    }
}