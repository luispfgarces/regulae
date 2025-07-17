namespace Regulae.Evaluation.Interpreted.ValueEvaluation.Dispatchers
{
    using Regulae;

    internal interface IConditionEvalDispatcher
    {
        bool EvalDispatch(Operand leftOperand, Operators @operator, Operand rightOperand);
    }
}