namespace Regulae.Evaluation.Compiled.ExpressionBuilders
{
    using Regulae.Evaluation.Compiled.ExpressionBuilders.StateMachine;

    internal interface IExpressionBuilderFactory
    {
        IExpressionBlockBuilder CreateExpressionBlockBuilder(
            string scopeName,
            IExpressionBlockBuilder parent,
            ExpressionConfiguration expressionConfiguration);

        IExpressionParametersBuilder CreateExpressionBuilder(string name);

        IExpressionParametersConfiguration CreateExpressionParametersConfiguration();

        IExpressionSwitchBuilder CreateExpressionSwitchBuilder(
            IExpressionBlockBuilder parent);
    }
}