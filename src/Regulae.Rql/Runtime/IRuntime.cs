namespace Regulae.Rql.Runtime
{
    using System;
    using System.Threading.Tasks;
    using Regulae.Rql.Runtime.Types;

    internal interface IRuntime
    {
        IRuntimeValue ApplyBinary(IRuntimeValue leftOperand, RqlOperators rqlOperator, IRuntimeValue rightOperand);

        IRuntimeValue ApplyUnary(IRuntimeValue value, RqlOperators rqlOperator);

        ValueTask<RqlArray> GetRulesetsAsync();

        ValueTask<RqlArray> GetUniqueConditionsAsync(string rulesetName, DateTime dateBegin, DateTime dateEnd);

        ValueTask<RqlArray> MatchRulesAsync(MatchRulesArgs matchRulesArgs);

        ValueTask<RqlArray> SearchRulesAsync(SearchRulesArgs searchRulesArgs);
    }
}