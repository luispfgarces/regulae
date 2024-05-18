namespace Regulae.Evaluation.Interpreted.ValueEvaluation.Dispatchers
{
    using Regulae;

    internal interface IConditionEvalDispatchProvider
    {
        IConditionEvalDispatcher GetEvalDispatcher(object leftOperand, Operators @operator, object rightOperand);
    }
}