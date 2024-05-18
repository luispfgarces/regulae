namespace Regulae.Evaluation.Compiled.ConditionBuilders
{
    using System;
    using System.Linq.Expressions;
    using Regulae;
    using Regulae.Evaluation.Compiled.ExpressionBuilders;
    using Regulae.Extensions;

    internal sealed class LesserThanOneToOneConditionExpressionBuilder : IConditionExpressionBuilder
    {
        public Expression BuildConditionExpression(IExpressionBlockBuilder builder, BuildConditionExpressionArgs args)
        {
            if (!args.DataTypeConfiguration.Type.HasLanguageOperator(LanguageOperator.LessThan))
            {
                throw new NotSupportedException($"The operator '{Operators.LesserThan}' is not supported for data type '{args.DataTypeConfiguration.DataType}'.");
            }

            return builder.LessThan(args.LeftHandOperand, args.RightHandOperand);
        }
    }
}