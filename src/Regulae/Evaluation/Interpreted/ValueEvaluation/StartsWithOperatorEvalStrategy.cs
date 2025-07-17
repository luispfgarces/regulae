namespace Regulae.Evaluation.Interpreted.ValueEvaluation
{
    using System;

    internal sealed class StartsWithOperatorEvalStrategy : IOneToOneOperatorEvalStrategy
    {
        public bool Eval(object leftOperand, object rightOperand)
        {
            if (leftOperand is string leftOperandAsString && rightOperand is string rightOperandAsString)
            {
                return leftOperandAsString.StartsWith(rightOperandAsString, StringComparison.Ordinal);
            }

            throw new NotSupportedException($"Unsupported 'startswith' comparison between operands of type '{leftOperand?.GetType().FullName}' and '{rightOperand?.GetType().FullName}'.");
        }
    }
}