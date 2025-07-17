namespace Regulae.Evaluation.Compiled.ConditionBuilders
{
    using Regulae;

    internal interface IConditionExpressionBuilderProvider
    {
        IConditionExpressionBuilder GetConditionExpressionBuilderFor(Operators @operator, Multiplicities multiplicity);
    }
}