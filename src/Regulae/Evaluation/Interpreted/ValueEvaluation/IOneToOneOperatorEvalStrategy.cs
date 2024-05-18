namespace Regulae.Evaluation.Interpreted.ValueEvaluation
{
    internal interface IOneToOneOperatorEvalStrategy
    {
        bool Eval(object leftOperand, object rightOperand);
    }
}