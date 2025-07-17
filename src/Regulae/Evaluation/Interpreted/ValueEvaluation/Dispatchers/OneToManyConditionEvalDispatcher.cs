namespace Regulae.Evaluation.Interpreted.ValueEvaluation.Dispatchers
{
    using System.Collections;
    using System.Linq;
    using Regulae;
    using Regulae.Evaluation;
    using Regulae.Evaluation.Interpreted.ValueEvaluation;

    internal sealed class OneToManyConditionEvalDispatcher : ConditionEvalDispatcherBase, IConditionEvalDispatcher
    {
        private readonly IOperatorEvalStrategyFactory operatorEvalStrategyFactory;

        public OneToManyConditionEvalDispatcher(
            IOperatorEvalStrategyFactory operatorEvalStrategyFactory,
            IDataTypesConfigurationProvider dataTypesConfigurationProvider)
            : base(dataTypesConfigurationProvider)
        {
            this.operatorEvalStrategyFactory = operatorEvalStrategyFactory;
        }

        public bool EvalDispatch(Operand leftOperand, Operators @operator, Operand rightOperand)
        {
            var dataTypeConfiguration = this.GetDataTypeConfiguration(rightOperand.DataType);

            return this.operatorEvalStrategyFactory.GetOneToManyOperatorEvalStrategy(@operator)
                .Eval(
                    CoalesceOne(leftOperand.Value!, dataTypeConfiguration),
                    ((IEnumerable)rightOperand.Value!).Cast<object>());
        }
    }
}