namespace Regulae.Evaluation
{
    using Regulae;

    internal interface IDataTypesConfigurationProvider
    {
        DataTypeConfiguration GetDataTypeConfiguration(DataTypes dataType);
    }
}