namespace Regulae.Rql.Ast.Expressions
{
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Regulae.Rql.Tokens;

    [ExcludeFromCodeCoverage]
    internal class NewObjectExpression : Expression
    {
        public NewObjectExpression(Token @object, Expression[] propertyAssignements)
            : base(@object.BeginPosition, propertyAssignements.LastOrDefault()?.EndPosition ?? @object.EndPosition)
        {
            this.Object = @object;
            this.PropertyAssignments = propertyAssignements;
        }

        public Token Object { get; }

        public Expression[] PropertyAssignments { get; }

        public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitNewObjectExpression(this);
    }
}