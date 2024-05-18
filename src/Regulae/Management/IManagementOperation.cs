namespace Regulae.Management
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Regulae;

    internal interface IManagementOperation
    {
        Task<IEnumerable<Rule>> ApplyAsync(IEnumerable<Rule> rules);
    }
}