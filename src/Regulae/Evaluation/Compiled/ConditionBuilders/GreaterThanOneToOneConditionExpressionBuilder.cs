namespace Regulae.Evaluation.Compiled.ConditionBuilders
{
    using System;
    using System.Linq.Expressions;
    using Regulae;
    using Regulae.Evaluation.Compiled.ExpressionBuilders;
    using Regulae.Extensions;

    internal sealed class GreaterThanOneToOneConditionExpressionBuilder : IConditionExpressionBuilder
    {
        public Expression BuildConditionExpression(IExpressionBlockBuilder builder, BuildConditionExpressionArgs args)
        {
            if (!args.DataTypeConfiguration.OneCardinality.Type.HasLanguageOperator(LanguageOperator.GreaterThan))
            {
                throw new NotSupportedException($"The operator '{Operators.GreaterThan}' is not supported for data type '{args.DataTypeConfiguration.DataType}'.");
            }

            return builder.GreaterThan(args.LeftHandOperand, args.RightHandOperand);
        }
    }
}