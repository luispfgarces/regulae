namespace Regulae.Evaluation
{
    using System;
    using System.Collections.Frozen;
    using System.Collections.Generic;
    using Regulae;

    internal sealed class DataTypesConfigurationProvider : IDataTypesConfigurationProvider
    {
        private readonly FrozenDictionary<DataTypes, DataTypeConfiguration> dataTypeConfigurations;

        public DataTypesConfigurationProvider(RulesEngineOptions rulesEngineOptions)
        {
            this.dataTypeConfigurations = new Dictionary<DataTypes, DataTypeConfiguration>
            {
                { DataTypes.Integer, CreateDataTypeConfiguration<int>(DataTypes.Integer, rulesEngineOptions) },
                { DataTypes.String, CreateDataTypeConfiguration<string>(DataTypes.String, rulesEngineOptions) },
                { DataTypes.Decimal, CreateDataTypeConfiguration<decimal>(DataTypes.Decimal, rulesEngineOptions) },
                { DataTypes.Boolean, CreateDataTypeConfiguration<bool>(DataTypes.Boolean, rulesEngineOptions) },
            }.ToFrozenDictionary();
        }

        public DataTypeConfiguration GetDataTypeConfiguration(DataTypes dataType)
            => this.dataTypeConfigurations.TryGetValue(dataType, out var dataTypeConfiguration)
            ? dataTypeConfiguration
            : throw new NotSupportedException($"Data type '{dataType}' is not supported.");

        private static DataTypeConfiguration CreateDataTypeConfiguration<TRuntimeType>(DataTypes dataType, RulesEngineOptions rulesEngineOptions)
        {
            return DataTypeConfiguration.Create(dataType, typeof(TRuntimeType), rulesEngineOptions.DataTypeDefaults[dataType]);
        }
    }
}