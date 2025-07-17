namespace Regulae.Evaluation.Interpreted.ValueEvaluation
{
    using System;
    using System.Globalization;

    internal sealed class CaseInsensitiveEndsWithOperatorEvalStrategy : IOneToOneOperatorEvalStrategy
    {
        public bool Eval(object leftOperand, object rightOperand)
        {
            if (leftOperand is string leftOperandAsString && rightOperand is string rightOperandAsString)
            {
                return leftOperandAsString.EndsWith(rightOperandAsString, ignoreCase: true, culture: CultureInfo.InvariantCulture);
            }

            throw new NotSupportedException($"Unsupported 'caseinsensitiveendswith' comparison between operands of type '{leftOperand?.GetType().FullName}' and '{rightOperand?.GetType().FullName}'.");
        }
    }
}