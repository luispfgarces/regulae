namespace Regulae.Evaluation.Interpreted.ValueEvaluation.Dispatchers
{
    using System.Collections.Generic;
    using Regulae;
    using Regulae.Evaluation;
    using Regulae.Evaluation.Interpreted.ValueEvaluation;

    internal sealed class ManyToManyConditionEvalDispatcher : ConditionEvalDispatcherBase, IConditionEvalDispatcher
    {
        private readonly IOperatorEvalStrategyFactory operatorEvalStrategyFactory;

        public ManyToManyConditionEvalDispatcher(
            IOperatorEvalStrategyFactory operatorEvalStrategyFactory,
            IDataTypesConfigurationProvider dataTypesConfigurationProvider)
            : base(dataTypesConfigurationProvider)
        {
            this.operatorEvalStrategyFactory = operatorEvalStrategyFactory;
        }

        public bool EvalDispatch(Operand leftOperand, Operators @operator, Operand rightOperand)
        {
            var dataTypeConfiguration = this.GetDataTypeConfiguration(rightOperand.DataType);

            return this.operatorEvalStrategyFactory.GetManyToManyOperatorEvalStrategy(@operator)
                .Eval(
                    CoalesceMany((IEnumerable<object>)leftOperand.Value!, dataTypeConfiguration),
                    (IEnumerable<object>)rightOperand.Value!);
        }
    }
}