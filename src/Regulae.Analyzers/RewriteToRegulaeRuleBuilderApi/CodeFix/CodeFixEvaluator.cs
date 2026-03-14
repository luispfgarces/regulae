namespace Regulae.Analyzers.RewriteToRegulaeRuleBuilderApi.CodeFix
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class CodeFixEvaluator
    {
        public static bool CanFixNode(SyntaxNode node, SemanticModel semanticModel)
        {
            if (node is ArgumentSyntax argumentSyntax)
            {
                // Unwrap the argument to get to the invocation expression in cases where the fluent chain is passed as an argument to a method.
                node = argumentSyntax.Expression;
            }

            if (node is InvocationExpressionSyntax invocationExpression)
            {
                (var chain, var chainRoot) = InvocationChainExtractor.ExtractInvocationChain(invocationExpression);
                var chainMethods = chain.Select(x => ((MemberAccessExpressionSyntax)x.Expression).Name.Identifier.Text).ToArray();
                if (!RulesFrameworkConstants.RuleBuilderMinimumRequiredMethodPrefixes.All(prefix => chainMethods.Any(method => method.StartsWith(prefix, StringComparison.Ordinal))))
                {
                    // The minimum viable is to have at least one instance of each method in the fluent chain.
                    // If that's not the case, this code fix provider does not support fix.
                    return false;
                }

                if (chainMethods.Any(method => !RulesFrameworkConstants.RuleBuilderAllSupportedMethods.Contains(method, StringComparer.Ordinal)))
                {
                    // If a unsupported method is found in the fluent chain, code fix provider is not able to fix it as well.
                    return false;
                }

                foreach (var chainedMethod in chainMethods)
                {
                    var chainedMethodInvocationExpression = chain
                        .Find(invocationExpression => string.Equals(((MemberAccessExpressionSyntax)invocationExpression.Expression).Name.Identifier.Text, chainedMethod, StringComparison.Ordinal));
                    var chainedMethodInfo = (IMethodSymbol)semanticModel.GetSymbolInfo(chainedMethodInvocationExpression).Symbol;

                    if (!RulesFrameworkConstants.RuleBuilderTypes.Contains(chainedMethodInfo.ContainingType.OriginalDefinition.ToString(), StringComparer.Ordinal))
                    {
                        // If any of the methods in the chain does not resolve to a method on IRuleBuilder, we cannot be sure this is a supported fluent chain and thus we won't offer a fix.
                        return false;
                    }

                    if (string.Equals(chainedMethod, RulesFrameworkConstants.WithCondition, StringComparison.Ordinal))
                    {
                        var withConditionArguments = chainedMethodInvocationExpression.ArgumentList.Arguments;
                        if (withConditionArguments.Count == 1)
                        {
                            if (!RulesFrameworkConstants.SupportedWithConditionParameterTypesForArityOne.Contains(chainedMethodInfo.Parameters[0].OriginalDefinition.Type.ToString(), StringComparer.Ordinal))
                            {
                                return false;
                            }

                            var lambdaArgument = (LambdaExpressionSyntax)withConditionArguments[0].Expression;
                            if (lambdaArgument.Body is not InvocationExpressionSyntax conditionInvocationExpression)
                            {
                                // We only support lambdas that directly invoke condition builder methods, e.g. .WithCondition(root => root.And(...))
                                return false;
                            }

                            if (!CanFixConditionNode(conditionInvocationExpression, semanticModel))
                            {
                                // If the lambda body does not resolve to a supported condition builder method, we won't offer a fix.
                                return false;
                            }
                        }
                    }
                }

                return true;
            }

            return false;
        }

        private static bool CanFixConditionNode(InvocationExpressionSyntax node, SemanticModel semanticModel)
        {
            (var chain, var chainRoot) = InvocationChainExtractor.ExtractInvocationChain(node);
            var chainMethods = chain.Select(x => ((MemberAccessExpressionSyntax)x.Expression).Name.Identifier.Text).ToArray();
            var firstChainMethod = chainMethods.Reverse().FirstOrDefault();
            switch (firstChainMethod)
            {
                case RulesFrameworkConstants.And:
                case RulesFrameworkConstants.Or:
                case RulesFrameworkConstants.Value:
                    if (!CanFixFluentConditionInvocation(semanticModel, chain, chainMethods))
                    {
                        return false;
                    }
                    break;
                case RulesFrameworkConstants.AsComposed:
                    if (!CanFixAsComposedConditionInvocation(semanticModel, chain, chainMethods))
                    {
                        return false;
                    }

                    break;
                case RulesFrameworkConstants.AsValued:
                    if (!CanFixAsValuedConditionInvocation(semanticModel, chain, chainMethods))
                    {
                        return false;
                    }

                    break;
                default:
                    break;
            }

            return true;
        }

        private static bool CanFixAsValuedConditionInvocation(SemanticModel semanticModel, List<InvocationExpressionSyntax> chain, string[] chainMethods)
        {
            InvocationExpressionSyntax chainedMethodInvocationExpression;
            IMethodSymbol chainedMethodSymbol;
            foreach (var chainedMethod in chainMethods)
            {
                if (!RulesFrameworkConstants.AsValuedMinimumRequiredMethodPrefixes.Contains(chainedMethod, StringComparer.Ordinal))
                {
                    return false;
                }

                chainedMethodInvocationExpression = chain
                    .Find(invocationExpression => string.Equals(((MemberAccessExpressionSyntax)invocationExpression.Expression).Name.Identifier.Text, chainedMethod, StringComparison.Ordinal));
                chainedMethodSymbol = (IMethodSymbol)semanticModel.GetSymbolInfo(chainedMethodInvocationExpression).Symbol;
                if (!RulesFrameworkConstants.ConditionBuilderTypes.Contains(chainedMethodSymbol.ContainingType.OriginalDefinition.ToString(), StringComparer.Ordinal))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool CanFixAsComposedConditionInvocation(SemanticModel semanticModel, List<InvocationExpressionSyntax> chain, string[] chainMethods)
        {
            InvocationExpressionSyntax chainedMethodInvocationExpression;
            IMethodSymbol chainedMethodSymbol;
            foreach (var chainedMethod in chainMethods)
            {
                if (!RulesFrameworkConstants.AsComposedMinimumRequiredMethodPrefixes.Contains(chainedMethod, StringComparer.Ordinal))
                {
                    return false;
                }

                chainedMethodInvocationExpression = chain
                    .Find(invocationExpression => string.Equals(((MemberAccessExpressionSyntax)invocationExpression.Expression).Name.Identifier.Text, chainedMethod, StringComparison.Ordinal));
                chainedMethodSymbol = (IMethodSymbol)semanticModel.GetSymbolInfo(chainedMethodInvocationExpression).Symbol;
                if (!RulesFrameworkConstants.ConditionBuilderTypes.Contains(chainedMethodSymbol.ContainingType.OriginalDefinition.ToString(), StringComparer.Ordinal))
                {
                    return false;
                }

                if (string.Equals(chainedMethod, RulesFrameworkConstants.AddCondition, StringComparison.Ordinal))
                {
                    var lambdaArgument = (LambdaExpressionSyntax)chainedMethodInvocationExpression.ArgumentList.Arguments[0].Expression;
                    if (lambdaArgument.Body is not InvocationExpressionSyntax conditionInvocationExpression)
                    {
                        // We only support lambdas that directly invoke condition builder methods, e.g. .WithCondition(root => root.And(...))
                        return false;
                    }

                    if (!CanFixConditionNode(conditionInvocationExpression, semanticModel))
                    {
                        // If the lambda body does not resolve to a supported condition builder method, we won't offer a fix.
                        return false;
                    }
                }
            }

            return true;
        }

        private static bool CanFixFluentConditionInvocation(SemanticModel semanticModel, List<InvocationExpressionSyntax> chain, string[] chainMethods)
        {
            InvocationExpressionSyntax chainedMethodInvocationExpression;
            IMethodSymbol chainedMethodSymbol;
            foreach (var chainedMethod in chainMethods)
            {
                if (!RulesFrameworkConstants.FluentConditionBuilderSupportedMethods.Contains(chainedMethod, StringComparer.Ordinal))
                {
                    return false;
                }

                chainedMethodInvocationExpression = chain
                    .Find(invocationExpression => string.Equals(((MemberAccessExpressionSyntax)invocationExpression.Expression).Name.Identifier.Text, chainedMethod, StringComparison.Ordinal));
                chainedMethodSymbol = (IMethodSymbol)semanticModel.GetSymbolInfo(chainedMethodInvocationExpression).Symbol;
                if (!RulesFrameworkConstants.ConditionBuilderTypes.Contains(chainedMethodSymbol.ContainingType.OriginalDefinition.ToString(), StringComparer.Ordinal))
                {
                    return false;
                }

                if (RulesFrameworkConstants.FluentConditionBuilderComposedConditionMethods.Contains(chainedMethod, StringComparer.Ordinal))
                {
                    var lambdaArgument = (LambdaExpressionSyntax)chainedMethodInvocationExpression.ArgumentList.Arguments[0].Expression;
                    if (lambdaArgument.Body is not InvocationExpressionSyntax conditionInvocationExpression)
                    {
                        // We only support lambdas that directly invoke condition builder methods, e.g. .WithCondition(root => root.And(...))
                        return false;
                    }

                    if (!CanFixConditionNode(conditionInvocationExpression, semanticModel))
                    {
                        // If the lambda body does not resolve to a supported condition builder method, we won't offer a fix.
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
