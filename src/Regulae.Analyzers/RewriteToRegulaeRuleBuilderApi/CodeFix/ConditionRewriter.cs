namespace Regulae.Analyzers.RewriteToRegulaeRuleBuilderApi.CodeFix
{
    using System;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Simplification;

    internal sealed class ConditionRewriter : CSharpSyntaxRewriter
    {
        private readonly SemanticModel semanticModel;
        private SyntaxTriviaList leadingTrivia;
        private static readonly string[] handledConditionsBuilderMethods = ["And", "Or", "Value"];

        public ConditionRewriter(SemanticModel semanticModel, SyntaxTriviaList leadingTrivia)
        {
            this.semanticModel = semanticModel;
            this.leadingTrivia = leadingTrivia;
        }

        public override SyntaxNode VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
        {
            ExpressionSyntax newExpression;
            SyntaxToken newOperator;
            node = (MemberAccessExpressionSyntax)base.VisitMemberAccessExpression(node);
            switch (node.Expression)
            {
                case IdentifierNameSyntax identifierNameOperator
                    when string.Equals(identifierNameOperator.Identifier.Text, "Operators", StringComparison.Ordinal):

                    // Optionally use semanticModel to confirm symbol if available.
                    var rewriteLeft = SyntaxFactory.ParseExpression("global::Regulae.Operators")
                        .WithTriviaFrom(node.Expression)
                        .WithAdditionalAnnotations(Simplifier.Annotation);

                    return node.WithExpression(rewriteLeft);

                case IdentifierNameSyntax identifierNameParameter
                    when this.semanticModel.GetSymbolInfo(identifierNameParameter).Symbol is IParameterSymbol:
                case InvocationExpressionSyntax _
                    when handledConditionsBuilderMethods.Contains(node.Name.Identifier.Text, StringComparer.Ordinal):

                    newExpression = node.Expression.WithTrailingTrivia(SyntaxFactory.ElasticLineFeed);
                    newOperator = node.OperatorToken.WithLeadingTrivia(this.leadingTrivia);
                    return node.WithOperatorToken(newOperator)
                        .WithExpression(newExpression);

                default:
                    return node;
            }
        }

        public override SyntaxNode VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            // Visit the invoked expression first (may be a member access like root.And / b.Value etc).
            var visitedExpression = (ExpressionSyntax)this.Visit(node.Expression);

            // Visit all arguments recursively (this will rewrite Operators.* inside argument expressions)
            var visitedArgumentList = (ArgumentListSyntax)this.Visit(node.ArgumentList);

            // If the invocation carries a lambda argument, ensure the lambda body is rewritten as well.
            // The Visit call above will handle it, but we still return a properly updated node.
            var updated = node.Update(visitedExpression, visitedArgumentList);

            return updated;
        }

        public override SyntaxNode VisitSimpleLambdaExpression(SimpleLambdaExpressionSyntax node)
        {
            this.leadingTrivia = this.leadingTrivia.Add(SyntaxFactory.ElasticTab);
            var newBody = (CSharpSyntaxNode)this.Visit(node.Body);
            this.leadingTrivia = this.leadingTrivia.RemoveAt(this.leadingTrivia.Count - 1);
            return node.WithBody(newBody);
        }

        public override SyntaxNode VisitParenthesizedLambdaExpression(ParenthesizedLambdaExpressionSyntax node)
        {
            this.leadingTrivia = this.leadingTrivia.Add(SyntaxFactory.ElasticTab);
            var newBody = (CSharpSyntaxNode)this.Visit(node.Body);
            this.leadingTrivia = this.leadingTrivia.RemoveAt(this.leadingTrivia.Count - 1);
            return SyntaxFactory.SimpleLambdaExpression(node.ParameterList.Parameters.First(), newBody);
        }
    }
}
