namespace Regulae.Evaluation
{
    using System.Collections.Generic;
    using Regulae;

    internal interface IConditionsTreeAnalyzer
    {
        bool AreAllSearchConditionsPresent(IConditionNode conditionNode, IDictionary<string, Operand> conditions);
    }
}