namespace Regulae.Analyzers.RewriteToRegulaeRuleBuilderApi
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class RulesFrameworkAnalyzer : DiagnosticAnalyzer
    {
        public static readonly string[] RuleBuilderApiTypes = ["RuleBuilder", "IRuleBuilder"];

        private static readonly LocalizableString Title = "Rules.Framework rule builder API usage detected";
        private static readonly LocalizableString MessageFormat = "Rule builder from 'Rules.Framework' detected. Consider rewriting to Regulae API.";
        private static readonly LocalizableString Description = "Detects usages of Rules.Framework rule builder API to help automated migration to Regulae rule builder API.";
        private const string Category = "Migration";

        private static readonly DiagnosticDescriptor Rule = new(
            RegulaeDiagnostics.RewriteToRegulaeRuleBuilderApiId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Info,
            isEnabledByDefault: true,
            description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterCompilationStartAction(compilationStartContext =>
            {
                compilationStartContext.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
            });
        }

        private static void AnalyzeRuleBuilderApiUsage(SyntaxNodeAnalysisContext context, SyntaxNode initialSyntaxNode, SyntaxNode currentSyntaxNode)
        {
            if (currentSyntaxNode is InvocationExpressionSyntax currentInvocationExpressionSyntax)
            {
                if (context.SemanticModel.GetSymbolInfo(currentInvocationExpressionSyntax, context.CancellationToken).Symbol is IMethodSymbol methodSymbol
                    && IsRuleBuilderApiUsage(methodSymbol))
                {
                    var nextSyntaxNode = currentInvocationExpressionSyntax.Expression;
                    if (nextSyntaxNode is MemberAccessExpressionSyntax nextMemberAccessExpressionSyntax)
                    {
                        AnalyzeRuleBuilderApiUsage(context, initialSyntaxNode, nextMemberAccessExpressionSyntax.Expression);
                    }
                }
            }

            if (currentSyntaxNode is IdentifierNameSyntax identifierNameSyntax)
            {
                if (context.SemanticModel.GetSymbolInfo(identifierNameSyntax, context.CancellationToken).Symbol is INamedTypeSymbol symbol
                    && RuleBuilderApiTypes.Contains(symbol.Name, StringComparer.Ordinal) && IsRulesFrameworkSymbol(symbol))
                {
                    var display = initialSyntaxNode.WithLeadingTrivia().WithTrailingTrivia().ToFullString();
                    var location = Location.Create(initialSyntaxNode.SyntaxTree, TextSpan.FromBounds(currentSyntaxNode.Span.Start, initialSyntaxNode.Span.End));
                    var diagnostic = Diagnostic.Create(Rule, location, display);
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }

        private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is InvocationExpressionSyntax invocation)
            {
                if (context.SemanticModel.GetSymbolInfo(invocation, context.CancellationToken).Symbol is IMethodSymbol methodSymbol)
                {
                    if (IsRulesFrameworkSymbol(methodSymbol.ContainingType))
                    {
                        switch (methodSymbol.Name)
                        {
                            case "Build" when RuleBuilderApiTypes.Contains(methodSymbol.ContainingType.Name, StringComparer.Ordinal):
                                AnalyzeRuleBuilderApiUsage(context, invocation, invocation);
                                return;

                            default:
                                return;
                        }
                    }

                    var expr = invocation.Expression;
                    var exprType = context.SemanticModel.GetTypeInfo(expr, context.CancellationToken).ConvertedType;
                    if (exprType is INamedTypeSymbol namedExprType && IsRulesFrameworkSymbol(namedExprType))
                    {
                        var display = namedExprType.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
                        var diagnostic = Diagnostic.Create(Rule, invocation.GetLocation(), display);
                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }
        }

        private static bool IsRuleBuilderApiUsage(IMethodSymbol methodSymbol)
        {
            if (RuleBuilderApiTypes.Contains(methodSymbol.ContainingType.Name, StringComparer.Ordinal))
            {
                return true;
            }

            if (methodSymbol.IsExtensionMethod && RuleBuilderApiTypes.Contains(methodSymbol.ReturnType.Name, StringComparer.Ordinal))
            {
                return true;
            }

            return false;
        }

        private static bool IsRulesFrameworkSymbol(INamespaceOrTypeSymbol symbol)
        {
            if (symbol == null)
            {
                return false;
            }

            var ns = symbol.ContainingNamespace;
            if (ns == null)
            {
                return false;
            }

            var nsString = ns.ToDisplayString();
            return nsString.StartsWith("Rules.Framework", StringComparison.Ordinal);
        }
    }
}