namespace Regulae.Core
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    internal interface IConditionsConverter
    {
        ValueTask<IDictionary<string, Operand>> ConvertConditionsAsync(IDictionary<string, object> conditions);
    }
}