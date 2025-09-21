namespace Regulae.Management
{
    using System.Threading.Tasks;

    internal interface IUpdateRuleController
    {
        ValueTask<OperationResult> UpdateRuleAsync(Rule rule);
        ValueTask<OperationResult> ValidateUpdateRuleAsync(Rule rule);
    }
}
