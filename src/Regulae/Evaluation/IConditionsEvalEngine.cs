namespace Regulae.Evaluation
{
    using System.Collections.Generic;
    using Regulae;

    internal interface IConditionsEvalEngine
    {
        bool Eval(IConditionNode conditionNode, IDictionary<string, object> conditions, EvaluationOptions evaluationOptions);
    }
}