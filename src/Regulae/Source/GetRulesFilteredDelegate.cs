namespace Regulae.Source
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Regulae;

    internal delegate ValueTask<IReadOnlyCollection<Rule>> GetRulesFilteredDelegate(GetRulesFilteredArgs args);
}