namespace Regulae.Evaluation.Interpreted.ValueEvaluation.Dispatchers
{
    using System.Collections.Generic;
    using System.Linq;
    using Regulae;
    using Regulae.Evaluation;
    using Regulae.Evaluation.Interpreted.ValueEvaluation;

    internal sealed class ManyToOneConditionEvalDispatcher : ConditionEvalDispatcherBase, IConditionEvalDispatcher
    {
        private readonly IOperatorEvalStrategyFactory operatorEvalStrategyFactory;

        public ManyToOneConditionEvalDispatcher(
            IOperatorEvalStrategyFactory operatorEvalStrategyFactory,
            IDataTypesConfigurationProvider dataTypesConfigurationProvider)
            : base(dataTypesConfigurationProvider)
        {
            this.operatorEvalStrategyFactory = operatorEvalStrategyFactory;
        }

        public bool EvalDispatch(DataTypes dataType, object leftOperand, Operators @operator, object rightOperand)
        {
            var dataTypeConfiguration = this.GetDataTypeConfiguration(dataType);

            var leftOperandAux = ConvertToTypedEnumerable(leftOperand, nameof(leftOperand));
            var leftOperandConverted = leftOperandAux.Select(x => ConvertToDataType(x, nameof(leftOperand), dataTypeConfiguration));
            var rightOperandConverted = ConvertToDataType(rightOperand, nameof(rightOperand), dataTypeConfiguration);

            return this.operatorEvalStrategyFactory.GetManyToOneOperatorEvalStrategy(@operator).Eval(leftOperandConverted, rightOperandConverted);
        }
    }
}