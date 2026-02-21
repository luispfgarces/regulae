namespace Regulae.Analyzers.RewriteToRegulaeRuleBuilderApi.CodeFix
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Simplification;

    internal sealed class ObsoleteConditionRewriter : CSharpSyntaxRewriter
    {
        private SyntaxTriviaList leadingTrivia;

        public ObsoleteConditionRewriter(SyntaxTriviaList leadingTrivia)
        {
            this.leadingTrivia = leadingTrivia;
        }

        public override SyntaxNode VisitParenthesizedLambdaExpression(ParenthesizedLambdaExpressionSyntax node)
            => this.TranslateLambdaExpression(node, parent: null);

        public override SyntaxNode VisitSimpleLambdaExpression(SimpleLambdaExpressionSyntax node)
            => this.TranslateLambdaExpression(node, parent: null);

        private ExpressionSyntax TranslateLambdaExpression(LambdaExpressionSyntax lambda, ExpressionSyntax parent)
        {
            if (lambda.Body is InvocationExpressionSyntax bodyInvocation)
            {
                var newLambdaParameter = lambda switch
                {
                    ParenthesizedLambdaExpressionSyntax parenthesized => parenthesized.ParameterList.Parameters.First(),
                    SimpleLambdaExpressionSyntax simple => simple.Parameter,
                    _ => throw new NotSupportedException(),
                };

                (var chain, var chainRoot) = InvocationChainExtractor.ExtractInvocationChain(bodyInvocation);

                if (chain.Count == 0)
                {
                    return null;
                }

                chain.Reverse();
                var conditionNodeBeginMethod = chain[0].Expression as MemberAccessExpressionSyntax;
                switch (conditionNodeBeginMethod.Name.Identifier.Text)
                {
                    case "AsComposed":
                        if (parent is null)
                        {
                            this.leadingTrivia = this.leadingTrivia.Add(SyntaxFactory.ElasticTab);
                            var newBody = this.TryTranslateComposed(chain, chainRoot);
                            this.leadingTrivia = this.leadingTrivia.RemoveAt(this.leadingTrivia.Count - 1);
                            return SyntaxFactory.SimpleLambdaExpression(newLambdaParameter, newBody)
                                .WithAdditionalAnnotations(Simplifier.Annotation);
                        }

                        return this.TryTranslateComposed(chain, parent);

                    case "AsValued":
                        if (parent is null)
                        {
                            this.leadingTrivia = this.leadingTrivia.Add(SyntaxFactory.ElasticTab);
                            var newBody = this.TryTranslateValued(chain, chainRoot);
                            this.leadingTrivia = this.leadingTrivia.RemoveAt(this.leadingTrivia.Count - 1);
                            return SyntaxFactory.SimpleLambdaExpression(newLambdaParameter, newBody)
                                .WithAdditionalAnnotations(Simplifier.Annotation);
                        }

                        return this.TryTranslateValued(chain, parent);

                    default:
                        throw new NotSupportedException($"Unsupported condition builder method: {conditionNodeBeginMethod.Name.Identifier.Text}");
                }

            }

            throw new NotSupportedException("Condition builder lamba expressions with body as block expression are not supported.");
        }

        private InvocationExpressionSyntax TryTranslateComposed(List<InvocationExpressionSyntax> chain, ExpressionSyntax parent)
        {
            var memberNames = chain.Select(inv => ((MemberAccessExpressionSyntax)inv.Expression).Name.Identifier.Text).ToArray();
            var idxWithLogicalOperator = Array.FindIndex(memberNames, name => string.Equals(name, "WithLogicalOperator", StringComparison.Ordinal));
            if (idxWithLogicalOperator < 0)
            {
                return null;
            }

            var addConditionInvocations = new List<InvocationExpressionSyntax>();
            for (var i = idxWithLogicalOperator + 1; i < memberNames.Length; i++)
            {
                if (string.Equals(memberNames[i], "AddCondition", StringComparison.Ordinal))
                {
                    addConditionInvocations.Add(chain[i]);
                }
            }

            if (addConditionInvocations.Count == 0)
            {
                return null;
            }

            var withLogicalInvocation = chain[idxWithLogicalOperator];
            var logicalArg = withLogicalInvocation.ArgumentList.Arguments.FirstOrDefault()?.Expression;
            var logicalOperatorName = ExtractSimpleMemberName(logicalArg);

            var firstAdd = addConditionInvocations[0];
            var firstArgLambda = firstAdd.ArgumentList.Arguments.FirstOrDefault()?.Expression as LambdaExpressionSyntax;
            var lambdaParamName = firstArgLambda switch
            {
                ParenthesizedLambdaExpressionSyntax parenthesizedLambdaExpression => parenthesizedLambdaExpression.ParameterList.Parameters.FirstOrDefault()?.Identifier.Text,
                SimpleLambdaExpressionSyntax simpleLambdaExpression => simpleLambdaExpression.Parameter.Identifier.Text,
                _ => throw new NotSupportedException("Unsupported lambda expression type in AddCondition argument."),
            };

            ExpressionSyntax bodyExpression = SyntaxFactory.IdentifierName(lambdaParamName);
            this.leadingTrivia = this.leadingTrivia.Add(SyntaxFactory.ElasticTab);
            foreach (var addInv in addConditionInvocations)
            {
                if (addInv.ArgumentList.Arguments[0].Expression is not LambdaExpressionSyntax lambda)
                {
                    throw new NotSupportedException();
                }

                bodyExpression = this.TranslateLambdaExpression(lambda, bodyExpression);
            }

            this.leadingTrivia = this.leadingTrivia.RemoveAt(this.leadingTrivia.Count - 1);
            var parameter = SyntaxFactory.Parameter(SyntaxFactory.Identifier(lambdaParamName));
            var lambdaExpr = SyntaxFactory.SimpleLambdaExpression(parameter, bodyExpression)
                .WithAdditionalAnnotations(Simplifier.Annotation);
            var composedAccess = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, parent, SyntaxFactory.IdentifierName(logicalOperatorName));
            var newComposedAccessExpression = composedAccess.Expression.WithTrailingTrivia(SyntaxFactory.ElasticLineFeed);
            var newComposedAccessOperatorToken = composedAccess.OperatorToken.WithLeadingTrivia(this.leadingTrivia);
            composedAccess = composedAccess.WithExpression(newComposedAccessExpression).WithOperatorToken(newComposedAccessOperatorToken);
            return SyntaxFactory.InvocationExpression(composedAccess, SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(lambdaExpr))))
                .WithLeadingTrivia(SyntaxFactory.ElasticLineFeed)
                .WithAdditionalAnnotations(Simplifier.Annotation);
        }

        private InvocationExpressionSyntax TryTranslateValued(IReadOnlyList<InvocationExpressionSyntax> chain, ExpressionSyntax parent)
        {
            InvocationExpressionSyntax asValued = null;
            InvocationExpressionSyntax withComparison = null;
            InvocationExpressionSyntax setOperand = null;

            foreach (var inv in chain)
            {
                if (inv.Expression is MemberAccessExpressionSyntax ma)
                {
                    var name = ma.Name.Identifier.Text;
                    if (string.Equals(name, "AsValued", StringComparison.Ordinal))
                    {
                        asValued = inv;
                    }
                    else if (string.Equals(name, "WithComparisonOperator", StringComparison.Ordinal))
                    {
                        withComparison = inv;
                    }
                    else if (string.Equals(name, "SetOperand", StringComparison.Ordinal))
                    {
                        setOperand = inv;
                    }
                }
            }

            var conditionKey = asValued.ArgumentList.Arguments[0].Expression;
            var operatorExpression = withComparison?.ArgumentList.Arguments[0].Expression;
            var operandExpression = setOperand?.ArgumentList.Arguments[0].Expression;

            var valueAccess = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, parent, SyntaxFactory.IdentifierName("Value"));
            var newValueAccessExpression = valueAccess.Expression.WithTrailingTrivia(SyntaxFactory.ElasticLineFeed);
            var newValueAccessOperatorToken = valueAccess.OperatorToken.WithLeadingTrivia(this.leadingTrivia);
            valueAccess = valueAccess.WithExpression(newValueAccessExpression).WithOperatorToken(newValueAccessOperatorToken);
            var argList = SyntaxFactory.ArgumentList(
                SyntaxFactory.SeparatedList(
                [
                    SyntaxFactory.Argument(conditionKey),
                    SyntaxFactory.Argument(MapToRegulaeOperatorsMember(operatorExpression)),
                    SyntaxFactory.Argument(operandExpression),
                ]));
            return SyntaxFactory.InvocationExpression(valueAccess, argList);
        }

        private static string ExtractSimpleMemberName(ExpressionSyntax expr)
        {
            if (expr is MemberAccessExpressionSyntax ma)
            {
                return ma.Name.Identifier.Text;
            }

            return null;
        }

        private static MemberAccessExpressionSyntax MapToRegulaeOperatorsMember(ExpressionSyntax originalOperatorExpr)
        {
            var opName = ExtractSimpleMemberName(originalOperatorExpr) ?? originalOperatorExpr.ToString();
            var left = SyntaxFactory.ParseExpression("global::Regulae.Operators");
            return SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, left, SyntaxFactory.IdentifierName(opName));
        }
    }
}