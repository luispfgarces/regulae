namespace Regulae.Providers.InMemory.DataModel
{
    using System;

    internal sealed class ConditionDataModel
    {
        public DateTime Creation { get; set; }

        public DataTypes DataType { get; set; }

        public string Name { get; set; }
    }
}