namespace Regulae.Evaluation.Compiled
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Regulae;

    internal interface IRuleConditionsExpressionBuilder
    {
        Expression<Func<IDictionary<string, Operand>, bool>> BuildExpression(IConditionNode rootConditionNode, MatchModes matchMode);
    }
}