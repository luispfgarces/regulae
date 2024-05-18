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

        public bool EvalDispatch(DataTypes dataType, object leftOperand, Operators @operator, object rightOperand)
        {
            var dataTypeConfiguration = this.GetDataTypeConfiguration(dataType);
            var leftOperandConverted = ConvertToDataType(leftOperand, nameof(leftOperand), dataTypeConfiguration);
            var rightOperandConverted = ConvertToDataType(rightOperand, nameof(rightOperand), dataTypeConfiguration);

            return this.operatorEvalStrategyFactory.GetOneToOneOperatorEvalStrategy(@operator).Eval(leftOperandConverted, rightOperandConverted);
        }
    }
}