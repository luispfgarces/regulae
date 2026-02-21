namespace Regulae.Analyzers.RewriteToRegulaeRuleBuilderApi.CodeFix
{
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal sealed class ExtractedValueConditionNode
    {
        public ExpressionSyntax ConditionKey { get; set; }
        public ExpressionSyntax OperatorExpression { get; set; }
        public ExpressionSyntax Operand { get; set; }
    }
}
