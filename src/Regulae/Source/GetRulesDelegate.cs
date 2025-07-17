namespace Regulae.Source
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Regulae;

    internal delegate ValueTask<IReadOnlyCollection<Rule>> GetRulesDelegate(GetRulesArgs args);
}