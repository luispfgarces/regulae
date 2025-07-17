namespace Regulae.Evaluation
{
    using Regulae;

    internal sealed class MultiplicityEvaluator : IMultiplicityEvaluator
    {
        public static Multiplicities Evaluate(Cardinalities leftOperandCardinality, Cardinalities rightOperandCardinality)
            => (Multiplicities)(((int)leftOperandCardinality << 1) | (int)rightOperandCardinality);

        public Multiplicities EvaluateMultiplicity(Cardinalities leftOperandCardinality, Cardinalities rightOperandCardinality)
            => Evaluate(leftOperandCardinality, rightOperandCardinality);
    }
}