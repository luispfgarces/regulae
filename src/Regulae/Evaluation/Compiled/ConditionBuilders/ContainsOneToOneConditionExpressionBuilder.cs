namespace Regulae.Evaluation.Compiled.ConditionBuilders
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Regulae;
    using Regulae.Evaluation.Compiled.ExpressionBuilders;

    internal sealed class ContainsOneToOneConditionExpressionBuilder : IConditionExpressionBuilder
    {
        private static readonly MethodInfo stringContainsMethodInfo = typeof(string).GetMethod(
            nameof(Enumerable.Contains),
            [typeof(string)])!;

        public Expression BuildConditionExpression(IExpressionBlockBuilder builder, BuildConditionExpressionArgs args)
        {
            if (args.DataTypeConfiguration.DataType != DataTypes.String)
            {
                throw new NotSupportedException(
                    $"The operator '{nameof(Operators.Contains)}' is not supported for data type '{args.DataTypeConfiguration.DataType}' on a one to one scenario.");
            }

            return builder.Call(
                args.LeftHandOperand,
                stringContainsMethodInfo,
                [args.RightHandOperand]);
        }
    }
}