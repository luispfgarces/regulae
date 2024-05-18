namespace Regulae.Evaluation.Compiled.ConditionBuilders
{
    using System.Linq.Expressions;
    using Regulae.Evaluation.Compiled.ExpressionBuilders;

    internal sealed class NotEqualOneToOneConditionExpressionBuilder : IConditionExpressionBuilder
    {
        public Expression BuildConditionExpression(IExpressionBlockBuilder builder, BuildConditionExpressionArgs args)
        {
            return builder.NotEqual(args.LeftHandOperand, args.RightHandOperand);
        }
    }
}