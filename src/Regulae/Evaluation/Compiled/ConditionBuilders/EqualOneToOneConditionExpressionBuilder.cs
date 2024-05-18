namespace Regulae.Evaluation.Compiled.ConditionBuilders
{
    using System.Linq.Expressions;
    using Regulae.Evaluation.Compiled.ExpressionBuilders;

    internal sealed class EqualOneToOneConditionExpressionBuilder : IConditionExpressionBuilder
    {
        public Expression BuildConditionExpression(IExpressionBlockBuilder builder, BuildConditionExpressionArgs args)
        {
            return builder.Equal(args.LeftHandOperand, args.RightHandOperand);
        }
    }
}