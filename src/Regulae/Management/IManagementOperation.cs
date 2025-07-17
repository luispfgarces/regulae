namespace Regulae.Management
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Regulae;

    internal interface IManagementOperation
    {
        ValueTask<IEnumerable<Rule>> ApplyAsync(IEnumerable<Rule> rules);
    }
}