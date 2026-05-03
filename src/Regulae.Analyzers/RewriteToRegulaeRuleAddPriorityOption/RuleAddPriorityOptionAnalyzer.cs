namespace Regulae.Analyzers.RewriteToRegulaeRuleAddPriorityOption
{
    using System;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class RuleAddPriorityOptionAnalyzer : DiagnosticAnalyzer
    {
        private static readonly string TypeName = "RuleAddPriorityOption";
        private static readonly LocalizableString Title = "Rules.Framework RuleAddPriorityOption usage detected";
        private static readonly LocalizableString MessageFormat = "RuleAddPriorityOption from 'Rules.Framework' detected. Consider rewriting to Regulae RuleAddPriorityOption.";
        private static readonly LocalizableString Description = "Detects usages of Rules.Framework RuleAddPriorityOption to help automated migration to Regulae RuleAddPriorityOption.";
        private const string Category = "Migration";

        private static readonly DiagnosticDescriptor Rule = new(
            RegulaeDiagnostics.RewriteToRegulaeRuleAddPriorityOptionId,
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
                compilationStartContext.RegisterSyntaxNodeAction(AnalyzeMemberAccess, SyntaxKind.SimpleMemberAccessExpression);
            });
        }

        private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is InvocationExpressionSyntax invocation)
            {
                if (context.SemanticModel.GetSymbolInfo(invocation, context.CancellationToken).Symbol is IMethodSymbol methodSymbol)
                {
                    if (RulesFrameworkHelpers.IsRulesFrameworkSymbol(methodSymbol.ContainingType)
                        && string.Equals(methodSymbol.ContainingType.Name, TypeName, StringComparison.Ordinal))
                    {
                        var display = invocation.WithLeadingTrivia().WithTrailingTrivia().ToFullString();
                        var location = Location.Create(invocation.SyntaxTree, TextSpan.FromBounds(invocation.Span.Start, invocation.Span.End));
                        var diagnostic = Diagnostic.Create(Rule, location, display);
                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }
        }

        private static void AnalyzeMemberAccess(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is MemberAccessExpressionSyntax memberAccess)
            {
                if (context.SemanticModel.GetSymbolInfo(memberAccess, context.CancellationToken).Symbol is IPropertySymbol propertySymbol)
                {
                    if (RulesFrameworkHelpers.IsRulesFrameworkSymbol(propertySymbol.ContainingType)
                        && string.Equals(propertySymbol.ContainingType.Name, TypeName, StringComparison.Ordinal))
                    {
                        var display = memberAccess.WithLeadingTrivia().WithTrailingTrivia().ToFullString();
                        var location = Location.Create(memberAccess.SyntaxTree, TextSpan.FromBounds(memberAccess.Span.Start, memberAccess.Span.End));
                        var diagnostic = Diagnostic.Create(Rule, location, display);
                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }
        }
    }
}
