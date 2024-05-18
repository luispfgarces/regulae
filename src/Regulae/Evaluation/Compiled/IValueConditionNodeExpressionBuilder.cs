namespace Regulae.Evaluation.Compiled
{
    using Regulae.Evaluation.Compiled.ExpressionBuilders;

    internal interface IValueConditionNodeExpressionBuilder
    {
        void Build(
            IExpressionBlockBuilder builder,
            BuildValueConditionNodeExpressionArgs args);
    }
}