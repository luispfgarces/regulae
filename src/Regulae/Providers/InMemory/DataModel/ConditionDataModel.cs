namespace Regulae.Providers.InMemory.DataModel
{
    using System;

    internal sealed class ConditionDataModel
    {
        public required DateTime Creation { get; set; }

        public required DataTypes DataType { get; set; }

        public required string Name { get; set; }
    }
}