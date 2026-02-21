namespace Regulae.Analyzers.RewriteToRegulaeRuleBuilderApi.CodeFix
{
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal sealed class ConditionParameters
    {
        public ExpressionSyntax RawConditionExpression { get; set; }

        public LambdaExpressionSyntax ConditionLambda { get; set; }

        public ExtractedValueConditionNode SimpleValueCondition { get; set; }
    }
}