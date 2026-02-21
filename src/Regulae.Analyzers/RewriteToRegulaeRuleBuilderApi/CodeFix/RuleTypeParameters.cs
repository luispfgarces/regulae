namespace Regulae.Analyzers.RewriteToRegulaeRuleBuilderApi.CodeFix
{
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal sealed class RuleTypeParameters
    {
        public TypeSyntax ConditionType { get; set; }

        public TypeSyntax RulesetType { get; set; }
    }
}
