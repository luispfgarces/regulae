namespace Regulae.Management
{
    using System.Threading.Tasks;

    internal interface IAddRuleController
    {
        ValueTask<OperationResult> AddRuleAsync(Rule rule, RuleAddPriorityOption ruleAddPriorityOption);
        ValueTask<OperationResult> ValidateAddRuleAsync(Rule rule, RuleAddPriorityOption ruleAddPriorityOption);
    }
}