namespace Regulae.Providers.InMemory.DataModel
{
    using System.Collections.Generic;
    using Regulae;

    internal class ConditionNodeDataModel
    {
        public required LogicalOperators LogicalOperator { get; set; }

        public required IDictionary<string, object> Properties { get; set; }
    }
}