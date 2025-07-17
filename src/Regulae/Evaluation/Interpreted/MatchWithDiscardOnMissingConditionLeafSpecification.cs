namespace Regulae.Evaluation.Interpreted
{
    using System.Collections.Generic;
    using Regulae.ConditionNodes;
    using Regulae.Evaluation.Interpreted.ValueEvaluation.Dispatchers;

    internal sealed class MatchWithDiscardOnMissingConditionLeafSpecification : SpecificationBase<IDictionary<string, Operand>>
    {
        private readonly IConditionEvalDispatcherProvider conditionEvalDispatchProvider;
        private readonly IValueConditionNode valueConditionNode;

        public MatchWithDiscardOnMissingConditionLeafSpecification(
            IValueConditionNode valueConditionNode,
            IConditionEvalDispatcherProvider conditionEvalDispatchProvider)
        {
            this.valueConditionNode = valueConditionNode;
            this.conditionEvalDispatchProvider = conditionEvalDispatchProvider;
        }

        public override bool IsSatisfiedBy(IDictionary<string, Operand> input)
        {
            var rightOperand = this.valueConditionNode.RightOperand;

            input.TryGetValue(valueConditionNode.Condition, out var leftOperand);

            if (leftOperand is null || leftOperand.Value is null)
            {
                return false;
            }

            return this.conditionEvalDispatchProvider.GetEvalDispatcher(leftOperand, valueConditionNode.Operator, rightOperand)
                .EvalDispatch(leftOperand, valueConditionNode.Operator, rightOperand);
        }
    }
}