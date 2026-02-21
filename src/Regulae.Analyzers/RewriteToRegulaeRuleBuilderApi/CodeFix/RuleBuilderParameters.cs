namespace Regulae.Analyzers.RewriteToRegulaeRuleBuilderApi.CodeFix
{
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal sealed class RuleBuilderParameters
    {
        public ExpressionSyntax Active { get; set; }
        public ExpressionSyntax Content { get; set; }
        public ExpressionSyntax DateBegin { get; set; }
        public ExpressionSyntax DateEnd { get; set; }
        public ExpressionSyntax Name { get; set; }
        public ExpressionSyntax Ruleset { get; set; }
        public RuleTypeParameters TypeParameters { get; } = new RuleTypeParameters();
        public ConditionParameters Condition { get; } = new ConditionParameters();
    }
}
