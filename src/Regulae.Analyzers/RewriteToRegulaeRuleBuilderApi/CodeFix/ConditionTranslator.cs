namespace Regulae.Analyzers.RewriteToRegulaeRuleBuilderApi.CodeFix
{
    using System;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class ConditionTranslator
    {
        private static readonly string[] legacyNames = ["AsComposed", "AsValued"];

        public static LambdaExpressionSyntax TranslateLambda(LambdaExpressionSyntax lambda, SemanticModel semanticModel, SyntaxTriviaList leadingTrivia)
        {

            var bodyIdentifiers = lambda.DescendantNodes()
                .OfType<IdentifierNameSyntax>()
                .Select(id => id.Identifier.Text);

            var useObsolete = bodyIdentifiers.Intersect(legacyNames, StringComparer.Ordinal).Any();

            if (useObsolete)
            {
                var rewriter = new ObsoleteConditionRewriter(leadingTrivia);
                return (LambdaExpressionSyntax)rewriter.Visit(lambda);
            }
            else
            {
                var rewriter = new ConditionRewriter(semanticModel, leadingTrivia);
                return (LambdaExpressionSyntax)rewriter.Visit(lambda);
            }
        }

        public static bool TryExtractValueConditionArguments(LambdaExpressionSyntax lambda, out ArgumentListSyntax argumentList)
        {
            if (lambda.Body is InvocationExpressionSyntax invocationExpression
                && invocationExpression.Expression is MemberAccessExpressionSyntax memberAccessExpression
                && string.Equals(memberAccessExpression.Name.Identifier.Text, "Value", StringComparison.Ordinal))
            {
                argumentList = invocationExpression.ArgumentList;
                return true;
            }

            argumentList = null!;
            return false;
        }
    }
}