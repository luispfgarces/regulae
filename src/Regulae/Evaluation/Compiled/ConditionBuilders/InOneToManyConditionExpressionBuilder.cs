namespace Regulae.Evaluation.Compiled.ConditionBuilders
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Regulae.Evaluation.Compiled.ExpressionBuilders;

    internal sealed class InOneToManyConditionExpressionBuilder : IConditionExpressionBuilder
    {
        private static readonly MethodInfo enumerableGenericContains
            = typeof(Enumerable)
                .GetMethods()
                .First(m => string.Equals(m.Name, "Contains", StringComparison.Ordinal) && m.GetParameters().Length == 2);

        public Expression BuildConditionExpression(IExpressionBlockBuilder builder, BuildConditionExpressionArgs args)
        {
            return builder.Call(
                instance: null!,
                enumerableGenericContains.MakeGenericMethod(args.DataTypeConfiguration.OneCardinality.Type),
                [args.RightHandOperand, args.LeftHandOperand]);
        }
    }
}