namespace Regulae.Analyzers.RewriteToRegulaeRuleAddPriorityOption
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class CodeFixEvaluator
    {
        public static bool CanFixNode(SyntaxNode node)
        {
            if (node is ArgumentSyntax argumentSyntax)
            {
                // Unwrap the argument to get to the invocation expression in cases where the fluent chain is passed as an argument to a method.
                node = argumentSyntax.Expression;
            }

            return node is InvocationExpressionSyntax or MemberAccessExpressionSyntax;
        }
    }
}
