namespace Regulae.Analyzers.RewriteToRegulaeRuleBuilderApi.CodeFix
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class InvocationChainExtractor
    {
        public static (List<InvocationExpressionSyntax> chain, IdentifierNameSyntax chainRoot) ExtractInvocationChain(InvocationExpressionSyntax invocationExpression)
        {
            var chain = new List<InvocationExpressionSyntax>();
            var curr = invocationExpression;
            IdentifierNameSyntax chainRoot = null;
            while (curr != null)
            {
                if (curr.Expression is MemberAccessExpressionSyntax ma)
                {
                    chain.Add(curr);
                    if (ma.Expression is InvocationExpressionSyntax inner)
                    {
                        curr = inner;
                        continue;
                    }
                    if (ma.Expression is IdentifierNameSyntax identifierRoot)
                    {
                        chainRoot = identifierRoot;
                    }
                }
                break;
            }
            return (chain, chainRoot);
        }
    }
}
