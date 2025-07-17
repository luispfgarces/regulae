namespace Regulae.Source
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Regulae;

    internal delegate ValueTask<IReadOnlyDictionary<string, Ruleset>> GetRulesetsDelegate(GetRulesetsArgs args);
}