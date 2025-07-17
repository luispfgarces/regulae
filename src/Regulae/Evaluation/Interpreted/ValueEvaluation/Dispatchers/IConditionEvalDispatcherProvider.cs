namespace Regulae.Evaluation.Interpreted.ValueEvaluation.Dispatchers
{
    using Regulae;

    internal interface IConditionEvalDispatcherProvider
    {
        IConditionEvalDispatcher GetEvalDispatcher(Operand leftOperand, Operators @operator, Operand rightOperand);
    }
}