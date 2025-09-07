namespace Regulae.Evaluation.Interpreted.ValueEvaluation.Dispatchers
{
    using System;
    using System.Collections.Frozen;
    using System.Collections.Generic;
    using Regulae;
    using Regulae.Evaluation;
    using Regulae.Evaluation.Interpreted.ValueEvaluation;

    internal sealed class ConditionEvalDispatchProvider : IConditionEvalDispatcherProvider
    {
        private readonly FrozenDictionary<Multiplicities, IConditionEvalDispatcher> dispatchers;
        private readonly IMultiplicityEvaluator multiplicityEvaluator;

        public ConditionEvalDispatchProvider(
            IOperatorEvalStrategyFactory operatorEvalStrategyFactory,
            IMultiplicityEvaluator multiplicityEvaluator,
            IDataTypesConfigurationProvider dataTypesConfigurationProvider)
        {
            this.dispatchers = new Dictionary<Multiplicities, IConditionEvalDispatcher>()
            {
                { Multiplicities.OneToOne, new OneToOneConditionEvalDispatcher(operatorEvalStrategyFactory, dataTypesConfigurationProvider) },
                { Multiplicities.OneToMany, new OneToManyConditionEvalDispatcher(operatorEvalStrategyFactory, dataTypesConfigurationProvider) },
                { Multiplicities.ManyToOne, new ManyToOneConditionEvalDispatcher(operatorEvalStrategyFactory, dataTypesConfigurationProvider) },
                { Multiplicities.ManyToMany, new ManyToManyConditionEvalDispatcher(operatorEvalStrategyFactory, dataTypesConfigurationProvider) },
            }.ToFrozenDictionary();
            this.multiplicityEvaluator = multiplicityEvaluator;
        }

        public IConditionEvalDispatcher GetEvalDispatcher(Operand leftOperand, Operators @operator, Operand rightOperand)
        {
            var multiplicity = this.multiplicityEvaluator.EvaluateMultiplicity(leftOperand.Cardinality, rightOperand.Cardinality);

            ThrowIfUnsupportedOperandsAndOperatorCombination(@operator, multiplicity);

            return this.dispatchers[multiplicity];
        }

        private static void ThrowIfUnsupportedOperandsAndOperatorCombination(Operators @operator, Multiplicities multiplicity)
        {
            if (!OperatorsMetadata.AllByOperator.TryGetValue(@operator, out var operatorMetadata) && operatorMetadata!.SupportedMultiplicities.Contains(multiplicity))
            {
                throw new NotSupportedException($"The multiplicity '{multiplicity}' is not supported for operator '{@operator}'.");
            }
        }
    }
}