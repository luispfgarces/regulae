namespace Regulae.Evaluation.Compiled.ConditionBuilders
{
    using System;
    using System.Linq.Expressions;
    using Regulae;
    using Regulae.Evaluation.Compiled.ExpressionBuilders;
    using Regulae.Extensions;

    internal sealed class GreaterThanOrEqualOneToOneConditionExpressionBuilder : IConditionExpressionBuilder
    {
        public Expression BuildConditionExpression(IExpressionBlockBuilder builder, BuildConditionExpressionArgs args)
        {
            if (!args.DataTypeConfiguration.Type.HasLanguageOperator(LanguageOperator.GreaterThanOrEqual))
            {
                throw new NotSupportedException($"The operator '{Operators.GreaterThanOrEqual}' is not supported for data type '{args.DataTypeConfiguration.DataType}'.");
            }

            return builder.GreaterThanOrEqual(args.LeftHandOperand, args.RightHandOperand);
        }
    }
}