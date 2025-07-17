namespace Regulae.Evaluation.Compiled.ConditionBuilders
{
    using System;
    using System.Collections.Frozen;
    using System.Collections.Generic;
    using Regulae;
    using Regulae.Evaluation;

    internal sealed class ConditionExpressionBuilderProvider : IConditionExpressionBuilderProvider
    {
        private readonly FrozenDictionary<int, IConditionExpressionBuilder> conditionExpressionBuilders;

        public ConditionExpressionBuilderProvider()
        {
            this.conditionExpressionBuilders = new Dictionary<int, IConditionExpressionBuilder>()
            {
                { Combine(Operators.CaseInsensitiveEndsWith, Multiplicities.OneToOne), new CaseInsensitiveEndsWithOneToOneConditionExpressionBuilder() },
                { Combine(Operators.CaseInsensitiveStartsWith, Multiplicities.OneToOne), new CaseInsensitiveStartsWithOneToOneConditionExpressionBuilder() },
                { Combine(Operators.Contains, Multiplicities.ManyToOne), new ContainsManyToOneConditionExpressionBuilder() },
                { Combine(Operators.Contains, Multiplicities.OneToOne), new ContainsOneToOneConditionExpressionBuilder() },
                { Combine(Operators.EndsWith, Multiplicities.OneToOne), new EndsWithOneToOneConditionExpressionBuilder() },
                { Combine(Operators.Equal, Multiplicities.OneToOne), new EqualOneToOneConditionExpressionBuilder() },
                { Combine(Operators.GreaterThan, Multiplicities.OneToOne), new GreaterThanOneToOneConditionExpressionBuilder() },
                { Combine(Operators.GreaterThanOrEqual, Multiplicities.OneToOne), new GreaterThanOrEqualOneToOneConditionExpressionBuilder() },
                { Combine(Operators.In, Multiplicities.OneToMany), new InOneToManyConditionExpressionBuilder() },
                { Combine(Operators.LesserThan, Multiplicities.OneToOne), new LesserThanOneToOneConditionExpressionBuilder() },
                { Combine(Operators.LesserThanOrEqual, Multiplicities.OneToOne), new LesserThanOrEqualOneToOneConditionExpressionBuilder() },
                { Combine(Operators.NotContains, Multiplicities.OneToOne), new NotContainsOneToOneConditionExpressionBuilder() },
                { Combine(Operators.NotEndsWith, Multiplicities.OneToOne), new NotEndsWithOneToOneConditionExpressionBuilder() },
                { Combine(Operators.NotEqual, Multiplicities.OneToOne), new NotEqualOneToOneConditionExpressionBuilder() },
                { Combine(Operators.NotIn, Multiplicities.OneToMany), new NotInOneToManyConditionExpressionBuilder() },
                { Combine(Operators.NotStartsWith, Multiplicities.OneToOne), new NotStartsWithOneToOneConditionExpressionBuilder() },
                { Combine(Operators.StartsWith, Multiplicities.OneToOne), new StartsWithOneToOneConditionExpressionBuilder() },
            }.ToFrozenDictionary();
        }

        public IConditionExpressionBuilder GetConditionExpressionBuilderFor(Operators @operator, Multiplicities multiplicity)
        {
            if (this.conditionExpressionBuilders.TryGetValue(Combine(@operator, multiplicity), out var operatorEvalStrategy))
            {
                return operatorEvalStrategy;
            }

            throw new NotSupportedException($"Operator compilation is not supported for operator '{@operator}' with '{multiplicity}' multiplicity.");
        }

        private static int Combine(Operators @operator, Multiplicities multiplicity)
        {
            return ((byte)@operator << 2) | (byte)multiplicity;
        }
    }
}