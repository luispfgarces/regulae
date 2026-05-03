namespace Regulae.Analyzers.Tests.RewriteToRegulaeRuleBuilderApi
{
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Testing;
    using Microsoft.CodeAnalysis.Testing;
    using Regulae.Analyzers.RewriteToRegulaeRuleBuilderApi;
    using Regulae.Analyzers.RewriteToRegulaeRuleBuilderApi.CodeFix;
    using Xunit;

    public partial class RewriteToRegulaeRuleBuilderApiTests
    {
        [Fact]
        public async Task RulesFramework_RewriteRuleWithComplexConditionTreeUsingObsoleteApisAndDateBegin()
        {
            var testCode = @"
using System;
using Rules.Framework;
using Rules.Framework.Core;

namespace Sample
{
    public class C
    {
        public void M()
        {
            RuleBuilder.NewRule<string, string>()
                .WithName(""LambdaRule"")
                .WithDateBegin(DateTime.Parse(""2018-01-01""))
                .WithContent(""test content type"", new object())
                .WithCondition((root) => root
                    .AsComposed()
                    .WithLogicalOperator(LogicalOperators.And)
                    .AddCondition(x => x
                        .AsValued(""age"")
                        .OfDataType<int>()
                        .WithComparisonOperator(Operators.GreaterThan)
                        .SetOperand(18)
                        .Build())
                    .AddCondition(x => x
                        .AsValued(""score"")
                        .OfDataType<int>()
                        .WithComparisonOperator(Operators.GreaterThanOrEqual)
                        .SetOperand(90)
                        .Build())
                    .AddCondition((x) => x
                        .AsComposed()
                        .WithLogicalOperator(LogicalOperators.Or)
                        .AddCondition((y) => y
                            .AsValued(""height"")
                            .OfDataType<int>()
                            .WithComparisonOperator(Operators.GreaterThanOrEqual)
                            .SetOperand(150)
                            .Build())
                        .AddCondition((y) => y
                            .AsValued(""height"")
                            .OfDataType<int>()
                            .WithComparisonOperator(Operators.LesserThan)
                            .SetOperand(190)
                            .Build())
                        .Build())
                    .Build())
                .Build();
        }
    }
}
";

            var fixedCode = @"
using System;
using Rules.Framework;
using Rules.Framework.Core;
using Regulae;

namespace Sample
{
    public class C
    {
        public void M()
        {
            global::Regulae.Rule.Create<string, string>(""LambdaRule"")
                .InRuleset(""test content type"")
                .SetContent(new object())
                .Since(DateTime.Parse(""2018-01-01""))
                .ApplyWhen(root => root
                    .And(x => x
                        .Value(""age"", global::Regulae.Operators.GreaterThan, 18)
                        .Value(""score"", global::Regulae.Operators.GreaterThanOrEqual, 90)
                        .Or(y => y
                            .Value(""height"", global::Regulae.Operators.GreaterThanOrEqual, 150)
                            .Value(""height"", global::Regulae.Operators.LesserThan, 190))))
                .Build();
        }
    }
}
";

            var test = new CSharpCodeFixTest<RuleBuilderApiAnalyzer, RuleBuilderApiCodeFixProvider, DefaultVerifier>
            {
                CodeActionEquivalenceKey = "RewriteToRegulaeRuleBuilderApi",
                NumberOfIncrementalIterations = 1
            };

            test.TestState.Sources.Add(("Test0.cs", testCode));
            test.TestState.ReferenceAssemblies = ReferenceAssemblies.Net.Net80
                .AddPackages([new PackageIdentity("Rules.Framework", "2.0.8")]);

            test.FixedState.Sources.Add(("Test0.cs", fixedCode));
            test.FixedState.ReferenceAssemblies = ReferenceAssemblies.Net.Net80
                .AddPackages([new PackageIdentity("Rules.Framework", "2.0.8")]);
            test.FixedState.AdditionalReferences.Add(typeof(Rule).Assembly);

            test.ExpectedDiagnostics.Add(
                new DiagnosticResult(RegulaeDiagnostics.RewriteToRegulaeRuleBuilderApiId, DiagnosticSeverity.Info)
                    .WithSpan("Test0.cs", 12, 13, 48, 25)
                    .WithMessage("Rule builder from 'Rules.Framework' detected. Consider rewriting to Regulae API."));

            await test.RunAsync();
        }

        [Fact]
        public async Task RulesFramework_RewriteRuleWithSingleConditionUsingObsoleteApisAndDateBegin()
        {
            var testCode = @"
using System;
using Rules.Framework;
using Rules.Framework.Core;

namespace Sample
{
    public class C
    {
        public void M()
        {
            RuleBuilder.NewRule<string, string>()
                .WithName(""Test Rule"")
                .WithDateBegin(DateTime.Parse(""2018-01-01""))
                .WithContent(""test content type"", new object())
                .WithCondition(x => x
                    .AsValued(""age"")
                    .OfDataType<int>()
                    .WithComparisonOperator(Operators.LesserThanOrEqual)
                    .SetOperand(18)
                    .Build())
                .Build();
        }
    }
}
";

            var fixedCode = @"
using System;
using Rules.Framework;
using Rules.Framework.Core;
using Regulae;

namespace Sample
{
    public class C
    {
        public void M()
        {
            global::Regulae.Rule.Create<string, string>(""Test Rule"")
                .InRuleset(""test content type"")
                .SetContent(new object())
                .Since(DateTime.Parse(""2018-01-01""))
                .ApplyWhen(""age"", global::Regulae.Operators.LesserThanOrEqual, 18)
                .Build();
        }
    }
}
";

            var test = new CSharpCodeFixTest<RuleBuilderApiAnalyzer, RuleBuilderApiCodeFixProvider, DefaultVerifier>
            {
                CodeActionEquivalenceKey = "RewriteToRegulaeRuleBuilderApi",
                NumberOfIncrementalIterations = 1,
            };

            test.TestState.Sources.Add(("Test0.cs", testCode));
            test.TestState.ReferenceAssemblies = ReferenceAssemblies.Net.Net80
                .AddPackages([new PackageIdentity("Rules.Framework", "2.0.8")]);

            test.FixedState.Sources.Add(("Test0.cs", fixedCode));
            test.FixedState.ReferenceAssemblies = ReferenceAssemblies.Net.Net80
                .AddPackages([new PackageIdentity("Rules.Framework", "2.0.8")]);
            test.FixedState.AdditionalReferences.Add(typeof(Rule).Assembly);

            test.ExpectedDiagnostics.Add(
                new DiagnosticResult(RegulaeDiagnostics.RewriteToRegulaeRuleBuilderApiId, DiagnosticSeverity.Info)
                    .WithSpan("Test0.cs", 12, 13, 22, 25)
                    .WithMessage("Rule builder from 'Rules.Framework' detected. Consider rewriting to Regulae API."));

            await test.RunAsync();
        }
        [Fact]
        public async Task RulesFramework_EmitsDiagnosticButNoRewriteWhenUsingConditionBuilderWithUnknownExtensionMethodOverObsoleteApis()
        {
            var testCode = @"
using System;
using Rules.Framework;
using Rules.Framework.Builder;
using Rules.Framework.Core;

namespace Sample
{
    public class C
    {
        public void M()
        {
            RuleBuilder.NewRule<string, string>()
                .WithName(""LambdaRule"")
                .WithDateBegin(DateTime.Parse(""2018-01-01""))
                .WithContent(""test content type"", new object())
                .WithCondition((root) => root
                    .AsComposed()
                    .WithLogicalOperator(LogicalOperators.And)
                    .AddCondition(x => x
                        .AsValued(""age"")
                        .OfDataType<int>()
                        .WithComparisonOperator(Operators.GreaterThan)
                        .SetOperand(18)
                        .Build())
                    .AddCondition(x => x
                        .AsValued(""score"")
                        .OfDataType<int>()
                        .WithComparisonOperator(Operators.GreaterThanOrEqual)
                        .SetOperand(90)
                        .Build())
                    .AddConditionIn(""color"", new[] { ""red"", ""blue"" })
                    .Build())
                .Build();
        }
    }
}

namespace Rules.Framework.Builder
{
    public static class ConditionBuilderExtensions
    {
        public static IComposedConditionNodeBuilder<TCondition> AddConditionIn<TCondition, TDataType>(this IComposedConditionNodeBuilder<TCondition> builder, TCondition condition, TDataType[] rightOperand)
            where TCondition : notnull
        {
            return builder.AddCondition(x => x
                .AsValued(condition)
                    .OfDataType<TDataType>()
                    .WithComparisonOperator(Operators.In)
                    .SetOperand(rightOperand)
                    .Build());
        }
    }
}
";

            var test = new CSharpCodeFixTest<RuleBuilderApiAnalyzer, RuleBuilderApiCodeFixProvider, DefaultVerifier>
            {
                CodeActionEquivalenceKey = "RewriteToRegulaeRuleBuilderApi",
                NumberOfIncrementalIterations = 0
            };

            test.TestState.Sources.Add(("Test0.cs", testCode));
            test.TestState.ReferenceAssemblies = ReferenceAssemblies.Net.Net80
                .AddPackages([new PackageIdentity("Rules.Framework", "2.0.8")]);

            test.FixedState.Sources.Add(("Test0.cs", testCode));
            test.FixedState.ReferenceAssemblies = ReferenceAssemblies.Net.Net80
                .AddPackages([new PackageIdentity("Rules.Framework", "2.0.8")]);

            test.ExpectedDiagnostics.Add(
                new DiagnosticResult(RegulaeDiagnostics.RewriteToRegulaeRuleBuilderApiId, DiagnosticSeverity.Info)
                    .WithSpan("Test0.cs", 13, 13, 34, 25)
                    .WithMessage("Rule builder from 'Rules.Framework' detected. Consider rewriting to Regulae API."));

            await test.RunAsync();
        }
    }
}
