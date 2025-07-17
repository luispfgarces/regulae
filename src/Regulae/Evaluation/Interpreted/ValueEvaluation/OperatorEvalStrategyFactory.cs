namespace Regulae.Evaluation.Interpreted.ValueEvaluation
{
    using System;
    using System.Collections.Frozen;
    using System.Collections.Generic;
    using Regulae;

    internal sealed class OperatorEvalStrategyFactory : IOperatorEvalStrategyFactory
    {
        private readonly FrozenDictionary<Operators, object> strategies;

        public OperatorEvalStrategyFactory()
        {
            this.strategies = new Dictionary<Operators, object>
            {
                { Operators.Equal, new EqualOperatorEvalStrategy() },
                { Operators.NotEqual, new NotEqualOperatorEvalStrategy() },
                { Operators.GreaterThan, new GreaterThanOperatorEvalStrategy() },
                { Operators.GreaterThanOrEqual, new GreaterThanOrEqualOperatorEvalStrategy() },
                { Operators.LesserThan, new LesserThanOperatorEvalStrategy() },
                { Operators.LesserThanOrEqual, new LesserThanOrEqualOperatorEvalStrategy() },
                { Operators.Contains, new ContainsOperatorEvalStrategy() },
                { Operators.NotContains, new NotContainsOperatorEvalStrategy() },
                { Operators.In, new InOperatorEvalStrategy() },
                { Operators.NotIn, new NotInOperatorEvalStrategy() },
                { Operators.StartsWith, new StartsWithOperatorEvalStrategy() },
                { Operators.EndsWith, new EndsWithOperatorEvalStrategy() },
                { Operators.CaseInsensitiveStartsWith, new CaseInsensitiveStartsWithOperatorEvalStrategy() },
                { Operators.CaseInsensitiveEndsWith, new CaseInsensitiveEndsWithOperatorEvalStrategy() },
                { Operators.NotStartsWith, new NotStartsWithOperatorEvalStrategy() },
                { Operators.NotEndsWith, new NotEndsWithOperatorEvalStrategy() }
            }.ToFrozenDictionary();
        }

        public IManyToManyOperatorEvalStrategy GetManyToManyOperatorEvalStrategy(Operators @operator)
        {
            if (this.strategies.TryGetValue(@operator, out var strategy) && strategy is IManyToManyOperatorEvalStrategy operatorEvalStrategy)
            {
                return operatorEvalStrategy;
            }

            throw new NotSupportedException($"Operator evaluation is not supported for operator '{@operator}' on the context of {nameof(IManyToManyOperatorEvalStrategy)}.");
        }

        public IManyToOneOperatorEvalStrategy GetManyToOneOperatorEvalStrategy(Operators @operator)
        {
            if (this.strategies.TryGetValue(@operator, out var strategy) && strategy is IManyToOneOperatorEvalStrategy operatorEvalStrategy)
            {
                return operatorEvalStrategy;
            }

            throw new NotSupportedException($"Operator evaluation is not supported for operator '{@operator}' on the context of {nameof(IManyToOneOperatorEvalStrategy)}.");
        }

        public IOneToManyOperatorEvalStrategy GetOneToManyOperatorEvalStrategy(Operators @operator)
        {
            if (this.strategies.TryGetValue(@operator, out var strategy) && strategy is IOneToManyOperatorEvalStrategy operatorEvalStrategy)
            {
                return operatorEvalStrategy;
            }

            throw new NotSupportedException($"Operator evaluation is not supported for operator '{@operator}' on the context of {nameof(IOneToManyOperatorEvalStrategy)}.");
        }

        public IOneToOneOperatorEvalStrategy GetOneToOneOperatorEvalStrategy(Operators @operator)
        {
            if (this.strategies.TryGetValue(@operator, out var strategy) && strategy is IOneToOneOperatorEvalStrategy operatorEvalStrategy)
            {
                return operatorEvalStrategy;
            }

            throw new NotSupportedException($"Operator evaluation is not supported for operator '{@operator}' on the context of {nameof(IOneToOneOperatorEvalStrategy)}.");
        }
    }
}