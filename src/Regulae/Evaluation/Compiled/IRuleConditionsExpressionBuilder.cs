namespace Regulae.Evaluation.Compiled
{
    using System;
    using System.Linq.Expressions;
    using Regulae;

    internal interface IRuleConditionsExpressionBuilder
    {
        Expression<Func<EvaluationContext, bool>> BuildExpression(IConditionNode rootConditionNode);
    }
}