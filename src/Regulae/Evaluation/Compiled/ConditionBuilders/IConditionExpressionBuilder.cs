namespace Regulae.Evaluation.Compiled.ConditionBuilders
{
    using System.Linq.Expressions;
    using Regulae.Evaluation.Compiled.ExpressionBuilders;

    internal interface IConditionExpressionBuilder
    {
        Expression BuildConditionExpression(IExpressionBlockBuilder builder, BuildConditionExpressionArgs args);
    }
}