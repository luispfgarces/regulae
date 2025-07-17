namespace Regulae.Evaluation.Interpreted.ValueEvaluation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal sealed class ContainsOperatorEvalStrategy : IOneToOneOperatorEvalStrategy, IManyToOneOperatorEvalStrategy
    {
        public bool Eval(object leftOperand, object rightOperand)
        {
            if (leftOperand is string leftOperandAsString && rightOperand is string rightOperandAsString)
            {
#if NETSTANDARD2_1_OR_GREATER
                return leftOperandAsString!.Contains(rightOperandAsString, StringComparison.Ordinal);
#else
                return leftOperandAsString!.Contains(rightOperandAsString);
#endif
            }

            throw new NotSupportedException($"Unsupported 'contains' comparison between operands of type '{leftOperand?.GetType().FullName}'.");
        }

        public bool Eval(IEnumerable<object> leftOperand, object rightOperand)
            => leftOperand.Contains(rightOperand);
    }
}