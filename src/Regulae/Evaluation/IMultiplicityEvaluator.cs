namespace Regulae.Evaluation
{
    using Regulae;

    internal interface IMultiplicityEvaluator
    {
        Multiplicities EvaluateMultiplicity(Cardinalities leftOperandCardinality, Cardinalities rightOperandCardinality);
    }
}