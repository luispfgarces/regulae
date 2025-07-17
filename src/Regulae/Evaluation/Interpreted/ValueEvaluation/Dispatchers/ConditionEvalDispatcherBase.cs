namespace Regulae.Evaluation.Interpreted.ValueEvaluation.Dispatchers
{
    using System.Collections.Generic;
    using Regulae;
    using Regulae.Evaluation;

    internal abstract class ConditionEvalDispatcherBase
    {
        private readonly IDataTypesConfigurationProvider dataTypesConfigurationProvider;

        protected ConditionEvalDispatcherBase(IDataTypesConfigurationProvider dataTypesConfigurationProvider)
        {
            this.dataTypesConfigurationProvider = dataTypesConfigurationProvider;
        }

        protected static IEnumerable<object> CoalesceMany(IEnumerable<object> operandValue, DataTypeConfiguration dataTypeConfiguration)
            => operandValue ?? (IEnumerable<object>)dataTypeConfiguration.ManyCardinality.Default;

        protected static object CoalesceOne(object operandValue, DataTypeConfiguration dataTypeConfiguration)
                    => operandValue ?? dataTypeConfiguration.OneCardinality.Default;

        protected DataTypeConfiguration GetDataTypeConfiguration(DataTypes dataType)
            => this.dataTypesConfigurationProvider.GetDataTypeConfiguration(dataType);
    }
}