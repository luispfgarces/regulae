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
        public async Task RulesFramework_RewriteRuleWithComplexConditionTreeAndDateBegin()
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
                .WithCondition(root => root
                .And(b => b
                .Value(""age"", Operators.GreaterThan, 18)
                .Value(""score"", Operators.GreaterThanOrEqual, 90)
                .Or((c) => c
                .Value(""height"", Operators.GreaterThanOrEqual, 150)
                .Value(""height"", Operators.LesserThan, 190))))
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
                    .And(b => b
                        .Value(""age"", global::Regulae.Operators.GreaterThan, 18)
                        .Value(""score"", global::Regulae.Operators.GreaterThanOrEqual, 90)
                        .Or(c => c
                            .Value(""height"", global::Regulae.Operators.GreaterThanOrEqual, 150)
                            .Value(""height"", global::Regulae.Operators.LesserThan, 190))))
                .Build();
        }
    }
}
";

            var test = new CSharpCodeFixTest<RulesFrameworkAnalyzer, RulesFrameworkCodeFixProvider, DefaultVerifier>
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
                    .WithSpan("Test0.cs", 12, 13, 23, 25)
                    .WithMessage("Rule builder from 'Rules.Framework' detected. Consider rewriting to Regulae API."));

            await test.RunAsync();
        }

        [Fact]
        public async Task RulesFramework_RewriteRuleWithSingleConditionAndDateBegin()
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
                .WithName(""Test Rule"");

            RuleBuilder.NewRule<string, string>()
                .WithName(""Test Rule"")
                .WithDatesInterval(DateTime.Parse(""2018-01-01""), DateTime.Parse(""2018-12-31""))
                .WithContent(""test content type"", new object())
                .WithCondition(""age"", Operators.LesserThanOrEqual, 18)
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
            RuleBuilder.NewRule<string, string>()
                .WithName(""Test Rule"");

            global::Regulae.Rule.Create<string, string>(""Test Rule"")
                .InRuleset(""test content type"")
                .SetContent(new object())
                .Since(DateTime.Parse(""2018-01-01""))
                .Until(DateTime.Parse(""2018-12-31""))
                .ApplyWhen(""age"", global::Regulae.Operators.LesserThanOrEqual, 18)
                .Build();
        }
    }
}
";

            var test = new CSharpCodeFixTest<RulesFrameworkAnalyzer, RulesFrameworkCodeFixProvider, DefaultVerifier>
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
                    .WithSpan("Test0.cs", 15, 13, 20, 25)
                    .WithMessage("Rule builder from 'Rules.Framework' detected. Consider rewriting to Regulae API."));

            await test.RunAsync();
        }

        [Fact]
        public async Task RulesFramework_RewriteRuleWithNoConditionAndDateBeginAndWithActive()
        {
            var testCode = @"
using System;
using Rules.Framework;
using Rules.Framework.Core;

namespace Sample
{
    public class ScoreSample
    {
        public void Configure()
        {
            RuleBuilder.NewRule<string, string>()
                .WithName(""Default score"")
                .WithDateBegin(DateTime.Parse(""2020-06-01""))
                .WithContent(""Score"", 10)
                .WithActive(false)
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
    public class ScoreSample
    {
        public void Configure()
        {
            global::Regulae.Rule.Create<string, string>(""Default score"")
                .InRuleset(""Score"")
                .SetContent(10)
                .Since(DateTime.Parse(""2020-06-01""))
                .WithActive(false)
                .Build();
        }
    }
}
";

            var test = new CSharpCodeFixTest<RulesFrameworkAnalyzer, RulesFrameworkCodeFixProvider, DefaultVerifier>
            {
                CodeActionEquivalenceKey = "RewriteToRegulaeRuleBuilderApi",
                TestBehaviors = TestBehaviors.SkipSuppressionCheck,
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
                    .WithSpan("Test0.cs", 12, 13, 17, 25)
                    .WithMessage("Rule builder from 'Rules.Framework' detected. Consider rewriting to Regulae API."));

            await test.RunAsync();
        }

        [Fact]
        public async Task RulesFramework_EmitsDiagnosticButNoRewriteWhenUsingConditionBuilderWithUnknownExtensionMethod()
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
                .WithCondition(root => root
                .And(b => b
                .Value(""age"", Operators.GreaterThan, 18)
                .Value(""score"", Operators.GreaterThanOrEqual, 90)
                .ValueIn(""color"", new[] { ""red"", ""blue"" })))
                .Build();
        }
    }
}

namespace Rules.Framework.Builder
{
    public static class ConditionBuilderExtensions
    {
        public static IFluentComposedConditionNodeBuilder<TCondition> ValueIn<TCondition, TDataType>(this IFluentComposedConditionNodeBuilder<TCondition> builder, TCondition condition, TDataType[] rightOperand)
            where TCondition : notnull
        {
            return builder.Value(condition, Operators.In, rightOperand);
        }
    }
}
";

            var test = new CSharpCodeFixTest<RulesFrameworkAnalyzer, RulesFrameworkCodeFixProvider, DefaultVerifier>
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
                    .WithSpan("Test0.cs", 13, 13, 22, 25)
                    .WithMessage("Rule builder from 'Rules.Framework' detected. Consider rewriting to Regulae API."));

            await test.RunAsync();
        }

        [Fact]
        public async Task RulesFramework_EmitsDiagnosticButNoRewriteWhenUsingRuleBuilderWithUnknownExtensionMethod()
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
                .WithDateBegin(2018, 1, 1)
                .WithContent(""test content type"", new object())
                .WithCondition(root => root
                .And(b => b
                .Value(""age"", Operators.GreaterThan, 18)
                .Value(""score"", Operators.GreaterThanOrEqual, 90)
                .Value(""color"", Operators.In, new[] { ""red"", ""blue"" })))
                .Build();
        }
    }
}

namespace Rules.Framework.Builder
{
    public static class RuleBuilderExtensions
    {
        public static IRuleBuilder<TContent, TCondition> WithDateBegin<TContent, TCondition>(this IRuleBuilder<TContent, TCondition> builder, int year, int month, int day)
            where TCondition : notnull
        {
            return builder.WithDateBegin(new DateTime(year, month, day));
        }
    }
}
";

            var test = new CSharpCodeFixTest<RulesFrameworkAnalyzer, RulesFrameworkCodeFixProvider, DefaultVerifier>
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
                    .WithSpan("Test0.cs", 13, 13, 22, 25)
                    .WithMessage("Rule builder from 'Rules.Framework' detected. Consider rewriting to Regulae API."));

            await test.RunAsync();
        }
    }
}