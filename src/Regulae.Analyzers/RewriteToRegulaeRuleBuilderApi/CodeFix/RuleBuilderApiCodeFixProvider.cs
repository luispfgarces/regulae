namespace Regulae.Analyzers.RewriteToRegulaeRuleBuilderApi.CodeFix
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
    using Regulae.Analyzers;

    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RuleBuilderApiCodeFixProvider))]
    public sealed class RuleBuilderApiCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds => [RegulaeDiagnostics.RewriteToRegulaeRuleBuilderApiId];

        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        private const string Title = "Rewrite to Regulae rule builder API";
        private const string EquivalenceKey = "RewriteToRegulaeRuleBuilderApi";

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var nodeCandidateToFix = root.FindNode(diagnostic.Location.SourceSpan);
            var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);

            if (CodeFixEvaluator.CanFixNode(nodeCandidateToFix, semanticModel))
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
            var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var originalSyntaxNode = root.FindNode(diagnostic.Location.SourceSpan);
            var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            var trackSyntaxNodeToReplace = new SyntaxAnnotation();
            var annotatedSyntaxNodeToReplace = root.FindNode(diagnostic.Location.SourceSpan)
                .WithAdditionalAnnotations(trackSyntaxNodeToReplace);
            editor.ReplaceNode(originalSyntaxNode, annotatedSyntaxNodeToReplace);

            document = await EnsureUsingRegulae(editor.GetChangedDocument(), cancellationToken).ConfigureAwait(false);

            SyntaxNode currentChain = originalSyntaxNode switch
            {
                ArgumentSyntax argumentSyntax => RewriteArgumentSyntax(semanticModel, argumentSyntax, cancellationToken),
                InvocationExpressionSyntax invocationExpressionSyntax => RewriteRuleBuildInvocationExpression(semanticModel, invocationExpressionSyntax, cancellationToken),
                _ => throw new NotSupportedException("Unsupported syntax node type for rewrite."),
            };

            var trackAnnotation = new SyntaxAnnotation();
            var replacement = currentChain
                .WithLeadingTrivia(originalSyntaxNode.GetLeadingTrivia())
                .WithTrailingTrivia(originalSyntaxNode.GetTrailingTrivia())
                .WithAdditionalAnnotations(Simplifier.Annotation, trackAnnotation);

            editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);
            root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var syntaxNodeToReplace = root.GetAnnotatedNodes(trackSyntaxNodeToReplace).FirstOrDefault();
            editor.ReplaceNode(syntaxNodeToReplace, replacement);

            document = editor.GetChangedDocument();

            var workspace = document.Project.Solution.Workspace;
            var optionSet = workspace.Options
                .WithChangedOption(FormattingOptions.UseTabs, LanguageNames.CSharp, value: false)
                .WithChangedOption(FormattingOptions.TabSize, LanguageNames.CSharp, 4)
                .WithChangedOption(FormattingOptions.IndentationSize, LanguageNames.CSharp, 4)
                .WithChangedOption(FormattingOptions.NewLine, LanguageNames.CSharp, "\n");

            var simplified = await Simplifier.ReduceAsync(document, optionSet, cancellationToken: cancellationToken).ConfigureAwait(false);

            var simplifiedRoot = await simplified.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var annotatedNode = simplifiedRoot?.GetAnnotatedNodes(trackAnnotation).FirstOrDefault();
            var formatSpan = annotatedNode?.FullSpan ?? (originalSyntaxNode.FullSpan);
            return await Formatter.FormatAsync(simplified, formatSpan, optionSet, cancellationToken).ConfigureAwait(false);
        }

        private static ArgumentSyntax RewriteArgumentSyntax(SemanticModel semanticModel, ArgumentSyntax argumentSyntax, CancellationToken cancellationToken)
        {
            var newExpression = RewriteRuleBuildInvocationExpression(semanticModel, argumentSyntax.Expression, cancellationToken);
            return argumentSyntax.WithExpression(newExpression);
        }

        private static ExpressionSyntax RewriteRuleBuildInvocationExpression(SemanticModel semanticModel, SyntaxNode syntaxNode, CancellationToken cancellationToken)
        {
            var ruleBuilderParameters = RuleBuilderExtractor.ExtractOriginalFluentChain(syntaxNode, semanticModel, cancellationToken);
            return RewriteSyntaxTree(syntaxNode, semanticModel, ruleBuilderParameters);
        }

        private static ExpressionSyntax RewriteSyntaxTree(SyntaxNode originalSyntaxNode, SemanticModel semanticModel, RuleBuilderParameters ruleBuilderParameters)
        {
            var nameExpr = ruleBuilderParameters.Name;
            var rulesetTypeSyntax = ruleBuilderParameters.TypeParameters.RulesetType;
            var conditionTypeSyntax = ruleBuilderParameters.TypeParameters.ConditionType;

            var fluentLeadingTrivia = originalSyntaxNode.GetLeadingTrivia()
                .Add(SyntaxFactory.ElasticTab)
                .Where(t => !t.IsKind(SyntaxKind.EndOfLineTrivia))
                .ToSyntaxTriviaList();
            var fluentTrailingTrivia = SyntaxFactory.TriviaList(SyntaxFactory.ElasticLineFeed);

            ExpressionSyntax currentChain = EmitCreate(nameExpr, rulesetTypeSyntax, conditionTypeSyntax, fluentTrailingTrivia);
            currentChain = EmitInRuleset(ruleBuilderParameters, fluentLeadingTrivia, fluentTrailingTrivia, currentChain);
            currentChain = EmitSetContent(ruleBuilderParameters, fluentLeadingTrivia, fluentTrailingTrivia, currentChain);
            currentChain = EmitSince(ruleBuilderParameters, fluentLeadingTrivia, fluentTrailingTrivia, currentChain);
            currentChain = EmitUntil(ruleBuilderParameters, fluentLeadingTrivia, fluentTrailingTrivia, currentChain);
            currentChain = EmitApplyWhen(semanticModel, ruleBuilderParameters, fluentLeadingTrivia, fluentTrailingTrivia, currentChain);
            currentChain = EmitWithActive(ruleBuilderParameters, fluentLeadingTrivia, fluentTrailingTrivia, currentChain);
            return EmitBuild(fluentLeadingTrivia, currentChain);
        }

        private static ExpressionSyntax EmitBuild(SyntaxTriviaList fluentLeadingTrivia, ExpressionSyntax currentChain)
        {
            var buildAccess = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, currentChain, SyntaxFactory.IdentifierName("Build"))
                            .ApplyFluentChainFormatting(fluentLeadingTrivia);
            currentChain = SyntaxFactory.InvocationExpression(buildAccess);
            return currentChain;
        }

        private static ExpressionSyntax EmitWithActive(RuleBuilderParameters ruleBuilderParameters, SyntaxTriviaList fluentLeadingTrivia, SyntaxTriviaList fluentTrailingTrivia, ExpressionSyntax currentChain)
        {
            if (ruleBuilderParameters.Active is not null)
            {
                var activeAccess = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, currentChain, SyntaxFactory.IdentifierName("WithActive"))
                    .ApplyFluentChainFormatting(fluentLeadingTrivia);
                currentChain = SyntaxFactory.InvocationExpression(activeAccess, SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(ruleBuilderParameters.Active))))
                    .ApplyFluentChainFormatting(fluentTrailingTrivia);
            }

            return currentChain;
        }

        private static ExpressionSyntax EmitApplyWhen(SemanticModel semanticModel, RuleBuilderParameters ruleBuilderParameters, SyntaxTriviaList fluentLeadingTrivia, SyntaxTriviaList fluentTrailingTrivia, ExpressionSyntax currentChain)
        {
            if (ruleBuilderParameters.Condition.SimpleValueCondition is not null)
            {
                var sc = ruleBuilderParameters.Condition.SimpleValueCondition;
                var condAccess = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, currentChain, SyntaxFactory.IdentifierName("ApplyWhen"))
                    .ApplyFluentChainFormatting(fluentLeadingTrivia);
                var operatorTypeSyntax = SyntaxFactory.ParseExpression("global::Regulae.Operators")
                    .WithAdditionalAnnotations(Simplifier.Annotation);
                var operatorNameSyntax = SyntaxFactory.IdentifierName(((MemberAccessExpressionSyntax)sc.OperatorExpression).Name.Identifier.Text);
                var operatorExpression = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, operatorTypeSyntax, operatorNameSyntax);
                var argList = SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(
                [
                    SyntaxFactory.Argument(sc.ConditionKey),
                    SyntaxFactory.Argument(operatorExpression),
                    SyntaxFactory.Argument(sc.Operand),
                ]));
                currentChain = SyntaxFactory.InvocationExpression(condAccess, argList)
                    .ApplyFluentChainFormatting(fluentTrailingTrivia);
            }
            else if (ruleBuilderParameters.Condition.ConditionLambda is not null)
            {
                var condAccess = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, currentChain, SyntaxFactory.IdentifierName("ApplyWhen"))
                    .ApplyFluentChainFormatting(fluentLeadingTrivia);
                var translatedLambda = ConditionTranslator.TranslateLambda(ruleBuilderParameters.Condition.ConditionLambda, semanticModel, fluentLeadingTrivia);
                var argList = ConditionTranslator.TryExtractValueConditionArguments(translatedLambda, out var valueConditionArgs)
                    ? valueConditionArgs
                    : SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(translatedLambda)));
                currentChain = SyntaxFactory.InvocationExpression(condAccess, argList)
                    .ApplyFluentChainFormatting(fluentTrailingTrivia);
            }
            else if (ruleBuilderParameters.Condition.RawConditionExpression is not null)
            {
                var condAccess = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, currentChain, SyntaxFactory.IdentifierName("ApplyWhen"))
                    .ApplyFluentChainFormatting(fluentLeadingTrivia);
                var argList = SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(ruleBuilderParameters.Condition.RawConditionExpression)));
                currentChain = SyntaxFactory.InvocationExpression(condAccess, argList)
                    .ApplyFluentChainFormatting(fluentTrailingTrivia);
            }

            return currentChain;
        }

        private static ExpressionSyntax EmitUntil(RuleBuilderParameters ruleBuilderParameters, SyntaxTriviaList fluentLeadingTrivia, SyntaxTriviaList fluentTrailingTrivia, ExpressionSyntax currentChain)
        {
            if (ruleBuilderParameters.DateEnd is not null)
            {
                var untilAccess = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, currentChain, SyntaxFactory.IdentifierName("Until"))
                    .ApplyFluentChainFormatting(fluentLeadingTrivia);
                currentChain = SyntaxFactory.InvocationExpression(untilAccess, SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(ruleBuilderParameters.DateEnd))))
                    .ApplyFluentChainFormatting(fluentTrailingTrivia);
            }

            return currentChain;
        }

        private static ExpressionSyntax EmitSince(RuleBuilderParameters ruleBuilderParameters, SyntaxTriviaList fluentLeadingTrivia, SyntaxTriviaList fluentTrailingTrivia, ExpressionSyntax currentChain)
        {
            var sinceAccess = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, currentChain, SyntaxFactory.IdentifierName("Since"))
                            .ApplyFluentChainFormatting(fluentLeadingTrivia);
            currentChain = SyntaxFactory.InvocationExpression(sinceAccess, SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(ruleBuilderParameters.DateBegin))))
                .ApplyFluentChainFormatting(fluentTrailingTrivia);
            return currentChain;
        }

        private static ExpressionSyntax EmitSetContent(RuleBuilderParameters ruleBuilderParameters, SyntaxTriviaList fluentLeadingTrivia, SyntaxTriviaList fluentTrailingTrivia, ExpressionSyntax currentChain)
        {
            var setContentAccess = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, currentChain, SyntaxFactory.IdentifierName("SetContent"))
                            .ApplyFluentChainFormatting(fluentLeadingTrivia);
            currentChain = SyntaxFactory.InvocationExpression(setContentAccess, SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(ruleBuilderParameters.Content))))
                .ApplyFluentChainFormatting(fluentTrailingTrivia);
            return currentChain;
        }

        private static ExpressionSyntax EmitInRuleset(RuleBuilderParameters ruleBuilderParameters, SyntaxTriviaList fluentLeadingTrivia, SyntaxTriviaList fluentTrailingTrivia, ExpressionSyntax currentChain)
        {
            var inRulesetAccess = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, currentChain, SyntaxFactory.IdentifierName("InRuleset"))
                            .ApplyFluentChainFormatting(fluentLeadingTrivia);
            currentChain = SyntaxFactory.InvocationExpression(inRulesetAccess, SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(ruleBuilderParameters.Ruleset))))
                .ApplyFluentChainFormatting(fluentTrailingTrivia);
            return currentChain;
        }

        private static InvocationExpressionSyntax EmitCreate(ExpressionSyntax nameExpr, TypeSyntax rulesetTypeSyntax, TypeSyntax conditionTypeSyntax, SyntaxTriviaList fluentTrailingTrivia)
        {
            var left = SyntaxFactory.ParseExpression("global::Regulae.Rule")
                            .WithAdditionalAnnotations(Simplifier.Annotation);
            var genericCreate = SyntaxFactory.GenericName(
                    SyntaxFactory.Identifier("Create"))
                .WithTypeArgumentList(
                    SyntaxFactory.TypeArgumentList(
                        SyntaxFactory.SeparatedList<TypeSyntax>(new SyntaxNodeOrToken[]
                        {
                            rulesetTypeSyntax,
                            SyntaxFactory.Token(SyntaxKind.CommaToken),
                            conditionTypeSyntax,
                        })));
            var createMemberAccess = SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, left, genericCreate);
            var createInvocation = SyntaxFactory.InvocationExpression(createMemberAccess, SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList([SyntaxFactory.Argument(nameExpr)])))
                .ApplyFluentChainFormatting(fluentTrailingTrivia);
            return createInvocation;
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