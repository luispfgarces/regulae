namespace Regulae.Evaluation.Compiled.ConditionBuilders
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using Regulae;
    using Regulae.Evaluation.Compiled.ExpressionBuilders;

    internal class NotStartsWithOneToOneConditionExpressionBuilder : IConditionExpressionBuilder
    {
        private static readonly Type booleanType = typeof(bool);

        private static readonly MethodInfo startsWithMethodInfo = typeof(string).GetMethod(
            nameof(string.StartsWith),
            [typeof(string), typeof(StringComparison)])!;

        public Expression BuildConditionExpression(IExpressionBlockBuilder builder, BuildConditionExpressionArgs args)
        {
            if (args.DataTypeConfiguration.DataType != DataTypes.String)
            {
                throw new NotSupportedException($"The operator '{Operators.NotStartsWith}' is not supported for data type '{args.DataTypeConfiguration.DataType}'.");
            }

            return builder.Not(builder.Call(
                args.LeftHandOperand,
                startsWithMethodInfo,
                [args.RightHandOperand, builder.Constant(StringComparison.InvariantCulture)]));
        }
    }
}