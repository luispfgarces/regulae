namespace Regulae.Analyzers.Tests.RewriteToRegulaeRuleAddPriorityOption
{
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp.Testing;
    using Microsoft.CodeAnalysis.Testing;
    using Regulae.Analyzers.RewriteToRegulaeRuleAddPriorityOption;
    using Xunit;

    public class RewriteToRegulaeRuleAddPriorityOptionTests
    {
        [Fact]
        public async Task RulesFramework_RewriteAtBottom()
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
            var x = RuleAddPriorityOption.AtBottom;
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
            var x = global::Regulae.RuleAddPriorityOption.AtLargestNumber;
        }
    }
}
";

            var test = new CSharpCodeFixTest<RuleAddPriorityOptionAnalyzer, RuleAddPriorityOptionCodeFixProvider, DefaultVerifier>
            {
                CodeActionEquivalenceKey = "RewriteToRegulaeRuleAddPriorityOption",
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
                new DiagnosticResult(RegulaeDiagnostics.RewriteToRegulaeRuleAddPriorityOptionId, DiagnosticSeverity.Info)
                    .WithSpan("Test0.cs", 12, 21, 12, 51)
                    .WithMessage("RuleAddPriorityOption from 'Rules.Framework' detected. Consider rewriting to Regulae RuleAddPriorityOption."));

            await test.RunAsync();
        }

        [Fact]
        public async Task RulesFramework_RewriteAtTop()
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
            var x = RuleAddPriorityOption.AtTop;
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
            var x = global::Regulae.RuleAddPriorityOption.AtSmallestNumber;
        }
    }
}
";

            var test = new CSharpCodeFixTest<RuleAddPriorityOptionAnalyzer, RuleAddPriorityOptionCodeFixProvider, DefaultVerifier>
            {
                CodeActionEquivalenceKey = "RewriteToRegulaeRuleAddPriorityOption",
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
                new DiagnosticResult(RegulaeDiagnostics.RewriteToRegulaeRuleAddPriorityOptionId, DiagnosticSeverity.Info)
                    .WithSpan("Test0.cs", 12, 21, 12, 48)
                    .WithMessage("RuleAddPriorityOption from 'Rules.Framework' detected. Consider rewriting to Regulae RuleAddPriorityOption."));

            await test.RunAsync();
        }

        [Fact]
        public async Task RulesFramework_RewriteMultipleUsages()
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
            var x = RuleAddPriorityOption.AtBottom;
            var y = RuleAddPriorityOption.AtTop;
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
            var x = global::Regulae.RuleAddPriorityOption.AtLargestNumber;
            var y = global::Regulae.RuleAddPriorityOption.AtSmallestNumber;
        }
    }
}
";

            var test = new CSharpCodeFixTest<RuleAddPriorityOptionAnalyzer, RuleAddPriorityOptionCodeFixProvider, DefaultVerifier>
            {
                CodeActionEquivalenceKey = "RewriteToRegulaeRuleAddPriorityOption",
                NumberOfIncrementalIterations = 2
            };

            test.TestState.Sources.Add(("Test0.cs", testCode));
            test.TestState.ReferenceAssemblies = ReferenceAssemblies.Net.Net80
                .AddPackages([new PackageIdentity("Rules.Framework", "2.0.8")]);

            test.FixedState.Sources.Add(("Test0.cs", fixedCode));
            test.FixedState.ReferenceAssemblies = ReferenceAssemblies.Net.Net80
                .AddPackages([new PackageIdentity("Rules.Framework", "2.0.8")]);
            test.FixedState.AdditionalReferences.Add(typeof(Rule).Assembly);

            test.ExpectedDiagnostics.Add(
                new DiagnosticResult(RegulaeDiagnostics.RewriteToRegulaeRuleAddPriorityOptionId, DiagnosticSeverity.Info)
                    .WithSpan("Test0.cs", 12, 21, 12, 51));
            test.ExpectedDiagnostics.Add(
                new DiagnosticResult(RegulaeDiagnostics.RewriteToRegulaeRuleAddPriorityOptionId, DiagnosticSeverity.Info)
                    .WithSpan("Test0.cs", 13, 21, 13, 48));

            await test.RunAsync();
        }

        [Fact]
        public async Task RulesFramework_RewriteByPriorityNumber()
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
            var x = RuleAddPriorityOption.ByPriorityNumber(10);
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
            var x = global::Regulae.RuleAddPriorityOption.AtNumber(10);
        }
    }
}
";

            var test = new CSharpCodeFixTest<RuleAddPriorityOptionAnalyzer, RuleAddPriorityOptionCodeFixProvider, DefaultVerifier>
            {
                CodeActionEquivalenceKey = "RewriteToRegulaeRuleAddPriorityOption",
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
                new DiagnosticResult(RegulaeDiagnostics.RewriteToRegulaeRuleAddPriorityOptionId, DiagnosticSeverity.Info)
                    .WithSpan("Test0.cs", 12, 21, 12, 63)
                    .WithMessage("RuleAddPriorityOption from 'Rules.Framework' detected. Consider rewriting to Regulae RuleAddPriorityOption."));

            await test.RunAsync();
        }

        [Fact]
        public async Task RulesFramework_RewriteByRuleName()
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
            var x = RuleAddPriorityOption.ByRuleName(""SomeRule"");
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
            var x = global::Regulae.RuleAddPriorityOption.AtRuleName(""SomeRule"");
        }
    }
}
";

            var test = new CSharpCodeFixTest<RuleAddPriorityOptionAnalyzer, RuleAddPriorityOptionCodeFixProvider, DefaultVerifier>
            {
                CodeActionEquivalenceKey = "RewriteToRegulaeRuleAddPriorityOption",
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
                new DiagnosticResult(RegulaeDiagnostics.RewriteToRegulaeRuleAddPriorityOptionId, DiagnosticSeverity.Info)
                    .WithSpan("Test0.cs", 12, 21, 12, 65)
                    .WithMessage("RuleAddPriorityOption from 'Rules.Framework' detected. Consider rewriting to Regulae RuleAddPriorityOption."));

            await test.RunAsync();
        }
    }
}
