namespace Regulae.Evaluation
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using Regulae;
    using Regulae.Core;

    internal sealed class DataTypeConfiguration
    {
        private DataTypeConfiguration(
            DataTypes dataType,
            DataTypeCardinalityConfiguration oneCardinality,
            DataTypeCardinalityConfiguration manyCardinality,
            Regex valuePattern)
        {
            this.DataType = dataType;
            this.OneCardinality = oneCardinality;
            this.ManyCardinality = manyCardinality;
            this.ValuePattern = valuePattern;
        }

        public DataTypes DataType { get; private set; }

        public DataTypeCardinalityConfiguration ManyCardinality { get; private set; }

        public DataTypeCardinalityConfiguration OneCardinality { get; private set; }

        public Regex ValuePattern { get; set; }

        public static DataTypeConfiguration Create(DataTypes dataType, Type type, object @default)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var oneCardinality = new DataTypeCardinalityConfiguration(type, @default);
            var manyType = typeof(IEnumerable<>).MakeGenericType(type);
            var manyCardinality = new DataTypeCardinalityConfiguration(manyType, Array.CreateInstance(type, 0));
            var valuePatternAttribute = TypesCache.DataTypes
                .GetMember(dataType.ToString())[0]
                .GetCustomAttribute<DataTypeValuePatternAttribute>();
            var valuePattern = new Regex(
                valuePatternAttribute.Pattern,
                RegexOptions.Compiled | RegexOptions.IgnoreCase,
                TimeSpan.FromSeconds(10));
            return new DataTypeConfiguration(dataType, oneCardinality, manyCardinality, valuePattern);
        }
    }
}