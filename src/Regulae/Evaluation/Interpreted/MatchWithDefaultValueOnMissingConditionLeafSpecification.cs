namespace Regulae.Evaluation.Interpreted
{
    using System.Collections.Generic;
    using Regulae.ConditionNodes;
    using Regulae.Evaluation.Interpreted.ValueEvaluation.Dispatchers;

    internal sealed class MatchWithDefaultValueOnMissingConditionLeafSpecification : SpecificationBase<IDictionary<string, Operand>>
    {
        private readonly IConditionEvalDispatcherProvider conditionEvalDispatcherProvider;
        private readonly IValueConditionNode valueConditionNode;

        public MatchWithDefaultValueOnMissingConditionLeafSpecification(
            IValueConditionNode valueConditionNode,
            IConditionEvalDispatcherProvider conditionEvalDispatchProvider)
        {
            this.valueConditionNode = valueConditionNode;
            this.conditionEvalDispatcherProvider = conditionEvalDispatchProvider;
        }

        public override bool IsSatisfiedBy(IDictionary<string, Operand> input)
        {
            var rightOperand = valueConditionNode.RightOperand;

            input.TryGetValue(valueConditionNode.Condition, out var leftOperand);
            leftOperand ??= Operand.DefaultFor(rightOperand.DataType);

            return this.conditionEvalDispatcherProvider.GetEvalDispatcher(leftOperand, valueConditionNode.Operator, rightOperand)
                .EvalDispatch(leftOperand, valueConditionNode.Operator, rightOperand);
        }
    }
}