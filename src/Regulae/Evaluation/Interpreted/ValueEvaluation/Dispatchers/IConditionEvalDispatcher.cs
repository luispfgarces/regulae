namespace Regulae.Evaluation.Interpreted.ValueEvaluation.Dispatchers
{
    using Regulae;

    internal interface IConditionEvalDispatcher
    {
        bool EvalDispatch(DataTypes dataType, object leftOperand, Operators @operator, object rightOperand);
    }
}