namespace Regulae.Evaluation
{
    using System;
    using System.Collections.Generic;
    using Regulae;

    internal sealed class DataTypeConfiguration
    {
        private DataTypeConfiguration(
            DataTypes dataType,
            DataTypeCardinalityConfiguration oneCardinality,
            DataTypeCardinalityConfiguration manyCardinality)
        {
            this.DataType = dataType;
            this.OneCardinality = oneCardinality;
            this.ManyCardinality = manyCardinality;
        }

        public DataTypes DataType { get; private set; }

        public DataTypeCardinalityConfiguration ManyCardinality { get; private set; }

        public DataTypeCardinalityConfiguration OneCardinality { get; private set; }

        public static DataTypeConfiguration Create(DataTypes dataType, Type type, object @default)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var oneCardinality = new DataTypeCardinalityConfiguration(type, @default);
            var manyType = typeof(IEnumerable<>).MakeGenericType(type);
            var manyCardinality = new DataTypeCardinalityConfiguration(manyType, Array.CreateInstance(type, 0));
            return new DataTypeConfiguration(dataType, oneCardinality, manyCardinality);
        }
    }
}