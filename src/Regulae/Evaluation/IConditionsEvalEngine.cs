namespace Regulae.Evaluation
{
    using System.Collections.Generic;
    using Regulae;

    internal interface IConditionsEvalEngine
    {
        bool Eval(IConditionNode conditionNode, IDictionary<string, Operand> conditions, EvaluationOptions evaluationOptions);
    }
}