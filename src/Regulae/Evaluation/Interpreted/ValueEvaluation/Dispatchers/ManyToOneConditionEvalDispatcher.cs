namespace Regulae.Evaluation.Interpreted.ValueEvaluation.Dispatchers
{
    using System.Collections.Generic;
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

        public bool EvalDispatch(Operand leftOperand, Operators @operator, Operand rightOperand)
        {
            var dataTypeConfiguration = this.GetDataTypeConfiguration(rightOperand.DataType);

            return this.operatorEvalStrategyFactory.GetManyToOneOperatorEvalStrategy(@operator)
                .Eval(
                    CoalesceMany((IEnumerable<object>)leftOperand.Value!, dataTypeConfiguration),
                    rightOperand.Value!);
        }
    }
}