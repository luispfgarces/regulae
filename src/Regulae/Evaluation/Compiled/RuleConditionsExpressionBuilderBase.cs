namespace Regulae.Evaluation.Compiled
{
    using System.Reflection;
    using Regulae.Evaluation;

    internal class RuleConditionsExpressionBuilderBase
    {
        protected static readonly MethodInfo multiplicityEvaluateMethod = typeof(MultiplicityEvaluator)
            .GetMethod(nameof(MultiplicityEvaluator.Evaluate));

        protected RuleConditionsExpressionBuilderBase()
        {
        }
    }
}