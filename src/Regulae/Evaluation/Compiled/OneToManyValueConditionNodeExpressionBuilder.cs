namespace Regulae.Evaluation.Compiled
{
    using Regulae.Evaluation;
    using Regulae.Evaluation.Compiled.ConditionBuilders;
    using Regulae.Evaluation.Compiled.ExpressionBuilders;

    internal sealed class OneToManyValueConditionNodeExpressionBuilder : IValueConditionNodeExpressionBuilder
    {
        private readonly IConditionExpressionBuilderProvider conditionExpressionBuilderProvider;

        public OneToManyValueConditionNodeExpressionBuilder(IConditionExpressionBuilderProvider conditionExpressionBuilderProvider)
        {
            this.conditionExpressionBuilderProvider = conditionExpressionBuilderProvider;
        }

        public void Build(
            IExpressionBlockBuilder builder,
            BuildValueConditionNodeExpressionArgs args)
        {
            var enumerableOfDataType = args.DataTypeConfiguration.ManyCardinality.Type;

            // Line 1.
            var leftOperandExpression = builder.Condition(
                args.TestLeftOperand,
                builder.UnboxOrConvert(
                    args.LeftOperandExpression,
                    args.DataTypeConfiguration),
                builder.UnboxOrConvert(
                    builder.Constant(args.DataTypeConfiguration.OneCardinality.Default),
                    args.DataTypeConfiguration));

            // Line 2.
            var rightOperandExpression = builder.ConvertChecked(
                args.RightOperandExpression,
                enumerableOfDataType);

            // Line 3.
            var conditionExpressionBuilder = this.conditionExpressionBuilderProvider
                .GetConditionExpressionBuilderFor(args.Operator, Multiplicities.OneToMany);
            var buildConditionExpressionArgs = new BuildConditionExpressionArgs
            {
                DataTypeConfiguration = args.DataTypeConfiguration,
                LeftHandOperand = leftOperandExpression,
                RightHandOperand = rightOperandExpression,
            };
            builder.Assign(
                args.ResultVariableExpression,
                conditionExpressionBuilder.BuildConditionExpression(builder, buildConditionExpressionArgs));

            // Line 4.
            builder.AddExpression(builder.Empty());
        }
    }
}