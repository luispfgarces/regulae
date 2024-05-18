namespace Regulae.Evaluation
{
    using Regulae;

    internal interface IMultiplicityEvaluator
    {
        string EvaluateMultiplicity(object leftOperand, Operators @operator, object rightOperand);
    }
}