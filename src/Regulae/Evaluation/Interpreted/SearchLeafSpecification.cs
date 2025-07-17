namespace Regulae.Evaluation.Interpreted
{
    using System.Collections.Generic;
    using Regulae.ConditionNodes;
    using Regulae.Evaluation.Interpreted.ValueEvaluation.Dispatchers;

    internal sealed class SearchLeafSpecification : SpecificationBase<IDictionary<string, Operand>>
    {
        private readonly IConditionEvalDispatcherProvider conditionEvalDispatchProvider;
        private readonly IValueConditionNode valueConditionNode;

        public SearchLeafSpecification(
            IValueConditionNode valueConditionNode,
            IConditionEvalDispatcherProvider conditionEvalDispatchProvider)
        {
            this.valueConditionNode = valueConditionNode;
            this.conditionEvalDispatchProvider = conditionEvalDispatchProvider;
        }

        public override bool IsSatisfiedBy(IDictionary<string, Operand> input)
        {
            var rightOperand = valueConditionNode.RightOperand;

            input.TryGetValue(valueConditionNode.Condition, out var leftOperand);

            if (leftOperand is null || leftOperand.Value is null)
            {
                // When match mode is search, if condition is missing, it is not used as search
                // criteria, so we don't filter out the rule.
                return true;
            }

            return this.conditionEvalDispatchProvider.GetEvalDispatcher(leftOperand, valueConditionNode.Operator, rightOperand)
                .EvalDispatch(leftOperand, valueConditionNode.Operator, rightOperand);
        }
    }
}