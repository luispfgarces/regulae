namespace Regulae.Analyzers.RewriteToRegulaeRuleAddPriorityOption
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.Editing;
    using Microsoft.CodeAnalysis.Formatting;
    using Microsoft.CodeAnalysis.Simplification;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RuleAddPriorityOptionCodeFixProvider))]
    public sealed class RuleAddPriorityOptionCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds => [RegulaeDiagnostics.RewriteToRegulaeRuleAddPriorityOptionId];

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        private const string Title = "Rewrite to Regulae RuleAddPriorityOption";
        private const string EquivalenceKey = "RewriteToRegulaeRuleAddPriorityOption";

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var nodeCandidateToFix = root.FindNode(diagnostic.Location.SourceSpan);

            if (CodeFixEvaluator.CanFixNode(nodeCandidateToFix))
            {
                context.RegisterCodeFix(
                        Microsoft.CodeAnalysis.CodeActions.CodeAction.Create(
                            Title,
                            ct => ApplyRewriteAsync(context.Document, diagnostic, ct),
                            equivalenceKey: EquivalenceKey),
                        diagnostic);
            }
        }

        private static async Task<Document> ApplyRewriteAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var originalNode = root.FindNode(diagnostic.Location.SourceSpan);
            var rewrittenNode = RewriteNode(originalNode);

            // Replace the node with rewritten expression
            var newRoot = root.ReplaceNode(originalNode, rewrittenNode);
            document = document.WithSyntaxRoot(newRoot);

            // Ensure using statement is added
            document = await EnsureUsingRegulae(document, cancellationToken).ConfigureAwait(false);

            var workspace = document.Project.Solution.Workspace;
            var optionSet = workspace.Options
                .WithChangedOption(FormattingOptions.UseTabs, LanguageNames.CSharp, value: false)
                .WithChangedOption(FormattingOptions.TabSize, LanguageNames.CSharp, 4)
                .WithChangedOption(FormattingOptions.IndentationSize, LanguageNames.CSharp, 4)
                .WithChangedOption(FormattingOptions.NewLine, LanguageNames.CSharp, "\n");

            var simplified = await Simplifier.ReduceAsync(document, optionSet, cancellationToken: cancellationToken).ConfigureAwait(false);
            return await Formatter.FormatAsync(simplified, rewrittenNode.FullSpan, optionSet, cancellationToken).ConfigureAwait(false);
        }

        private static SyntaxNode RewriteNode(SyntaxNode originalNode) =>
            originalNode switch
            {
                InvocationExpressionSyntax invocation => RewriteInvocation(invocation),
                MemberAccessExpressionSyntax memberAccess => RewriteMemberAccess(memberAccess),
                ArgumentSyntax argumentSyntax => argumentSyntax.WithExpression((ExpressionSyntax)RewriteNode(argumentSyntax.Expression)),
                _ => throw new NotSupportedException("Unsupported syntax node type for rewrite."),
            };

        private static InvocationExpressionSyntax RewriteInvocation(InvocationExpressionSyntax invocation)
        {
            if (invocation.Expression is MemberAccessExpressionSyntax memberAccess)
            {
                var memberName = memberAccess.Name.Identifier.Text;
                var regulaeMemberName = MapRulesFrameworkMemberToRegulae(memberName);

                var regulaeNamespace = SyntaxFactory.ParseExpression("global::Regulae.RuleAddPriorityOption")
                    .WithAdditionalAnnotations(Simplifier.Annotation);

                var newMemberAccess = SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    regulaeNamespace,
                    SyntaxFactory.IdentifierName(regulaeMemberName))
                        .WithLeadingTrivia(memberAccess.GetLeadingTrivia())
                        .WithTrailingTrivia(memberAccess.GetTrailingTrivia());

                return SyntaxFactory.InvocationExpression(newMemberAccess, invocation.ArgumentList)
                    .WithLeadingTrivia(invocation.GetLeadingTrivia())
                    .WithTrailingTrivia(invocation.GetTrailingTrivia());
            }

            return invocation;
        }

        private static MemberAccessExpressionSyntax RewriteMemberAccess(MemberAccessExpressionSyntax memberAccess)
        {
            var memberName = memberAccess.Name.Identifier.Text;
            var regulaeMemberName = MapRulesFrameworkMemberToRegulae(memberName);

            var regulaeNamespace = SyntaxFactory.ParseExpression("global::Regulae.RuleAddPriorityOption")
                .WithAdditionalAnnotations(Simplifier.Annotation);

            var newMemberAccess = SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                regulaeNamespace,
                SyntaxFactory.IdentifierName(regulaeMemberName))
                    .WithLeadingTrivia(memberAccess.GetLeadingTrivia())
                    .WithTrailingTrivia(memberAccess.GetTrailingTrivia());

            return newMemberAccess;
        }

        private static string MapRulesFrameworkMemberToRegulae(string rulesFrameworkMember)
        {
            return rulesFrameworkMember switch
            {
                RulesFrameworkConstants.AtBottom => "AtLargestNumber",
                RulesFrameworkConstants.AtTop => "AtSmallestNumber",
                RulesFrameworkConstants.ByPriorityNumber => "AtNumber",
                RulesFrameworkConstants.ByRuleName => "AtRuleName",
                _ => rulesFrameworkMember,
            };
        }

        private static async Task<Document> EnsureUsingRegulae(Document document, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            if (root is CompilationUnitSyntax compilationUnit)
            {
                var hasRegulaeUsing = compilationUnit.Usings.Any(u => string.Equals(u.Name.ToString(), "Regulae", StringComparison.Ordinal));
                if (!hasRegulaeUsing)
                {
                    var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);
                    var usingDirective = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("Regulae"))
                        .WithTrailingTrivia(SyntaxFactory.ElasticLineFeed);
                    var newCompilation = compilationUnit.AddUsings(usingDirective);
                    editor.ReplaceNode(compilationUnit, newCompilation);
                    return editor.GetChangedDocument();
                }
            }

            return document;
        }
    }
}
