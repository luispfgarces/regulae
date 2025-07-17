namespace Regulae.Evaluation.Compiled
{
    using System;
    using System.Collections.Frozen;
    using System.Collections.Generic;
    using Regulae.Evaluation;
    using Regulae.Evaluation.Compiled.ConditionBuilders;

    internal sealed class ValueConditionNodeExpressionBuilderProvider : IValueConditionNodeExpressionBuilderProvider
    {
        private readonly FrozenDictionary<Multiplicities, IValueConditionNodeExpressionBuilder> compilers;

        public ValueConditionNodeExpressionBuilderProvider(
            IConditionExpressionBuilderProvider conditionExpressionBuilderProvider)
        {
            this.compilers = new Dictionary<Multiplicities, IValueConditionNodeExpressionBuilder>()
            {
                { Multiplicities.OneToOne, new OneToOneValueConditionNodeExpressionBuilder(conditionExpressionBuilderProvider) },
                { Multiplicities.OneToMany, new OneToManyValueConditionNodeExpressionBuilder(conditionExpressionBuilderProvider) },
                { Multiplicities.ManyToOne, new ManyToOneValueConditionNodeExpressionBuilder(conditionExpressionBuilderProvider) },
                { Multiplicities.ManyToMany, new ManyToManyValueConditionNodeExpressionBuilder(conditionExpressionBuilderProvider) },
            }.ToFrozenDictionary();
        }

        public IValueConditionNodeExpressionBuilder GetExpressionBuilder(Multiplicities multiplicity)
        {
            if (this.compilers.TryGetValue(multiplicity, out var compiler))
            {
                return compiler;
            }

            throw new NotSupportedException($"No compiler for multiplicity '{multiplicity}' defined.");
        }
    }
}