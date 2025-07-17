namespace Regulae.Evaluation.Interpreted.ValueEvaluation
{
    using System;

    internal sealed class NotContainsOperatorEvalStrategy : IOneToOneOperatorEvalStrategy
    {
        public bool Eval(object leftOperand, object rightOperand)
        {
            if (leftOperand is string leftOperandAsString && rightOperand is string rightOperandAsString)
            {
                return !leftOperandAsString.Contains(rightOperandAsString);
            }

            throw new NotSupportedException($"Unsupported 'not contains' comparison between operands of type '{leftOperand?.GetType().FullName}'.");
        }
    }
}