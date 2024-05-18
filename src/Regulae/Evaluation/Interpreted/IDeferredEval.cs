namespace Regulae.Evaluation.Interpreted
{
    using System;
    using System.Collections.Generic;
    using Regulae.ConditionNodes;
    using Regulae.Evaluation;

    internal interface IDeferredEval
    {
        Func<IDictionary<string, object>, bool> GetDeferredEvalFor(IValueConditionNode valueConditionNode, MatchModes matchMode);
    }
}