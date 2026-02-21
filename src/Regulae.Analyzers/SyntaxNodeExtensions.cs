namespace Regulae.Analyzers
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class SyntaxNodeExtensions
    {
        public static MemberAccessExpressionSyntax ApplyFluentChainFormatting(
            this MemberAccessExpressionSyntax memberAccessExpression,
            SyntaxTriviaList? leadingTrivia = null)
        {
            var newLeadingTrivia = leadingTrivia ?? [];
            var newOperatorToken = memberAccessExpression.OperatorToken.WithLeadingTrivia(newLeadingTrivia);
            return memberAccessExpression.WithOperatorToken(newOperatorToken);
        }

        public static InvocationExpressionSyntax ApplyFluentChainFormatting(
            this InvocationExpressionSyntax invocationExpression,
            SyntaxTriviaList? trailingTrivia = null)
        {
            var newTrailingTrivia = trailingTrivia ?? [];
            return invocationExpression.WithTrailingTrivia(newTrailingTrivia);
        }
    }
}
