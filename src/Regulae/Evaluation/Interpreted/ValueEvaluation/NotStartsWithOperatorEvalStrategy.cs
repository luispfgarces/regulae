namespace Regulae.Evaluation.Interpreted.ValueEvaluation
{
    using System;

    internal sealed class NotStartsWithOperatorEvalStrategy : IOneToOneOperatorEvalStrategy
    {
        public bool Eval(object leftOperand, object rightOperand)
        {
            if (leftOperand is string leftOperandAsString && rightOperand is string rightOperandAsString)
            {
                return !leftOperandAsString.StartsWith(rightOperandAsString, StringComparison.Ordinal);
            }

            throw new NotSupportedException($"Only operands of type {nameof(String)} supported.");
        }
    }
}