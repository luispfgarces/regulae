namespace Regulae.Evaluation.Compiled
{
    internal interface IValueConditionNodeExpressionBuilderProvider
    {
        IValueConditionNodeExpressionBuilder GetExpressionBuilder(string multiplicity);
    }
}