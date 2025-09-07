namespace Regulae.Evaluation.Compiled
{
    using System.Linq;
    using System.Reflection;
    using Regulae.Evaluation;
    using Regulae.Evaluation.Compiled.ConditionBuilders;
    using Regulae.Evaluation.Compiled.ExpressionBuilders;

    internal sealed class ManyToManyValueConditionNodeExpressionBuilder : IValueConditionNodeExpressionBuilder
    {
        private static readonly MethodInfo enumerableEmptyMethodInfo = typeof(Enumerable).GetMethod(nameof(Enumerable.Empty))!;
        private readonly IConditionExpressionBuilderProvider conditionExpressionBuilderProvider;

        public ManyToManyValueConditionNodeExpressionBuilder(IConditionExpressionBuilderProvider conditionExpressionBuilderProvider)
        {
            this.conditionExpressionBuilderProvider = conditionExpressionBuilderProvider;
        }

        public void Build(
            IExpressionBlockBuilder builder,
            BuildValueConditionNodeExpressionArgs args)
        {
            var enumerableOfDataType = args.DataTypeConfiguration.ManyCardinality.Type;
            var enumerableEmptyOfDataTypeMethodInfo = enumerableEmptyMethodInfo
                .MakeGenericMethod(args.DataTypeConfiguration.OneCardinality.Type);

            // Line 1.
            var leftOperandExpression = builder.Condition(
                args.TestLeftOperand,
                builder.ConvertChecked(
                    args.LeftOperandExpression,
                    enumerableOfDataType),
                builder.Call(null!, enumerableEmptyOfDataTypeMethodInfo));

            // Line 2.
            var rightOperandExpression = builder.ConvertChecked(args.RightOperandExpression, enumerableOfDataType);

            // Line 3.
            var conditionExpressionBuilder = this.conditionExpressionBuilderProvider
                .GetConditionExpressionBuilderFor(args.Operator, Multiplicities.ManyToMany);
            var buildConditionExpressionArgs = new BuildConditionExpressionArgs
            {
                DataTypeConfiguration = args.DataTypeConfiguration,
                LeftHandOperand = leftOperandExpression,
                RightHandOperand = rightOperandExpression,
            };
            builder.Assign(
                args.ResultVariableExpression,
                conditionExpressionBuilder.BuildConditionExpression(builder, buildConditionExpressionArgs));

            // Line 5.
            builder.AddExpression(builder.Empty());
        }
    }
}