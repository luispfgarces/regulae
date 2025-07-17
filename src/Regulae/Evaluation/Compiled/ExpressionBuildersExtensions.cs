namespace Regulae.Evaluation.Compiled
{
    using System.Linq.Expressions;
    using Regulae.Evaluation.Compiled.ExpressionBuilders;

    internal static class ExpressionBuildersExtensions
    {
        public static Expression UnboxOrConvert(this IExpressionBlockBuilder builder, Expression expression, DataTypeConfiguration dataTypeConfiguration)
        {
            if (dataTypeConfiguration.DataType is DataTypes.Boolean or DataTypes.Decimal or DataTypes.Integer)
            {
                return builder.Unbox(expression, dataTypeConfiguration.OneCardinality.Type);
            }

            return builder.ConvertChecked(expression, dataTypeConfiguration.OneCardinality.Type);
        }
    }
}