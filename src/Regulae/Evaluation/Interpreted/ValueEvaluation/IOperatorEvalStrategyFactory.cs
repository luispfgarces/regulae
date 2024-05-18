namespace Regulae.Evaluation.Interpreted.ValueEvaluation
{
    using Regulae;

    internal interface IOperatorEvalStrategyFactory
    {
        IManyToManyOperatorEvalStrategy GetManyToManyOperatorEvalStrategy(Operators @operator);

        IManyToOneOperatorEvalStrategy GetManyToOneOperatorEvalStrategy(Operators @operator);

        IOneToManyOperatorEvalStrategy GetOneToManyOperatorEvalStrategy(Operators @operator);

        IOneToOneOperatorEvalStrategy GetOneToOneOperatorEvalStrategy(Operators @operator);
    }
}