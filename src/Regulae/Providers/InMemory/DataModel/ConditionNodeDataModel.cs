namespace Regulae.Providers.InMemory.DataModel
{
    using System.Collections.Generic;
    using Regulae;

    internal class ConditionNodeDataModel
    {
        public LogicalOperators LogicalOperator { get; set; }

        public IDictionary<string, object> Properties { get; set; }
    }
}