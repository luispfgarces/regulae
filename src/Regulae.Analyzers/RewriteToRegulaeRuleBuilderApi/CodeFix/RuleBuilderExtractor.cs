namespace Regulae.Analyzers.RewriteToRegulaeRuleBuilderApi.CodeFix
{
    using System.Threading;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class RuleBuilderExtractor
    {
        public static RuleBuilderParameters ExtractOriginalFluentChain(SyntaxNode originalSyntaxNode, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            var ruleBuilderParameters = new RuleBuilderParameters();
            var currentSyntaxNode = originalSyntaxNode;

            while (currentSyntaxNode is InvocationExpressionSyntax invocationExpression)
            {
                if (semanticModel.GetSymbolInfo(invocationExpression, cancellationToken).Symbol is not IMethodSymbol methodSymbol)
                {
                    break;
                }

                var args = invocationExpression.ArgumentList.Arguments;
                switch (methodSymbol.Name)
                {
                    case "Build":
                        break;

                    case "WithActive":
                        HandleWithActive(ruleBuilderParameters, args);
                        break;

                    case "WithContent":
                        HandleWithContent(ruleBuilderParameters, args);
                        break;

                    case "WithName":
                        HandleWithName(ruleBuilderParameters, args);
                        break;

                    case "WithDateBegin":
                        HandleWithDateBegin(ruleBuilderParameters, args);
                        break;

                    case "WithDatesInterval":
                        HandleWithDatesInterval(ruleBuilderParameters, args);
                        break;

                    case "WithCondition":
                        HandleWithCondition(ruleBuilderParameters, args);
                        break;

                    case "NewRule":
                        HandleNewRule(ruleBuilderParameters, methodSymbol);
                        break;
                }

                var memberAccess = invocationExpression.Expression as MemberAccessExpressionSyntax;
                if (memberAccess?.Expression is InvocationExpressionSyntax innerInvocation)
                {
                    currentSyntaxNode = innerInvocation;
                }
                else
                {
                    break;
                }
            }

            return ruleBuilderParameters;
        }

        private static void HandleNewRule(RuleBuilderParameters ruleBuilderParameters, IMethodSymbol methodSymbol)
        {
            if (methodSymbol.TypeArguments.Length >= 2)
            {
                var rulesetTypeSymbol = methodSymbol.TypeArguments[0];
                var conditionTypeSymbol = methodSymbol.TypeArguments[1];

                if (rulesetTypeSymbol != null)
                {
                    var rulesetName = rulesetTypeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
                    ruleBuilderParameters.TypeParameters.RulesetType = SyntaxFactory.ParseTypeName(rulesetName);
                }

                if (conditionTypeSymbol != null)
                {
                    var conditionName = conditionTypeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
                    ruleBuilderParameters.TypeParameters.ConditionType = SyntaxFactory.ParseTypeName(conditionName);
                }
            }
        }

        private static void HandleWithCondition(RuleBuilderParameters ruleBuilderParameters, SeparatedSyntaxList<ArgumentSyntax> args)
        {
            if (args.Count == 1)
            {
                var argExpr = args[0].Expression;
                if (argExpr is SimpleLambdaExpressionSyntax || argExpr is ParenthesizedLambdaExpressionSyntax)
                {
                    ruleBuilderParameters.Condition.ConditionLambda = (LambdaExpressionSyntax)argExpr;
                }
                else
                {
                    ruleBuilderParameters.Condition.RawConditionExpression = argExpr;
                }
            }
            else if (args.Count == 3)
            {
                var conditionKeyExpr = args[0].Expression;
                var operatorExpr = args[1].Expression;
                var operandExpr = args[2].Expression;

                ruleBuilderParameters.Condition.SimpleValueCondition = new ExtractedValueConditionNode
                {
                    ConditionKey = conditionKeyExpr,
                    OperatorExpression = operatorExpr,
                    Operand = operandExpr,
                };
            }
            else
            {
                var any = args.FirstOrDefault();
                if (any != null)
                {
                    ruleBuilderParameters.Condition.RawConditionExpression = any.Expression;
                }
            }
        }

        private static void HandleWithDatesInterval(RuleBuilderParameters ruleBuilderParameters, SeparatedSyntaxList<ArgumentSyntax> args)
        {
            if (ruleBuilderParameters.DateBegin is not null || ruleBuilderParameters.DateEnd is not null)
            {
                return;
            }

            if (args.Count >= 2)
            {
                ruleBuilderParameters.DateBegin = args[0].Expression;
                ruleBuilderParameters.DateEnd = args[1].Expression;
            }
        }

        private static void HandleWithDateBegin(RuleBuilderParameters ruleBuilderParameters, SeparatedSyntaxList<ArgumentSyntax> args)
        {
            if (ruleBuilderParameters.DateBegin is not null)
            {
                return;
            }

            var dbArg = args.FirstOrDefault();
            if (dbArg != null)
            {
                ruleBuilderParameters.DateBegin = dbArg.Expression;
            }
        }

        private static void HandleWithName(RuleBuilderParameters ruleBuilderParameters, SeparatedSyntaxList<ArgumentSyntax> args)
        {
            if (ruleBuilderParameters.Name is not null)
            {
                return;
            }

            var nameArgument = args.FirstOrDefault();
            if (nameArgument != null)
            {
                ruleBuilderParameters.Name = nameArgument.Expression;
            }
        }

        private static void HandleWithContent(RuleBuilderParameters ruleBuilderParameters, SeparatedSyntaxList<ArgumentSyntax> args)
        {
            if (ruleBuilderParameters.Content is not null)
            {
                return;
            }

            if (args.Count >= 2)
            {
                ruleBuilderParameters.Ruleset = args[0].Expression;
                ruleBuilderParameters.Content = args[1].Expression;
            }
        }

        private static void HandleWithActive(RuleBuilderParameters ruleBuilderParameters, SeparatedSyntaxList<ArgumentSyntax> args)
        {
            if (ruleBuilderParameters.Active is not null)
            {
                return;
            }

            var activeArgument = args.FirstOrDefault();
            if (activeArgument != null)
            {
                ruleBuilderParameters.Active = activeArgument.Expression;
            }
        }
    }
}
