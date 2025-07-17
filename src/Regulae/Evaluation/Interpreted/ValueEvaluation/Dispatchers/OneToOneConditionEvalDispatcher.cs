namespace Regulae.Evaluation.Interpreted.ValueEvaluation.Dispatchers
{
    using Regulae;
    using Regulae.Evaluation;
    using Regulae.Evaluation.Interpreted.ValueEvaluation;

    internal sealed class OneToOneConditionEvalDispatcher : ConditionEvalDispatcherBase, IConditionEvalDispatcher
    {
        private readonly IOperatorEvalStrategyFactory operatorEvalStrategyFactory;

        public OneToOneConditionEvalDispatcher(
            IOperatorEvalStrategyFactory operatorEvalStrategyFactory,
            IDataTypesConfigurationProvider dataTypesConfigurationProvider)
            : base(dataTypesConfigurationProvider)
        {
            this.operatorEvalStrategyFactory = operatorEvalStrategyFactory;
        }

        public bool EvalDispatch(Operand leftOperand, Operators @operator, Operand rightOperand)
        {
            var dataTypeConfiguration = this.GetDataTypeConfiguration(rightOperand.DataType);

            return this.operatorEvalStrategyFactory.GetOneToOneOperatorEvalStrategy(@operator)
                .Eval(
                    CoalesceOne(leftOperand.Value!, dataTypeConfiguration),
                    rightOperand.Value!);
        }
    }
}