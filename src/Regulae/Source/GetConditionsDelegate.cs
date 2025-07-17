namespace Regulae.Source
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    internal delegate ValueTask<IReadOnlyDictionary<string, Condition>> GetConditionsDelegate(GetConditionsArgs args);
}