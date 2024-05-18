namespace Regulae.Source
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Regulae;

    internal delegate Task<IEnumerable<Rule>> GetRulesDelegate(GetRulesArgs args);
}