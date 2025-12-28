namespace Regulae.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Regulae.Source;

    internal class ConditionsConverter : IConditionsConverter
    {
        private static readonly Dictionary<DataTypes, Func<IEnumerable, IEnumerable>> EnumerableConverters = new()
        {
            { DataTypes.String, value => value.Cast<string>().ToArray() },
            { DataTypes.Boolean, value => value.Cast<bool>().ToArray() },
            { DataTypes.Integer, value => value.Cast<int>().ToArray() },
            { DataTypes.Decimal, value => value.Cast<decimal>().ToArray() },
        };

        private static readonly Dictionary<DataTypes, Func<object, object>> SingleConverters = new()
        {
            { DataTypes.String, value => value is string s ? s : Convert.ToString(value, CultureInfo.InvariantCulture)! },
            { DataTypes.Boolean, value => value is bool b ? b : bool.TryParse(value.ToString(), out var bv) ? bv : throw new InvalidCastException($"The value {value} is not convertible to bool.") },
            { DataTypes.Integer, value => value is int i ? i : int.TryParse(value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var iv) ? iv : throw new InvalidCastException($"The value {value} is not convertible to int.") },
            { DataTypes.Decimal, value => value is decimal d ? d : decimal.TryParse(value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var dv) ? dv : throw new InvalidCastException($"The value {value} is not convertible to decimal.") },
        };

        private readonly IRulesSource rulesSource;

        public ConditionsConverter(IRulesSource rulesSource)
        {
            this.rulesSource = rulesSource;
        }

        public async ValueTask<IDictionary<string, Operand>> ConvertConditionsAsync(IDictionary<string, object> conditions)
        {
            var conditionModels = await this.rulesSource.GetConditionsAsync(new GetConditionsArgs()).ConfigureAwait(false);

            var conditionsLeftOperands = new Dictionary<string, Operand>(conditionModels.Count, StringComparer.Ordinal);
            foreach (var condition in conditions)
            {
                if (!conditionModels.TryGetValue(condition.Key, out var conditionModel))
                {
                    throw new ArgumentOutOfRangeException(nameof(conditions), $"The given condition with name '{condition.Key}' does not exist. " +
                        "Please create the condition before using it to evaluate rules.");
                }

                _ = conditionsLeftOperands.TryAdd(condition.Key, ConvertToOperand(condition.Value, conditionModel.DataType));
            }

            return conditionsLeftOperands;
        }

        private static IEnumerable ConvertEnumerable(DataTypes type, IEnumerable values)
        {
            var convertedValues = values is ICollection collection ? new List<object>(collection.Count) : new List<object>();
            foreach (var value in values)
            {
                convertedValues.Add(ConvertSingle(type, value));
            }

            _ = !EnumerableConverters.TryGetValue(type, out var converter);

            return converter!(convertedValues);
        }

        private static object ConvertSingle(DataTypes type, object value)
        {
            if (!SingleConverters.TryGetValue(type, out var converter))
            {
                throw new NotSupportedException($"The data type '{type}' is not supported.");
            }

            return converter(value);
        }

        private static Operand ConvertToOperand(object value, DataTypes dataType)
        {
            if (value is IEnumerable enumerable && value is not string)
            {
                var values = ConvertEnumerable(dataType, enumerable);
                return new Operand(values, dataType, Cardinalities.Many);
            }

            var converted = ConvertSingle(dataType, value);
            return new Operand(converted, dataType, Cardinalities.One);
        }
    }
}