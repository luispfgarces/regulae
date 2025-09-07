namespace Regulae.Evaluation.Compiled.ConditionBuilders
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using Regulae;
    using Regulae.Evaluation.Compiled.ExpressionBuilders;

    internal sealed class NotContainsOneToOneConditionExpressionBuilder : IConditionExpressionBuilder
    {
        private static readonly MethodInfo stringContainsMethodInfo = typeof(string).GetMethod("Contains", [typeof(string)])!;

        public Expression BuildConditionExpression(IExpressionBlockBuilder builder, BuildConditionExpressionArgs args)
        {
            if (args.DataTypeConfiguration.DataType != DataTypes.String)
            {
                throw new NotSupportedException($"The operator '{Operators.NotContains}' is not supported for data type '{args.DataTypeConfiguration.DataType}'.");
            }

            return builder.Not(builder.Call(
                args.LeftHandOperand,
                stringContainsMethodInfo,
                [args.RightHandOperand]));
        }
    }
}