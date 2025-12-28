namespace Regulae.Evaluation.Interpreted
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Regulae;
    using Regulae.ConditionNodes;
    using Regulae.Evaluation;
    using Regulae.Evaluation.Interpreted.ValueEvaluation.Dispatchers;

    internal sealed class InterpretedConditionsEvalEngine : IConditionsEvalEngine
    {
        private readonly IConditionEvalDispatcherProvider conditionsEvalDispatchProvider;
        private readonly IConditionsTreeAnalyzer conditionsTreeAnalyzer;
        private readonly RulesEngineOptions rulesEngineOptions;

        public InterpretedConditionsEvalEngine(
            IConditionEvalDispatcherProvider conditionsEvalDispatchProvider,
            IConditionsTreeAnalyzer conditionsTreeAnalyzer,
            RulesEngineOptions rulesEngineOptions)
        {
            this.conditionsEvalDispatchProvider = conditionsEvalDispatchProvider;
            this.conditionsTreeAnalyzer = conditionsTreeAnalyzer;
            this.rulesEngineOptions = rulesEngineOptions;
        }

        public bool Eval(IConditionNode conditionNode, IDictionary<string, Operand> conditions, EvaluationOptions evaluationOptions)
        {
            if (evaluationOptions.ExcludeRulesWithoutSearchConditions && !this.conditionsTreeAnalyzer.AreAllSearchConditionsPresent(conditionNode, conditions))
            {
                return false;
            }

            var specification = this.BuildSpecification(conditionNode, evaluationOptions.MatchMode);

            return specification.IsSatisfiedBy(conditions);
        }

        private ISpecification<IDictionary<string, Operand>> BuildSpecification(IConditionNode conditionNode, MatchModes matchMode)
        {
            return conditionNode switch
            {
                IValueConditionNode valueConditionNode => this.BuildSpecificationForValueNode(valueConditionNode, matchMode),
                ComposedConditionNode composedConditionNode => this.BuildSpecificationForComposedNode(composedConditionNode, matchMode),
                _ => throw new NotSupportedException($"Unsupported condition node: '{conditionNode.GetType().Name}'."),
            };
        }

        private ISpecification<IDictionary<string, Operand>> BuildSpecificationForComposedNode(ComposedConditionNode composedConditionNode, MatchModes matchMode)
        {
            var childConditionNodesSpecifications = composedConditionNode
                .ChildConditionNodes
                .Select(cn => this.BuildSpecification(cn, matchMode));

            return composedConditionNode.LogicalOperator switch
            {
                LogicalOperators.And => childConditionNodesSpecifications.Aggregate((s1, s2) => s1.And(s2)),
                LogicalOperators.Or => childConditionNodesSpecifications.Aggregate((s1, s2) => s1.Or(s2)),
                LogicalOperators.Xor => childConditionNodesSpecifications.Aggregate((s1, s2) => s1.Xor(s2)),
                _ => throw new NotSupportedException($"Unsupported logical operator: '{composedConditionNode.LogicalOperator}'."),
            };
        }

        private ISpecification<IDictionary<string, Operand>> BuildSpecificationForValueNode(IValueConditionNode valueConditionNode, MatchModes matchMode)
        {
            if (matchMode == MatchModes.Search)
            {
                return new SearchLeafSpecification(valueConditionNode, this.conditionsEvalDispatchProvider);
            }

            if (this.rulesEngineOptions.MissingConditionBehavior == MissingConditionBehaviors.Discard)
            {
                return new MatchWithDiscardOnMissingConditionLeafSpecification(valueConditionNode, this.conditionsEvalDispatchProvider);
            }

            return new MatchWithDefaultValueOnMissingConditionLeafSpecification(valueConditionNode, this.conditionsEvalDispatchProvider);
        }
    }
}