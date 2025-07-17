namespace Regulae.Evaluation.Compiled
{
    using System;
    using System.Collections.Frozen;
    using System.Collections.Generic;
    using Regulae;
    using Regulae.Core;
    using Regulae.Evaluation;

    internal sealed class CompiledConditionsEvalEngine : IConditionsEvalEngine
    {
        private static readonly FrozenDictionary<MatchModes, string> compiledDelegateKeyByMatchMode = new Dictionary<MatchModes, string>
        {
            { MatchModes.Search, ConditionNodeProperties.CompilationProperties.CompiledSearchDelegateKey },
            { MatchModes.Exact, ConditionNodeProperties.CompilationProperties.CompiledMatchDelegateKey },
        }.ToFrozenDictionary();

        private readonly IConditionsTreeAnalyzer conditionsTreeAnalyzer;
        private readonly RulesEngineOptions rulesEngineOptions;

        public CompiledConditionsEvalEngine(
            IConditionsTreeAnalyzer conditionsTreeAnalyzer,
            RulesEngineOptions rulesEngineOptions)
        {
            this.conditionsTreeAnalyzer = conditionsTreeAnalyzer;
            this.rulesEngineOptions = rulesEngineOptions;
        }

        public bool Eval(IConditionNode conditionNode, IDictionary<string, Operand> conditions, EvaluationOptions evaluationOptions)
        {
            if (evaluationOptions.ExcludeRulesWithoutSearchConditions && !this.conditionsTreeAnalyzer.AreAllSearchConditionsPresent(conditionNode, conditions))
            {
                return false;
            }

            if (!compiledDelegateKeyByMatchMode.TryGetValue(evaluationOptions.MatchMode, out var key))
            {
                throw new NotSupportedException($"Match mode '{evaluationOptions.MatchMode}' is not supported.");
            }

            if (!conditionNode.Properties.TryGetValue(key, out var conditionFuncAux))
            {
                throw new ArgumentException("Condition node does not contain compiled information.", nameof(conditionNode));
            }

            return ((Func<IDictionary<string, Operand>, bool>)conditionFuncAux)(conditions);
        }
    }
}